using System;
using System.Collections.Generic;
using System.Text;

namespace HitRefresh.Schedule
{
    
    /// <summary>
    /// 一节课程的基本信息
    /// </summary>
    public struct CourseCell : IEquatable<CourseCell>
    {
        /// <summary>
        /// 课程名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 老师名称
        /// </summary>
        public string Teacher { get; set; }
        /// <summary>
        /// 课程位置
        /// </summary>
        public string Location { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if(obj is CourseCell cell)
            {
                return Equals(cell);
            }
            return false;
        }
        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Teacher, Location);
        }
        /// <inheritdoc/>
        public static bool operator ==(CourseCell left, CourseCell right)
        {
            return left.Equals(right);
        }
        /// <inheritdoc/>
        public static bool operator !=(CourseCell left, CourseCell right)
        {
            return !(left == right);
        }
        /// <inheritdoc/>
        public bool Equals(CourseCell cell)
        {
            return cell.Name == Name &&
                    cell.Teacher == Teacher &&
                    cell.Location == Location;
        }
    }
}
