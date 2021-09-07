using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using ExcelDataReader;
using HitRefresh.Schedule.ScheduleResource;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Calendar = Ical.Net.Calendar;

namespace HitRefresh.Schedule
{
    /// <summary>
    ///     课表实例
    /// </summary>
    public class ScheduleEntity 
    {
        /// <summary>
        ///     指定年份和学期创建空的课表
        /// </summary>
        /// <param name="year">要创建课表的年份</param>
        /// <param name="semester">要创建课表的学期</param>
        public ScheduleEntity(int year, Semester semester)
        {
            Year = year;
            Semester = semester;
        }

        /// <summary>
        ///     创建空的课表，学期和季节为默认
        /// </summary>
        public ScheduleEntity()
        {
        }

        /// <summary>
        ///     课程最大持续周数
        /// </summary>
        public int MaxWeek => Entries.Select(e => e.MaxWeek).Max();

        /// <summary>
        ///     日历映射系统
        /// </summary>
        public IDictionary<DateTime, DateTime?> DateMap { get; } = new Dictionary<DateTime, DateTime?>();

        /// <summary>
        ///     对本课程表是否打开提醒
        /// </summary>
        public bool? EnableNotification
        {
            get
            {
                bool? r = null;
                foreach (var entry in EnumerateCourseEntries())
                    if (r == null)
                        r = entry.EnableNotification;
                    else if (r != entry.EnableNotification)
                        return null;
                return r;
            }
            set
            {
                if (value == null) return;
                var b = (bool) value;
                foreach (var entry in EnumerateCourseEntries()) entry.EnableNotification = b;
            }
        }

        /// <summary>
        ///     提醒发送的时间
        /// </summary>
        public int NotificationTime { get; set; } = 25;

        /// <summary>
        ///     是否禁用周序号索引
        ///     如果设为true，则不会在每一周的日历事件上添加周索引事件。默认为false
        /// </summary>
        public bool DisableWeekIndex { get; set; }

        /// <summary>
        ///     获取指定的课表条目
        /// </summary>
        /// <param name="index">课表条目的索引</param>
        /// <returns>实例中储存的课表条目实例</returns>
        public CourseEntry this[int index] => Entries[index];

        /// <summary>
        ///     当前课表中所有的条目
        /// </summary>
        private CourseEntryCollection Entries { get; } = new();

        /// <summary>
        ///     获取指定名称的课程，没有则添加
        /// </summary>
        /// <param name="courseName"></param>
        /// <returns></returns>
        public CourseEntry this[string courseName]
        {
            get
            {
                if (!Contains(courseName))
                    Add(courseName);
                return Entries[courseName];
            }
        }

        /// <summary>
        ///     课表的年份
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        ///     课表学期开始的时间
        /// </summary>
        [JsonIgnore]
        public DateTime SemesterStart => ResourceProvider.Resource.SemesterStarts[(Year - 2020) * 3 + (int) Semester];

        /// <summary>
        ///     课表的学期
        /// </summary>

        public Semester Semester { get; set; }

        /// <summary>
        ///     判断是否有指定名称的课程
        /// </summary>
        /// <param name="courseName"></param>
        /// <returns></returns>
        public bool Contains(string courseName)
        {
            return Entries.Contains(courseName);
        }

        /// <summary>
        ///     添加具有指定名称的课程
        /// </summary>
        /// <param name="courseName"></param>
        /// <returns></returns>
        private void Add(string courseName)
        {
            Entries.Add(new() {CourseName = courseName});
        }

        /// <summary>
        ///     添加具有指定名称的课程
        /// </summary>
        /// <param name="courseName"></param>
        /// <returns></returns>
        public void Remove(string courseName)
        {
            Entries.Remove(courseName);
        }

        /// <summary>
        ///     添加具有指定名称的课程
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public void Add(CourseEntry entry)
        {
            Entries.Add(entry);
        }

