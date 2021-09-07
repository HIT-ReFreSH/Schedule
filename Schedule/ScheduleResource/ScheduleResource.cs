using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HitRefresh.Schedule.ScheduleResource
{
    /// <summary>
    /// 通用课表静态资源
    /// </summary>
    internal abstract class ScheduleResource:IScheduleResource
    {
        /// <inheritdoc/>
        public abstract List<DateTime> SemesterStarts { get; }
        /// <inheritdoc/>
        public abstract List<TimeSpan> StartTimes { get; }
        /// <inheritdoc/>
        public abstract Regex TeacherNameRegex { get; }
        /// <inheritdoc/>
        public abstract Regex CourseTimeRegex { get; }
        /// <inheritdoc/>
        public abstract Regex LocationRegex { get; }
        /// <inheritdoc/>
        public abstract Regex ScheduleExpressionUnitRegex { get; }
        /// <inheritdoc/>
        public abstract string RemoveCommaSpace(string source);
        /// <inheritdoc/>
        public abstract IEnumerable<int> ToIntSequence(string source);
        /// <inheritdoc/>
        public abstract int ColumnOffset { get; }
        /// <inheritdoc/>
        public abstract int RowOffset { get; }
        /// <inheritdoc/>
        public abstract string ExperimentLabel { get; }
        /// <inheritdoc/>
        public abstract string CellPreprocessing(string origin);
        /// <inheritdoc/>
        public string CourseTimeToFriendlyName(CourseTime courseTime)
        {
            return courseTime switch
            {
                CourseTime.Noon => "中午",
                CourseTime.C12 => "一二节",
                CourseTime.C34 => "三四节",
                CourseTime.C56 => "五六节",
                CourseTime.C78 => "七八节",
                CourseTime.C9A => "晚上",
                _ => throw new ArgumentOutOfRangeException(nameof(courseTime), courseTime, null)
            };
        }
    }
}
