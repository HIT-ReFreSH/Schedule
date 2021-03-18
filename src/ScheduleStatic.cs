using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using PlasticMetal.MobileSuit.Logging;
using PlasticMetal.MobileSuit.Core;

namespace HitRefresh.Schedule
{
    /// <summary>
    /// 所在校区
    /// </summary>
    public enum Region
    {
        /// <summary>
        /// 本部
        /// </summary>
        Harbin = 0,
        /// <summary>
        /// 深圳校区
        /// </summary>
        ShenZhen = 1
    }
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
        /// Logger
        /// </summary>
        public static ISuitLogger Logger { get; } =
            //ISuitLogger.CreateFileByPath("D:\\HSM2021.log");
            ISuitLogger.CreateEmpty();

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


    }
}
