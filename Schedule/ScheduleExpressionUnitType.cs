namespace HitRefresh.Schedule
{
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
}