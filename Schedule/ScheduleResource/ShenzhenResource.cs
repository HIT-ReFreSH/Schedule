using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace HitRefresh.Schedule.ScheduleResource
{
    /// <summary>
    ///     哈尔滨工业大学深圳校区课表资源
    /// </summary>
    public class ShenzhenResource : IScheduleResource
    {
        /// <inheritdoc />
        public List<DateTime> SemesterStarts { get; } = new()
        {
            new(2020, 02, 24),
            new(2020, 06, 29),
            new(2020, 09, 07),
            new(2021, 02, 22),
            new(2021, 07, 12)
        };

        /// <inheritdoc />
        public List<TimeSpan> StartTimes { get; } = new()
        {
            new(12, 30, 00),
            new(08, 30, 00),
            new(10, 30, 00),
            new(14, 00, 00),
            new(16, 00, 00),
            new(18, 30, 00)
        };


        /// <inheritdoc />
        public Regex TeacherNameRegex { get; } = new(@"^\[[\u4e00-\u9fa5]{2,4}\]$|\[^(\w+\s?)+\]$");

        /// <inheritdoc />
        public Regex CourseTimeRegex { get; } = new(@"^\[(((\d+)|((\d+)\-(\d+)))(单|双)?(\|)?)+周\]$");

        /// <inheritdoc />
        public Regex LocationRegex { get; } = new(@"^\[([\u4e00-\u9fa5]+|[A-Z]{1,2})\d{2,5}\]$");

        /// <inheritdoc />
        public Regex ScheduleExpressionUnitRegex { get; } =
            new(
                @"(\[[\u4e00-\u9fa5^0-9]{2,4}\])|(\[^(\w+\s?)+\])|(\[(((\d+)|((\d+)\-(\d+)))(单|双)?(\|)?)+周\])|(\[([\u4e00-\u9fa5]+|[A-Z]{1,2})\d{2,5}\])");

        /// <inheritdoc />
        public string RemoveCommaSpace(string source)
        {
            return source
                .Replace("单周", "单", StringComparison.CurrentCultureIgnoreCase)
                .Replace("双周", "双", StringComparison.CurrentCultureIgnoreCase)
                .Replace("]周", "]", StringComparison.CurrentCultureIgnoreCase) //移出时间表达式后面的“周”
                .Replace(", ", "|", true, CultureInfo.CurrentCulture) //英文逗号+空格
                .Replace("，", "|", true, CultureInfo.CurrentCulture);
        }

        /// <inheritdoc />
        public IEnumerable<int> ToIntSequence(string source)
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
                        if (hasDouble && (i & 1) == 0 ||
                            hasSingle && (i & 1) == 1)
                            r.Add(i);
            }

            return source[^1] switch
            {
                '单' => r.AsParallel().Where(i => (i & 1) == 1).ToList(),
                '双' => r.AsParallel().Where(i => (i & 1) == 0).ToList(),
                _ => r
            };
        }

        /// <inheritdoc />
        public int ColumnOffset { get; } = 1;

        /// <inheritdoc />
        public int RowOffset { get; } = 3;

        /// <inheritdoc />
        public string ExperimentLabel { get; } = "【实验】";

        /// <inheritdoc />
        public string CellPreprocessing(string origin)
        {
            return origin.Replace("]\n", "]", StringComparison.CurrentCulture)
                .Replace("],", "]\n")
                .Replace(",", "|");
        }
    }
}