namespace HitRefresh.Schedule;

/// <summary>
///     课程的时间(第几节课)
/// </summary>
public enum CourseTime
{
    /// <summary>
    ///     中午课
    /// </summary>
    Noon = 0,

    /// <summary>
    ///     上午第一二节
    /// </summary>
    C12 = 1,

    /// <summary>
    ///     上午第三四节
    /// </summary>
    C34 = 2,

    /// <summary>
    ///     下午第五六节
    /// </summary>
    C56 = 3,

    /// <summary>
    ///     下午第七八节
    /// </summary>
    C78 = 4,

    /// <summary>
    ///     晚上第九十节
    /// </summary>
    C9A = 5
}