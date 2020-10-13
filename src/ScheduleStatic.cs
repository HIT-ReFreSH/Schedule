using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using PlasticMetal.MobileSuit.Core;

namespace HitRefresh.Schedule
{
    /// <summary>
    /// 静态资源
    /// </summary>
    public static class ScheduleStatic
    {
        /// <summary>
        /// 转换Json为课表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static ScheduleEntity AsSchedule(this string json) => ScheduleEntity.FromJson(json);
        /// <summary>
        /// 转换Json为课程
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static CourseEntry AsCourse(this string json) => CourseEntry.FromJson(json);
        /// <summary>
        ///     课程的时间(第几节课)
        /// </summary>
        public enum CourseTime
        {
            /// <summary>
            /// 中午课
            /// </summary>
            Noon = 0,

            /// <summary>
            /// 上午第一二节
            /// </summary>
            C12 = 1,

            /// <summary>
            /// 上午第三四节
            /// </summary>
            C34 = 2,

            /// <summary>
            /// 下午第五六节
            /// </summary>
            C56 = 3,

            /// <summary>
            /// 下午第七八节
            /// </summary>
            C78 = 4,

            /// <summary>
            /// 晚上第九十节
            /// </summary>
            C9A = 5
        }
        /// <summary>
        ///     课表中的学期
        /// </summary>
        public enum Semester
        {
            /// <summary>
            /// 春季学期
            /// </summary>
            Spring = 0,

            /// <summary>
            /// 秋季学期
            /// </summary>
            Autumn = 2,

            /// <summary>
            /// 夏季学期
            /// </summary>
            Summer = 1
        }
        /// <summary>
        /// 每学期开始时间
        /// </summary>
        public static IList<DateTime> SemesterStarts { get; } = new[]
        {
            new DateTime(2020, 02, 24),
            new DateTime(2020, 06, 29),
            new DateTime(2020, 09, 07),
            new DateTime(2021, 03, 08),
            new DateTime(2021, 07, 12)

        };
        /// <summary>
        /// 课堂开始时间
        /// </summary>
        public static IList<TimeSpan> StartTimes { get; } = new[]
        {
            new TimeSpan(12, 30, 00),
            new TimeSpan(08, 00, 00),
            new TimeSpan(10, 00, 00),
            new TimeSpan(13, 45, 00),
            new TimeSpan(15, 45, 00),
            new TimeSpan(18, 30, 00)
        };
        /// <summary>
        /// 表达式单元的种类
        /// </summary>
        public enum ScheduleExpressionUnitType
        {
            /// <summary>
            /// 教师
            /// </summary>
            Teacher = 0,
            /// <summary>
            /// 时间信息
            /// </summary>
            Time = 1,
            /// <summary>
            /// 教室信息
            /// </summary>
            Location = 2,
            /// <summary>
            /// 未知，出错了
            /// </summary>
            Unknown = -1
        }
        /// <summary>
        /// Logger
        /// </summary>
        public static Logger Logger { get; } =
        //ILogger.OfFile("D:\\HSM.log");
        ILogger.OfTemp();

        /// <summary>
        /// 老师的名字
        /// </summary>
        public static Regex TeacherNameRegex { get; } = new Regex(@"^[\u4e00-\u9fa5^0-9]{2,4}$|^(\w+\s?)+$");
        /// <summary>
        /// 课程周数表达式
        /// </summary>
        public static Regex CourseTimeRegex { get; } = new Regex(@"^\[(((\d+)|((\d+)\-(\d+)))(单|双)?(\|)?)+\](单|双)?$");
        /// <summary>
        /// 教室名称表达式
        /// </summary>
        public static Regex LocationRegex { get; } = new Regex(@"^([\u4e00-\u9fa5]+|[A-Z]{1,2})\d{2,5}$");
        /// <summary>
        /// 老师、课程时间、位置表达式的和
        /// </summary>
        public static Regex ScheduleExpressionUnitRegex { get; } =
             new Regex(
            @"(([\u4e00-\u9fa5]+|[A-Z]{1,2})\d{2,5})|(\[(((\d+)|((\d+)\-(\d+)))(单|双)?(\|)?)+\](单|双)?)|([\u4e00-\u9fa5]{2,4}|(\w+\s?)+)");
        /// <summary>
        /// 第几节课
        /// </summary>
        /// <param name="courseTime"></param>
        /// <returns></returns>
        public static string ToFriendlyName(this CourseTime courseTime)
        {
            return courseTime switch
            {
                CourseTime.Noon => ScheduleMasterString.中午,
                CourseTime.C12 => ScheduleMasterString.一二节,
                CourseTime.C34 => ScheduleMasterString.三四节,
                CourseTime.C56 => ScheduleMasterString.五六节,
                CourseTime.C78 => ScheduleMasterString.七八节,
                CourseTime.C9A => ScheduleMasterString.晚上,
                _ => throw new ArgumentOutOfRangeException(nameof(courseTime), courseTime, null)
            };
        }
        /// <summary>
        ///     周几
        /// </summary>
        public static string ToFriendlyName(this DayOfWeek dayOfWeek)
            => CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(dayOfWeek);
        /// <summary>
        /// 移出课表表达式中的各种奇怪逗号
        /// </summary>
        /// <param name="source">xls表达式</param>
        /// <returns>净化后的表达式</returns>
        public static string RemoveCommaSpace(this string source)
        {
            return source?
                .Replace("单周", "单", StringComparison.CurrentCultureIgnoreCase)
                .Replace("双周", "双", StringComparison.CurrentCultureIgnoreCase)
                .Replace("]周", "]", StringComparison.CurrentCultureIgnoreCase)//移出时间表达式后面的“周”
                .Replace(", ", "|", true, CultureInfo.CurrentCulture) //英文逗号+空格
                .Replace("，", "|", true, CultureInfo.CurrentCulture) //中文逗号
                                                                     //.Replace(" ", "|", true, CultureInfo.CurrentCulture) //手动输入的空格
                ?? "";
        }
        /// <summary>
        /// 转换符合CourseTimeRegex的文本为时间序列
        /// </summary>
        /// <param name="source">符合CourseTimeRegex的文本</param>
        /// <returns></returns>
        public static IEnumerable<int> ToIntSequence(this string source)
        {
            if (!CourseTimeRegex.IsMatch(source)
                || source == null) throw new ArgumentOutOfRangeException(nameof(source), source, null);
            var r = new List<int>();
            var subWeekExpression = source.Split('|');

            foreach (var s in subWeekExpression)
            {
                var hasSingle = !s.Contains('双', StringComparison.CurrentCultureIgnoreCase);
                var hasDouble = !s.Contains('单', StringComparison.CurrentCultureIgnoreCase);


                var weekRange =
                    Regex.Matches(s, @"\d+").AsParallel()
                         .Select(w => int.Parse(w.Value, CultureInfo.CurrentCulture.NumberFormat))
                         .ToList();


                if (weekRange.Count == 0) continue;
                if (weekRange.Count == 1)
                    r.Add(weekRange[0]);
                else
                    for (var i = weekRange[0]; i <= weekRange[1]; i++)
                        if ((hasDouble && (i & 1) == 0) ||
                            (hasSingle && (i & 1) == 1)) r.Add(i);


            }
            return source[^1] switch
            {
                '单' => r.AsParallel().Where(i => (i & 1) == 1).ToList(),
                '双' => r.AsParallel().Where(i => (i & 1) == 0).ToList(),
                _ => r
            };
        }
    }
}
