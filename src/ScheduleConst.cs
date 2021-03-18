using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace HitRefresh.Schedule
{
    /// <summary>
    /// 用于各校区的课表常量
    /// </summary>
    public class ScheduleConst
    {
        private readonly Region _region;

        /// <summary>
        /// 本部静态资源
        /// </summary>
        private static class Hit
        {
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
            /// 移出课表表达式中的各种奇怪逗号
            /// </summary>
            /// <param name="source">xls表达式</param>
            /// <returns>净化后的表达式</returns>
            public static string RemoveCommaSpace(string source)
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
            public static IEnumerable<int> ToIntSequence(string source)
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

            public static int ColumnOffset { get; } = 2;
            public static int RowOffset { get; } = 2;
            public static string ExperimentLabel { get; } = "(实验)";

            public static string CellPreprocessing(string origin)
                => origin.Replace("周\n", "周", StringComparison.CurrentCulture);
        }
        /// <summary>
        /// 深圳校区静态资源
        /// </summary>
        private static class HitSz
        {
            /// <summary>
            /// 每学期开始时间
            /// </summary>
            public static IList<DateTime> SemesterStarts { get; } = new[]
            {
            new DateTime(2020, 02, 24),
            new DateTime(2020, 06, 29),
            new DateTime(2020, 09, 07),
            new DateTime(2021, 02, 22),
            new DateTime(2021, 07, 12)

        };
            /// <summary>
            /// 课堂开始时间
            /// </summary>
            public static IList<TimeSpan> StartTimes { get; } = new[]
            {
            new TimeSpan(12, 30, 00),
            new TimeSpan(08, 30, 00),
            new TimeSpan(10, 30, 00),
            new TimeSpan(14, 00, 00),
            new TimeSpan(16, 00, 00),
            new TimeSpan(18, 30, 00)
        };



            /// <summary>
            /// 老师的名字
            /// </summary>
            public static Regex TeacherNameRegex { get; } = new Regex(@"^\[[\u4e00-\u9fa5]{2,4}\]$|\[^(\w+\s?)+\]$");
            /// <summary>
            /// 课程周数表达式
            /// </summary>
            public static Regex CourseTimeRegex { get; } = new Regex(@"^\[(((\d+)|((\d+)\-(\d+)))(单|双)?(\|)?)+周\]$");
            /// <summary>
            /// 教室名称表达式
            /// </summary>
            public static Regex LocationRegex { get; } = new Regex(@"^\[([\u4e00-\u9fa5]+|[A-Z]{1,2})\d{2,5}\]$");
            /// <summary>
            /// 老师、课程时间、位置表达式的和
            /// </summary>
            public static Regex ScheduleExpressionUnitRegex { get; } =
                 new Regex(@"(\[[\u4e00-\u9fa5^0-9]{2,4}\])|(\[^(\w+\s?)+\])|(\[(((\d+)|((\d+)\-(\d+)))(单|双)?(\|)?)+周\])|(\[([\u4e00-\u9fa5]+|[A-Z]{1,2})\d{2,5}\])");
            /// <summary>
            /// 移出课表表达式中的各种奇怪逗号
            /// </summary>
            /// <param name="source">xls表达式</param>
            /// <returns>净化后的表达式</returns>
            public static string RemoveCommaSpace(string source)
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
            public static IEnumerable<int> ToIntSequence(string source)
            {
                //source = $"[{source}]";
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

            public static int ColumnOffset { get; } = 1;
            public static int RowOffset { get; } = 3;
            public static string ExperimentLabel { get; } = "【实验】";

            public static string CellPreprocessing(string origin)
                => origin.Replace("]\n", "]", StringComparison.CurrentCulture)
                    .Replace("],", "]\n")
                    .Replace(",", "|");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        public ScheduleConst(Region region)
        {
            _region = region;
        }

        /// <summary>
        /// 每学期开始时间
        /// </summary>
        public IList<DateTime> SemesterStarts => _region switch
        {
            Region.Harbin => Hit.SemesterStarts,
            _ => HitSz.SemesterStarts
        };

        /// <summary>
        /// 课堂开始时间
        /// </summary>
        public IList<TimeSpan> StartTimes => _region switch
        {
            Region.Harbin => Hit.StartTimes,
            _ => HitSz.StartTimes
        };

        /// <summary>
        /// 老师的名字
        /// </summary>
        public Regex TeacherNameRegex => _region switch
        {
            Region.Harbin => Hit.TeacherNameRegex,
            _ => HitSz.TeacherNameRegex
        };

        /// <summary>
        /// 课程周数表达式
        /// </summary>
        public Regex CourseTimeRegex => _region switch
        {
            Region.Harbin => Hit.CourseTimeRegex,
            _ => HitSz.CourseTimeRegex
        };
        /// <summary>
        /// 教室名称表达式
        /// </summary>
        public Regex LocationRegex => _region switch
        {
            Region.Harbin => Hit.LocationRegex,
            _ => HitSz.LocationRegex
        };
        /// <summary>
        /// 老师、课程时间、位置表达式的和
        /// </summary>
        public Regex ScheduleExpressionUnitRegex => _region switch
        {
            Region.Harbin => Hit.ScheduleExpressionUnitRegex,
            _ => HitSz.ScheduleExpressionUnitRegex
        };
        /// <summary>
        /// 移出课表表达式中的各种奇怪逗号
        /// </summary>
        /// <param name="source">xls表达式</param>
        /// <returns>净化后的表达式</returns>
        public string RemoveCommaSpace(string source)
          => _region switch
          {
              Region.Harbin => Hit.RemoveCommaSpace(source),
              _ => HitSz.RemoveCommaSpace(source)
          };

        /// <summary>
        /// 转换符合CourseTimeRegex的文本为时间序列
        /// </summary>
        /// <param name="source">符合CourseTimeRegex的文本</param>
        /// <returns></returns>
        public IEnumerable<int> ToIntSequence(string source)
            => _region switch
            {
                Region.Harbin => Hit.ToIntSequence(source),
                _ => HitSz.ToIntSequence(source)
            };
        /// <summary>
        /// xls中的列偏移
        /// </summary>
        public int ColumnOffset
        => _region switch
        {
            Region.Harbin => Hit.ColumnOffset,
            _ => HitSz.ColumnOffset
        };
        /// <summary>
        /// xls中的行偏移
        /// </summary>
        public int RowOffset => _region switch
        {
            Region.Harbin => Hit.RowOffset,
            _ => HitSz.RowOffset
        };
        /// <summary>
        /// 实验课标签
        /// </summary>
        public string ExperimentLabel => _region switch
        {
            Region.Harbin => Hit.ExperimentLabel,
            _ => HitSz.ExperimentLabel
        };
        /// <summary>
        /// 单元格预处理
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public string CellPreprocessing(string origin)
            => _region switch
            {
                Region.Harbin => Hit.CellPreprocessing(origin),
                _ => HitSz.CellPreprocessing(origin)
            };
    }
}