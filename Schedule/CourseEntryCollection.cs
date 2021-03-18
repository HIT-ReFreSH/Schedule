using System.Collections.ObjectModel;

namespace HitRefresh.Schedule
{
    /// <summary>
    ///     课程的集合
    /// </summary>
    public class CourseEntryCollection : KeyedCollection<string, CourseEntry>
    {
        /// <inheritdoc />
        protected override string GetKeyForItem(CourseEntry item)
        {
            return item.CourseName;
        }
    }
}