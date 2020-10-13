using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static HitRefresh.Schedule.ScheduleStatic;

namespace HitRefresh.Schedule
{
    /// <summary>
    /// 课程代表条目的子条目，实际上是某节课的时间;
    /// </summary>
    public class CourseSubEntry:IEnumerable<(int,CourseCell)>
    {
        /// <summary>
        /// 储存到json
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(new CourseSubEntryJson
            {
                CourseName = CourseName,
                CourseTime = CourseTime,
                IsLab = IsLab,
                IsLongCourse = IsLongCourse,
                DayOfWeek = DayOfWeek,
                WeekInformation = WeekInformation
            });
        }
        private CourseSubEntry()
        {

        }
        private class CourseSubEntryJson
        {
            /// <summary>
            /// 课程名称
            /// </summary>
            public string CourseName { get; set; }
            /// <summary>
            /// 是否为实验课
            /// </summary>
            public bool IsLab { get; set; }
            /// <summary>
            /// 在周几上课
            /// </summary>
            public DayOfWeek DayOfWeek { get; set; }


            /// <summary>
            ///     课程的时间(第几节课)
            /// </summary>
            public CourseTime CourseTime { get; set; }

            /// <summary>
            ///     是否是两节连在一起那种课
            /// </summary>
            public bool IsLongCourse
            {
                get;set;
            }

            /// <summary>
            /// 周数信息，包含上课的周和对应的教师、教室
            /// AF: int周数->(str教师,str教室)
            /// </summary>
            public IDictionary<int, CourseCell> WeekInformation { get; set; } = new Dictionary<int, CourseCell>();

        }
        /// <summary>
        /// 从json产生课表子条目
        /// </summary>
        /// <param name="jsonContent"></param>
        /// <returns></returns>
        public static CourseSubEntry FromJson(string jsonContent)
        {
            
            var j = JsonConvert.DeserializeObject<CourseSubEntryJson>(jsonContent);
            var r = new CourseSubEntry()
            {
                CourseName=j.CourseName,
                CourseTime=j.CourseTime,
                IsLongCourse=j.IsLongCourse,
                DayOfWeek=j.DayOfWeek,
                IsLab=j.IsLab
                


            };
            foreach (var (i,obj) in j.WeekInformation)
            {
                r.WeekInformation.Add(i, obj);
            }
            return r;
        }