        /// <summary>
        ///     从已经打打开的XLS流中读取并创建课表
        /// </summary>
        /// <param name="inputStream">输入的流</param>
        public static ScheduleEntity FromXls(Stream inputStream)
        {
            //var res = new ResourceManager(typeof(ScheduleMasterString));
            //Fix codepage
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //I want to say f-word here, but no idea to microsoft, mono or ExcelDataReader
            using var reader = ExcelReaderFactory.CreateReader(inputStream);
            var table = reader.AsDataSet().Tables[0];
            if (!(table.Rows[0][0] is string tableHead))
                throw new ArgumentException("课表格式错误");

            var schedule = new ScheduleEntity(
                int.Parse(tableHead[..4], CultureInfo.GetCultureInfo("zh-Hans").NumberFormat),
                tableHead[4] switch
                {
                    '春' => Semester.Spring,
                    '夏' => Semester.Summer,
                    _ => Semester.Autumn
                });

            for (var i = 0; i < 7; i++) //列
            for (var j = 0; j < 5; j++) //行
            {
                var current = table.Rows[j + 2][i + 2] as string;
                if (string.IsNullOrWhiteSpace(current))
                    continue;
                var next = table.Rows[j + 3][i + 2] as string;
                var currentCourses = current.Replace("周\n", "周", StringComparison.CurrentCulture).Split('\n');

                if (currentCourses.Length % 2 != 0)
                    throw new ArgumentException("课表格式错误");
                for (var c = 0; c < currentCourses.Length; c += 2)
                {
                    var courseName = currentCourses[c];
                    var isLab = false;
                    if (courseName.Contains("(实验)", StringComparison.CurrentCultureIgnoreCase))
                    {
                        isLab = true;
                        courseName = courseName.Replace("(实验)", "", StringComparison.CurrentCultureIgnoreCase);
                    }

                    if (!schedule.Entries.Contains(courseName)) schedule.Entries.Add(new() {CourseName = courseName});
                    schedule.Entries[courseName].AddContent(
                        (DayOfWeek) ((i + 1) % 7),
                        (CourseTime) (j + 1),
                        current == next,
                        isLab,
                        currentCourses[c + 1]);
                }

                if (current == next) j++;
            }

            return schedule;
        }

        /// <summary>
        ///     将当前课表实例转化为日历
        /// </summary>
        /// <returns>表示当前课表的日历实例</returns>
        public Calendar ToCalendar()
        {
            var calendar = new Calendar();
            calendar.AddTimeZone(new VTimeZone("Asia/Shanghai"));
            if (!DisableWeekIndex)
            {
                var maxWeek = MaxWeek;
                for (var i = 0; i < maxWeek; i++)
                {
                    var courseDate = SemesterStart.AddDays(i * 7).AddHours(7);

                    var cEvent = new CalendarEvent
                    {
                        Start = new CalDateTime(courseDate),
                        Duration = TimeSpan.FromHours(0),
                        Summary = $"第{i + 1}周",
                        RecurrenceRules = new List<RecurrencePattern>
                        {
                            new(FrequencyType.Daily, 1)
                            {
                                Count = 7
                            }
                        }
                    };


                    cEvent.Alarms.Clear();

                    calendar.Events.Add(cEvent);
                }
            }

            foreach (var entry in Entries)
            foreach (var subEntry in entry.EnumerateContents())
            {
                //var i = 0;
                var dayOfWeek = subEntry.DayOfWeek == DayOfWeek.Sunday ? 6 : (int) subEntry.DayOfWeek - 1;
                foreach (var (i, item) in subEntry.EnumerateInformation())
                {
                    var courseDate = SemesterStart.AddDays((i - 1) * 7 + dayOfWeek);

                    if (DateMap?.ContainsKey(courseDate) == true)
                    {
                        var date = DateMap[courseDate];
                        if (date == null)
                            continue;
                        courseDate = (DateTime) date;
                    }

                    courseDate += subEntry.StartTime;
                    var cEvent = new CalendarEvent
                    {
                        Location = item.Location,
                        Start = new CalDateTime(courseDate),
                        Duration = subEntry.Length,
                        Summary = $"{entry.CourseName}{(subEntry.IsLab ? "(实验)" : "")} by {item.Teacher}"
                    };

                    if (entry.EnableNotification)
                        cEvent.Alarms.Add(new()
                        {
                            Summary = string.Format(CultureInfo.CurrentCulture,
                                "您在{0}有一节{1}即将开始",
                                item.Location, entry.CourseName),
                            Action = AlarmAction.Display,
                            Trigger = new(TimeSpan.FromMinutes(-NotificationTime))
                        });
                    else
                        cEvent.Alarms.Clear();
                    calendar.Events.Add(cEvent);
                }
            }

            return calendar;
        }

        /// <summary>
        ///     枚举课程
        /// </summary>
        /// <returns>包含所有课程的不可变数组</returns>
        public ImmutableArray<CourseEntry> EnumerateCourseEntries()
        {
            return Entries.ToImmutableArray();
        }

    }
}