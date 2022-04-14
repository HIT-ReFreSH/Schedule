namespace HitRefresh.Schedule.ScheduleResource;

/// <summary>
///     提供区域相关课表资源
/// </summary>
public static class ResourceProvider
{
    /// <summary>
    ///     当前使用的课表资源
    /// </summary>
    public static IScheduleResource Resource { get; set; } = new HarbinResource();
}