using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static HitRefresh.Schedule.ScheduleStatic;

namespace HitRefresh.Schedule
{
    /// <summary>
    /// 记载课程的课表条目
    /// </summary>
    public class CourseEntry:IEnumerable<CourseSubEntry>
    {
        private class CourseEntryJson
        {
            public string CourseName { get; set; }
            public bool EnableNotification { get; set; }
            public List<string> SubEntries { get; set; } = new List<string>();
        }
        /// <summary>
        ///     从json生成课程
        /// </summary>
        public static CourseEntry FromJson(string jsonContent)
        {
            var j = JsonConvert.DeserializeObject<CourseEntryJson>(jsonContent);
            var r = new CourseEntry()
            {
                CourseName = j.CourseName,
                EnableNotification = j.EnableNotification
            };
            foreach (var item in j.SubEntries)
            {
                r.AddSubEntry(CourseSubEntry.FromJson(item));
            }
            return r;
        }
        /// <summary>
        /// 储存到json
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(new CourseEntryJson
            {
                CourseName = CourseName,
                EnableNotification = EnableNotification,
                SubEntries = SubEntries.Select(se => se.ToJson()).ToList()
            }); ;
        }
        private CourseEntry()
        {
        }
        /// <summary>
        /// 课程名称
        /// </summary>
        public string CourseName { get; set; }
        /// <summary>
        ///     创造一个课表条目实例
        /// </summary>
        /// <param name="courseName">课程的名称</param>
        public CourseEntry(string courseName)
        {
            CourseName = courseName;
            Logger.LogDebug(courseName);


        }
        /// <summary>
        /// 对本课程是否打开提醒
        /// </summary>
        public bool EnableNotification { get; set; }
        /// <summary>
        /// 课程包含的子条目
        /// </summary>
        private List<CourseSubEntry> SubEntries { get;} = new List<CourseSubEntry>(); //SortedList<DayOfWeek,CourseSubEntry>();

        /// <summary>
        /// 返回位置为i的子条目的引用
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public CourseSubEntry this[int i] =>SubEntries[i];
        /// <summary>
        /// 移除位置为index的子条目
        /// </summary>
        /// <param name="index"></param>
        public void RemoveSubEntry(int index) => SubEntries.RemoveAt(index);
        /// <summary>
        /// 移除指定的子条目
        /// </summary>
        /// <param name="subEntry"></param>
        public void RemoveSubEntry(CourseSubEntry subEntry) => SubEntries.Remove(subEntry);
        /// <summary>
        /// 课程最大持续周数
        /// </summary>
        [JsonIgnore]
        public int MaxWeek => SubEntries.Select(e => e.MaxWeek).Max();
        /// <summary>
        /// 添加子条目
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <param name="courseTime"></param>
        /// <param name="isLongCourse"></param>
        /// <param name="isLab"></param>
        /// <param name="weekExpression"></param>
        /// <returns></returns>
        public CourseSubEntry AddSubEntry(DayOfWeek dayOfWeek, CourseTime courseTime, bool isLongCourse, bool isLab, string weekExpression)
        {
            var r = new CourseSubEntry(CourseName, dayOfWeek, courseTime, isLongCourse, isLab, weekExpression);
            AddSubEntry(r);
            return r;

        }

        private void AddSubEntry(CourseSubEntry e)
        {
            
            SubEntries.Add(e);
            SubEntries.Sort((c1, c2) =>
            {
                return (int)c1.DayOfWeek * 10 + c1.CourseTime - (int)c2.DayOfWeek * 10 - c2.CourseTime;
            });

        }


        /// <inheritdoc/>
        public IEnumerator<CourseSubEntry> GetEnumerator()
        {
            return SubEntries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