        /// <summary>
        /// 课程名称
        /// </summary>
        public string CourseName { get; private set; }
        /// <summary>
        /// 课程最大持续周数
        /// </summary>
        [JsonIgnore]
        public int MaxWeek => WeekInformation.Keys.Max();
        /// <summary>
        /// 新建一个课程条目的子条目
        /// </summary>
        /// <param name="courseName"></param>
        /// <param name="dayOfWeek"></param>
        /// <param name="courseTime"></param>
        /// <param name="isLongCourse"></param>
        /// <param name="isLab"></param>
        /// <param name="weekExpression"></param>
        internal CourseSubEntry(string courseName, DayOfWeek dayOfWeek, CourseTime courseTime, bool isLongCourse, bool isLab, string weekExpression)
        {
            CourseName = courseName;
            DayOfWeek = dayOfWeek;
            CourseTime = courseTime;
            IsLongCourse = isLongCourse;
            IsLab = isLab;

            //Parse Week Expression
            weekExpression = weekExpression.RemoveCommaSpace();
            Logger.LogDebug("WE-Parsing: " + weekExpression);

            var currentTeacher = "";
            var timeStack = new Stack<string>();
            var timeTeacherMap = new Dictionary<string, string>();
            var timeLocationMap = new Dictionary<string, string>();

            foreach (var match in ScheduleExpressionUnitRegex.Matches(weekExpression))
            {
                var unit = match.ToString();
                var unitType = LocationRegex.IsMatch(match.ToString()) ? ScheduleExpressionUnitType.Location :
                TeacherNameRegex.IsMatch(match.ToString()) ? ScheduleExpressionUnitType.Teacher :
                CourseTimeRegex.IsMatch(match.ToString()) ? ScheduleExpressionUnitType.Time :
                ScheduleExpressionUnitType.Unknown;
                Logger.LogDebug($"\t{unit} as {unitType}");

                switch (unitType)
                {
                    case ScheduleExpressionUnitType.Teacher:
                        currentTeacher = unit;
                        break;
                    case ScheduleExpressionUnitType.Time:
                        timeTeacherMap.Add(unit, currentTeacher);
                        timeStack.Push(unit);
                        break;
                    case ScheduleExpressionUnitType.Location:
                        while (timeStack.Count > 0)
                        {
                            timeLocationMap.Add(timeStack.Pop(), unit);
                        }
                        break;
                    case ScheduleExpressionUnitType.Unknown:
                        throw new ArgumentException(weekExpression, nameof(weekExpression), null);
                    default:
                        break;
                }
            }

            while (timeStack.Count > 0)
            {
                timeLocationMap.Add(timeStack.Pop(), $"<{ScheduleMasterString.地点待定}>");
            }

            foreach (var time in timeTeacherMap.Keys)
            {
                foreach (var weekIndex in time.ToIntSequence())
                {
                    WeekInformation.Add(weekIndex, new CourseCell
                    {
                        Name = CourseName,
                        Teacher= timeTeacherMap[time],
                        Location= timeLocationMap[time]
                    });
                }
            }

        }
        /// <summary>
        /// 是否为实验课
        /// </summary>
        public bool IsLab { get; set; }
        /// <summary>
        /// 在周几上课
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }


        /// <summary>
        ///     课程的时间(第几节课)
        /// </summary>
        public CourseTime CourseTime { get; set; }

        /// <summary>
        ///     是否是两节连在一起那种课
        /// </summary>
        public bool IsLongCourse
        {
            get => Length == new TimeSpan(3, 30, 00);
            set
            {
                Length = !value
                    ? new TimeSpan(1, 45, 00)
                    : new TimeSpan(3, 30, 00);
            }
        }


        ///// <summary>
        /////     授课教师
        ///// </summary>
        //[JsonIgnore] 
        //public ICollection<string> Teachers => WeekInformation.Values.Select(v => v.Item1).ToList();

        ///// <summary>
        /////     教室
        ///// </summary>
        //[JsonIgnore] 
        //public ICollection<string> Locations => WeekInformation.Values.Select(v => v.Item2).ToList();

        /// <summary>
        ///     课程的长度
        /// </summary>
        [JsonIgnore]
        public TimeSpan Length { get; private set; }

        /// <summary>
        ///     课程开始的时间距离0点的时长
        /// </summary>
        [JsonIgnore]
        public TimeSpan StartTime => StartTimes[(int)CourseTime];
        /// <summary>
        /// 周数信息，包含上课的周和对应的教师、教室
        /// AF: int周数->(str教师,str教室)
        /// </summary>
        private IDictionary<int, CourseCell> WeekInformation { get; } = new Dictionary<int, CourseCell>();
        /// <summary>
        /// 返回一个按照周的切片
        /// </summary>
        /// <param name="i">周数</param>
        /// <returns>该周的课，如果无课则返回为课名称为空</returns>
        public CourseCell Slice(int i)
        {
            if(WeekInformation.ContainsKey(i))
                return WeekInformation[i];
            return new CourseCell();
        }
        /// <inheritdoc/>
        public IEnumerator<(int, CourseCell)> GetEnumerator()
        {
            return WeekInformation.Select(kv => (kv.Key, kv.Value)).ToList().GetEnumerator();
        }
        /// <inheritdoc/>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// 返回一个按照周的切片
        /// </summary>
        /// <param name="i">周数</param>
        /// <returns>该周的课，如果无课则返回为课名称为空</returns>
        public CourseCell this[int i]=>Slice(i);

    }
}
