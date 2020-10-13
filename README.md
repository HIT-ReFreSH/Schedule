# HIT-ReFreSH/Schedule

![GitHub Workflow Status](https://img.shields.io/github/workflow/status/HIT-ReFreSH/Schedule/publish%20to%20nuget?label=publish) ![GitHub Workflow Status](https://img.shields.io/github/workflow/status/HIT-ReFreSH/Schedule/test) ![GitHub issues](https://img.shields.io/github/issues/HIT-ReFreSH/Schedule) ![GitHub tag (latest by date)](https://img.shields.io/github/v/tag/HIT-ReFreSH/Schedule) ![GitHub](https://img.shields.io/github/license/HIT-ReFreSH/Schedule) ![Nuget](https://img.shields.io/nuget/dt/HCGStudio.HITScheduleMasterCore) ![Codecov](https://img.shields.io/codecov/c/gh/HIT-ReFreSH/Schedule)

解析HIT课程表的核心库，帮助转化XLS格式的课表！目前版本已经可以较稳定使用！下载请到nuget，附带XML注释。

快速开始：

``` csharp
using var fs = File.OpenRead(path);
var schedule = ScheduleEntity.FromXls(fs);
var cal = Schedule.GetCalendar();
var str = new CalendarSerializer().SerializeToString(cal);
```

对于导入的课表，本库也提供了编辑的方法。

[View at Nuget.org](https://www.nuget.org/packages/HitRefresh.Schedule/)
