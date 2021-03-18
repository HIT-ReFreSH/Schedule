using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HitRefresh.Schedule.ScheduleResource
{
    /// <summary>
    /// 课表资源的通用实现
    /// </summary>
    public interface IScheduleResource
    {
        /// <summary>
        /// 每学期开始时间
        /// </summary>
        List<DateTime> SemesterStarts { get; }

        /// <summary>
        /// 课堂开始时间
        /// </summary>
        List<TimeSpan> StartTimes { get; }

        /// <summary>
        /// 用于提取教师名字的正则表达式
        /// </summary>
        Regex TeacherNameRegex { get; }

        /// <summary>
        /// 用于提取课程时间的正则表达式
        /// </summary>
        Regex CourseTimeRegex { get; }

        /// <summary>
        /// 用于提取课程地点的正则表达式
        /// </summary>
        Regex LocationRegex { get; }

        /// <summary>
        /// 用于提取一条课表的正则表达式
        /// </summary>
        Regex ScheduleExpressionUnitRegex { get; }

        /// <summary>
        /// 移出课表表达式中的逗号
        /// </summary>
        /// <param name="source">xls表达式</param>
        /// <returns>处理后的表达式</returns>
        string RemoveCommaSpace(string source);

        /// <summary>
        /// 转换符合CourseTimeRegex的文本为时间序列
        /// </summary>
        /// <param name="source">符合正则表达式<see cref="CourseTimeRegex"/>的文本</param>
        /// <returns>时间序列</returns>
        IEnumerable<int> ToIntSequence(string source);

        /// <summary>
        /// 课表xls文件中课程开始的列数
        /// </summary>
        int ColumnOffset { get; }

        /// <summary>
        /// 课表xls文件中课程开始的行数
        /// </summary>
        int RowOffset { get; }

        /// <summary>
        /// 实验课的标记
        /// </summary>
        string ExperimentLabel { get; }

        /// <summary>
        /// 处理单元格
        /// </summary>
        /// <param name="origin">处理前的字符串</param>
        /// <returns>处理后符合标准的字符串</returns>
        string CellPreprocessing(string origin);
    }
}
