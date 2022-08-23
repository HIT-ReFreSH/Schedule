using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;

namespace HitRefresh.Schedule;

/// <summary>
///     记载课程的课表条目
/// </summary>
public class CourseEntry
{
    /// <summary>
    ///     课程名称
    /// </summary>
    public string CourseName { get; set; } = string.Empty;

    /// <summary>
    ///     对本课程是否打开提醒
    /// </summary>
    public bool EnableNotification { get; set; }

    /// <summary>
    ///     课程包含的子条目
    /// </summary>
    internal List<CourseContent> SubEntries { get; set; } = new();

    /// <summary>
    ///     返回位置为i的子条目的引用
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public CourseContent this[int i] => SubEntries[i];

    /// <summary>
    ///     课程最大持续周数
    /// </summary>
    [JsonIgnore]
    public int MaxWeek => SubEntries.Select(e => e.MaxWeek).Max();

    /// <summary>
    ///     移除指定的子条目
    /// </summary>
    /// <param name="content"></param>
    public void RemoveSubEntry(CourseContent content)
    {
        SubEntries.Remove(content);
    }

    /// <summary>
    ///     移除指定的子条目
    /// </summary>
    /// <param name="index"></param>
    public void RemoveSubEntryAt(int index)
    {
        SubEntries.RemoveAt(index);
    }

    /// <summary>
    ///     添加内容
    /// </summary>
    /// <param name="dayOfWeek"></param>
    /// <param name="courseTime"></param>
    /// <param name="isLongCourse"></param>
    /// <param name="isLab"></param>
    /// <param name="weekExpression"></param>
    /// <returns>添加内容</returns>
    public CourseContent AddContent(DayOfWeek dayOfWeek, CourseTime courseTime, bool isLongCourse, CourseContentType isLab,
        string weekExpression)
    {
        var entry = new CourseContent(CourseName, dayOfWeek, courseTime, isLongCourse, isLab, weekExpression);
        AddContent(entry);
        return entry;
    }

    /// <summary>
    ///     添加内容
    /// </summary>
    /// <param name="dayOfWeek"></param>
    /// <param name="courseTime"></param>
    /// <param name="isLongCourse"></param>
    /// <param name="isLab"></param>
    /// <param name="weekExpression"></param>
    /// <returns>添加内容</returns>
    public CourseContent AddContent(DayOfWeek dayOfWeek, CourseTime courseTime, bool isLongCourse, CourseContentType isLab,
        Dictionary<int, CourseCell> weekExpression)
    {
        var entry = new CourseContent(CourseName, dayOfWeek, courseTime, isLongCourse, isLab, weekExpression);
        AddContent(entry);
        return entry;
    }

    private void AddContent(CourseContent e)
    {
        SubEntries.Add(e);
    }

    /// <summary>
    ///     枚举课程内容
    /// </summary>
    /// <returns>包含所有课程内容的不可变数组</returns>
    public ImmutableArray<CourseContent> EnumerateContents()
    {
        SubEntries.Sort();
        return SubEntries.ToImmutableArray();
    }

    public void AddGraduateContent(DayOfWeek dayOfWeek, CourseTime courseTime, bool b, CourseContentType isLab, string currentCourse)
    {
        throw new NotImplementedException();
    }
}