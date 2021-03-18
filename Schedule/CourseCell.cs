namespace HitRefresh.Schedule
{
    /// <summary>
    ///     一节课程的基本信息
    /// </summary>
    public record CourseCell
    {
        /// <summary>
        ///     课程名称
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        ///     老师名称
        /// </summary>
        public string Teacher { get; init; } = string.Empty;

        /// <summary>
        ///     课程位置
        /// </summary>
        public string Location { get; init; } = string.Empty;
    }
}