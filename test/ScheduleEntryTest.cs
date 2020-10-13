using System;
using System.Collections.Generic;
using System.Text;
using HitRefresh.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HitRefresh.Schedule.Test
{
    [TestClass]
    public class ScheduleEntryTest
    {
        private CourseEntry entry;

        //[TestInitialize]
        //public void TestInit()
        //{
        //    entry = new ScheduleEntry(DayOfWeek.Monday, CourseTime.C12, "测试用课", "张三[8-15]周格物201");
        //}


        //[TestMethod]
        //public void TestWeekExpressionParse()
        //{
        //    Assert.AreEqual("张三", entry.Teacher);
        //    Assert.AreEqual("格物201", entry.Location);
        //    Assert.AreEqual(15, entry.MaxWeek);
        //    Assert.AreEqual((uint)0b11111111 << 8, entry.Week);
        //    entry.WeekExpression = "1-3，10-13";
        //    Assert.AreEqual(((uint)0b1111 << 10) + 0b1110, entry.Week);
        //    entry = new ScheduleEntry(DayOfWeek.Monday, CourseTime.C12, "测试用课", "张三[7，11]单周格物201");
        //    Assert.AreEqual((uint)0b10001 << 7, entry.Week);
        //}
    }
}
