using CalendarManagementApi.Models;
using CalendarManagementApi.Services;
using CalendarManagementApi.Tests.Helpers;
using NUnit.Framework;

namespace CalendarManagementApi.Tests.Services;

[TestFixture]
public class CalendarServiceTests
{
    private CalendarService _service = null!;

    [SetUp]
    public void SetUp()
    {
        var context = TestDbContextFactory.Create();
        _service = new CalendarService(context);
    }

    #region CheckDayOfWeek Tests

    [Test]
    public void CheckDayOfWeek_WhenDateMatchesDayOfWeek_ReturnsTrue()
    {
        // Monday = 1
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.DayOfWeek,
            DayOfWeek = 1 // Monday
        };
        var monday = new DateOnly(2026, 1, 26); // A Monday

        var result = _service.CheckDayOfWeek(repeatEvent, monday);

        Assert.That(result, Is.True);
    }

    [Test]
    public void CheckDayOfWeek_WhenDateDoesNotMatchDayOfWeek_ReturnsFalse()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.DayOfWeek,
            DayOfWeek = 1 // Monday
        };
        var tuesday = new DateOnly(2026, 1, 27); // A Tuesday

        var result = _service.CheckDayOfWeek(repeatEvent, tuesday);

        Assert.That(result, Is.False);
    }

    [Test]
    public void CheckDayOfWeek_WhenDayOfWeekIsNull_ReturnsFalse()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.DayOfWeek,
            DayOfWeek = null
        };
        var date = new DateOnly(2026, 1, 26);

        var result = _service.CheckDayOfWeek(repeatEvent, date);

        Assert.That(result, Is.False);
    }

    [Test]
    public void CheckDayOfWeek_Sunday_IsZero()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.DayOfWeek,
            DayOfWeek = 0 // Sunday
        };
        var sunday = new DateOnly(2026, 1, 25); // A Sunday

        var result = _service.CheckDayOfWeek(repeatEvent, sunday);

        Assert.That(result, Is.True);
    }

    [Test]
    public void CheckDayOfWeek_Saturday_IsSix()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.DayOfWeek,
            DayOfWeek = 6 // Saturday
        };
        var saturday = new DateOnly(2026, 1, 24); // A Saturday

        var result = _service.CheckDayOfWeek(repeatEvent, saturday);

        Assert.That(result, Is.True);
    }

    #endregion

    #region CheckDayOfWeekOfMonth Tests

    [Test]
    public void CheckDayOfWeekOfMonth_WhenDayAndWeekMatch_ReturnsTrue()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.DayOfWeekOfMonth,
            DayOfWeek = 1, // Monday
            WeeksOfMonth = "1" // First week
        };
        var firstMonday = new DateOnly(2026, 1, 5); // First Monday of January 2026

        var result = _service.CheckDayOfWeekOfMonth(repeatEvent, firstMonday);

        Assert.That(result, Is.True);
    }

    [Test]
    public void CheckDayOfWeekOfMonth_WhenDayMatchesButWeekDoesNot_ReturnsFalse()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.DayOfWeekOfMonth,
            DayOfWeek = 1, // Monday
            WeeksOfMonth = "1" // First week only
        };
        var secondMonday = new DateOnly(2026, 1, 12); // Second Monday of January 2026

        var result = _service.CheckDayOfWeekOfMonth(repeatEvent, secondMonday);

        Assert.That(result, Is.False);
    }

    [Test]
    public void CheckDayOfWeekOfMonth_WhenDayOfWeekIsNull_ReturnsFalse()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.DayOfWeekOfMonth,
            DayOfWeek = null,
            WeeksOfMonth = "1"
        };
        var date = new DateOnly(2026, 1, 5);

        var result = _service.CheckDayOfWeekOfMonth(repeatEvent, date);

        Assert.That(result, Is.False);
    }

    [Test]
    public void CheckDayOfWeekOfMonth_WhenWeeksOfMonthIsNull_ReturnsFalse()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.DayOfWeekOfMonth,
            DayOfWeek = 1,
            WeeksOfMonth = null
        };
        var date = new DateOnly(2026, 1, 5);

        var result = _service.CheckDayOfWeekOfMonth(repeatEvent, date);

        Assert.That(result, Is.False);
    }

    [Test]
    public void CheckDayOfWeekOfMonth_WhenWeeksOfMonthIsEmpty_ReturnsFalse()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.DayOfWeekOfMonth,
            DayOfWeek = 1,
            WeeksOfMonth = ""
        };
        var date = new DateOnly(2026, 1, 5);

        var result = _service.CheckDayOfWeekOfMonth(repeatEvent, date);

        Assert.That(result, Is.False);
    }

    [Test]
    public void CheckDayOfWeekOfMonth_WithMultipleWeeks_MatchesCorrectly()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.DayOfWeekOfMonth,
            DayOfWeek = 1, // Monday
            WeeksOfMonth = "1,3" // First and third weeks
        };
        var firstMonday = new DateOnly(2026, 1, 5);  // Week 1
        var secondMonday = new DateOnly(2026, 1, 12); // Week 2
        var thirdMonday = new DateOnly(2026, 1, 19);  // Week 3

        Assert.Multiple(() =>
        {
            Assert.That(_service.CheckDayOfWeekOfMonth(repeatEvent, firstMonday), Is.True);
            Assert.That(_service.CheckDayOfWeekOfMonth(repeatEvent, secondMonday), Is.False);
            Assert.That(_service.CheckDayOfWeekOfMonth(repeatEvent, thirdMonday), Is.True);
        });
    }

    [Test]
    public void CheckDayOfWeekOfMonth_WhenWeekMatchesButDayDoesNot_ReturnsFalse()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.DayOfWeekOfMonth,
            DayOfWeek = 1, // Monday
            WeeksOfMonth = "1"
        };
        var firstTuesday = new DateOnly(2026, 1, 6); // Tuesday, week 1

        var result = _service.CheckDayOfWeekOfMonth(repeatEvent, firstTuesday);

        Assert.That(result, Is.False);
    }

    #endregion

    #region CheckInterval Tests

    [Test]
    public void CheckInterval_OnStartDate_ReturnsTrue()
    {
        var startDate = new DateOnly(2026, 1, 1);
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.Interval,
            StartDate = startDate,
            IntervalDays = 7
        };

        var result = _service.CheckInterval(repeatEvent, startDate);

        Assert.That(result, Is.True);
    }

    [Test]
    public void CheckInterval_OnExactInterval_ReturnsTrue()
    {
        var startDate = new DateOnly(2026, 1, 1);
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.Interval,
            StartDate = startDate,
            IntervalDays = 7
        };
        var oneWeekLater = new DateOnly(2026, 1, 8);
        var twoWeeksLater = new DateOnly(2026, 1, 15);

        Assert.Multiple(() =>
        {
            Assert.That(_service.CheckInterval(repeatEvent, oneWeekLater), Is.True);
            Assert.That(_service.CheckInterval(repeatEvent, twoWeeksLater), Is.True);
        });
    }

    [Test]
    public void CheckInterval_BeforeStartDate_ReturnsFalse()
    {
        var startDate = new DateOnly(2026, 1, 10);
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.Interval,
            StartDate = startDate,
            IntervalDays = 7
        };
        var beforeStart = new DateOnly(2026, 1, 5);

        var result = _service.CheckInterval(repeatEvent, beforeStart);

        Assert.That(result, Is.False);
    }

    [Test]
    public void CheckInterval_OnNonIntervalDate_ReturnsFalse()
    {
        var startDate = new DateOnly(2026, 1, 1);
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.Interval,
            StartDate = startDate,
            IntervalDays = 7
        };
        var nonIntervalDate = new DateOnly(2026, 1, 5); // 4 days after start

        var result = _service.CheckInterval(repeatEvent, nonIntervalDate);

        Assert.That(result, Is.False);
    }

    [Test]
    public void CheckInterval_WhenIntervalDaysIsNull_ReturnsFalse()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.Interval,
            StartDate = new DateOnly(2026, 1, 1),
            IntervalDays = null
        };
        var date = new DateOnly(2026, 1, 8);

        var result = _service.CheckInterval(repeatEvent, date);

        Assert.That(result, Is.False);
    }

    [Test]
    public void CheckInterval_WhenStartDateIsNull_ReturnsFalse()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.Interval,
            StartDate = null,
            IntervalDays = 7
        };
        var date = new DateOnly(2026, 1, 8);

        var result = _service.CheckInterval(repeatEvent, date);

        Assert.That(result, Is.False);
    }

    #endregion

    #region CheckDate Tests

    [Test]
    public void CheckDate_WhenMonthAndDayMatch_ReturnsTrue()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.Date,
            Month = 7,
            Day = 4
        };
        var matchingDate = new DateOnly(2026, 7, 4);

        var result = _service.CheckDate(repeatEvent, matchingDate);

        Assert.That(result, Is.True);
    }

    [Test]
    public void CheckDate_WhenMonthDoesNotMatch_ReturnsFalse()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.Date,
            Month = 7,
            Day = 4
        };
        var differentMonth = new DateOnly(2026, 8, 4);

        var result = _service.CheckDate(repeatEvent, differentMonth);

        Assert.That(result, Is.False);
    }

    [Test]
    public void CheckDate_WhenDayDoesNotMatch_ReturnsFalse()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.Date,
            Month = 7,
            Day = 4
        };
        var differentDay = new DateOnly(2026, 7, 5);

        var result = _service.CheckDate(repeatEvent, differentDay);

        Assert.That(result, Is.False);
    }

    [Test]
    public void CheckDate_WhenMonthIsNull_ReturnsFalse()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.Date,
            Month = null,
            Day = 4
        };
        var date = new DateOnly(2026, 7, 4);

        var result = _service.CheckDate(repeatEvent, date);

        Assert.That(result, Is.False);
    }

    [Test]
    public void CheckDate_WhenDayIsNull_ReturnsFalse()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.Date,
            Month = 7,
            Day = null
        };
        var date = new DateOnly(2026, 7, 4);

        var result = _service.CheckDate(repeatEvent, date);

        Assert.That(result, Is.False);
    }

    [Test]
    public void CheckDate_MatchesAcrossYears()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Birthday",
            RepeatType = RepeatType.Date,
            Month = 12,
            Day = 25
        };

        Assert.Multiple(() =>
        {
            Assert.That(_service.CheckDate(repeatEvent, new DateOnly(2026, 12, 25)), Is.True);
            Assert.That(_service.CheckDate(repeatEvent, new DateOnly(2027, 12, 25)), Is.True);
            Assert.That(_service.CheckDate(repeatEvent, new DateOnly(2028, 12, 25)), Is.True);
        });
    }

    #endregion

    #region GetWeekOfMonth Tests

    [Test]
    [TestCase(1, 1)]
    [TestCase(7, 1)]
    [TestCase(8, 2)]
    [TestCase(14, 2)]
    [TestCase(15, 3)]
    [TestCase(21, 3)]
    [TestCase(22, 4)]
    [TestCase(28, 4)]
    [TestCase(29, 5)]
    [TestCase(30, 5)]
    [TestCase(31, 5)]
    public void GetWeekOfMonth_ReturnsCorrectWeek(int dayOfMonth, int expectedWeek)
    {
        var date = new DateOnly(2026, 1, dayOfMonth);

        var result = _service.GetWeekOfMonth(date);

        Assert.That(result, Is.EqualTo(expectedWeek));
    }

    #endregion

    #region GetEventsForDate Tests

    [Test]
    public async Task GetEventsForDate_ReturnsWaitingEventsThatArePastDue()
    {
        var context = TestDbContextFactory.Create();
        var service = new CalendarService(context);

        var pastDueEvent = new WaitingEvent
        {
            Name = "Past Due",
            OccurrenceDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
            Layer = Layer.Red
        };
        var futureEvent = new WaitingEvent
        {
            Name = "Future",
            OccurrenceDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7))
        };
        context.WaitingEvents.AddRange(pastDueEvent, futureEvent);
        await context.SaveChangesAsync();

        var result = await service.GetEventsForDate(DateOnly.FromDateTime(DateTime.Today));

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Past Due"));
        Assert.That(result[0].EventType, Is.EqualTo("WaitingEvent"));
        Assert.That(result[0].Layer, Is.EqualTo(Layer.Red));
    }

    [Test]
    public async Task GetEventsForDate_ReturnsMatchingRepeatingEvents()
    {
        var context = TestDbContextFactory.Create();
        var service = new CalendarService(context);

        // Add a repeating event that occurs every Monday
        var mondayEvent = new RepeatingEvent
        {
            Name = "Monday Meeting",
            RepeatType = RepeatType.DayOfWeek,
            DayOfWeek = 1, // Monday
            Layer = Layer.Red
        };
        context.RepeatingEvents.Add(mondayEvent);
        await context.SaveChangesAsync();

        var monday = new DateOnly(2026, 1, 26); // A Monday
        var result = await service.GetEventsForDate(monday);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Monday Meeting"));
        Assert.That(result[0].EventType, Is.EqualTo("RepeatingEvent"));
        Assert.That(result[0].Layer, Is.EqualTo(Layer.Red));
    }

    [Test]
    public async Task GetEventsForDate_DoesNotReturnMessagesOfTheDay()
    {
        var context = TestDbContextFactory.Create();
        var service = new CalendarService(context);

        var motd = new MessageOfTheDay
        {
            Message = "Birthday",
            Month = 1,
            Day = 24
        };
        context.MessagesOfTheDay.Add(motd);
        await context.SaveChangesAsync();

        var result = await service.GetEventsForDate(new DateOnly(2026, 1, 24));

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetEventsForDate_ReturnsEmptyListWhenNoMatches()
    {
        var context = TestDbContextFactory.Create();
        var service = new CalendarService(context);

        var result = await service.GetEventsForDate(new DateOnly(2026, 1, 24));

        Assert.That(result, Is.Empty);
    }

    #endregion

    #region GetMotdForDate Tests

    [Test]
    public async Task GetMotdForDate_ReturnsMatchingMessages()
    {
        var context = TestDbContextFactory.Create();
        var service = new CalendarService(context);

        var birthday = new MessageOfTheDay
        {
            Message = "Birthday",
            Month = 7,
            Day = 4,
            Layer = Layer.Red
        };
        var otherMessage = new MessageOfTheDay
        {
            Message = "Other",
            Month = 8,
            Day = 15
        };
        context.MessagesOfTheDay.AddRange(birthday, otherMessage);
        await context.SaveChangesAsync();

        var result = await service.GetMotdForDate(new DateOnly(2026, 7, 4));

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.Select(e => e.Name), Is.EquivalentTo(new[] { "Birthday" }));
        Assert.That(result.All(e => e.EventType == "MessageOfTheDay"), Is.True);
        Assert.That(result[0].Layer, Is.EqualTo(Layer.Red));
    }

    [Test]
    public async Task GetMotdForDate_ReturnsEmptyListWhenNoMatches()
    {
        var context = TestDbContextFactory.Create();
        var service = new CalendarService(context);

        var motd = new MessageOfTheDay
        {
            Message = "Birthday",
            Month = 7,
            Day = 4
        };
        context.MessagesOfTheDay.Add(motd);
        await context.SaveChangesAsync();

        var result = await service.GetMotdForDate(new DateOnly(2026, 8, 15));

        Assert.That(result, Is.Empty);
    }

    #endregion

    #region DoesRepeatOnDate Tests

    [Test]
    public void DoesRepeatOnDate_DispatchesToCorrectMethod_DayOfWeek()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.DayOfWeek,
            DayOfWeek = 1 // Monday
        };
        var monday = new DateOnly(2026, 1, 26);

        var result = _service.DoesRepeatOnDate(repeatEvent, monday);

        Assert.That(result, Is.True);
    }

    [Test]
    public void DoesRepeatOnDate_DispatchesToCorrectMethod_DayOfWeekOfMonth()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.DayOfWeekOfMonth,
            DayOfWeek = 1,
            WeeksOfMonth = "1"
        };
        var firstMonday = new DateOnly(2026, 1, 5);

        var result = _service.DoesRepeatOnDate(repeatEvent, firstMonday);

        Assert.That(result, Is.True);
    }

    [Test]
    public void DoesRepeatOnDate_DispatchesToCorrectMethod_Interval()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.Interval,
            StartDate = new DateOnly(2026, 1, 1),
            IntervalDays = 7
        };
        var intervalDate = new DateOnly(2026, 1, 8);

        var result = _service.DoesRepeatOnDate(repeatEvent, intervalDate);

        Assert.That(result, Is.True);
    }

    [Test]
    public void DoesRepeatOnDate_DispatchesToCorrectMethod_Date()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = RepeatType.Date,
            Month = 7,
            Day = 4
        };
        var matchingDate = new DateOnly(2026, 7, 4);

        var result = _service.DoesRepeatOnDate(repeatEvent, matchingDate);

        Assert.That(result, Is.True);
    }

    [Test]
    public void DoesRepeatOnDate_UnknownRepeatType_ReturnsFalse()
    {
        var repeatEvent = new RepeatingEvent
        {
            Name = "Test",
            RepeatType = (RepeatType)999 // Invalid type
        };
        var date = new DateOnly(2026, 1, 24);

        var result = _service.DoesRepeatOnDate(repeatEvent, date);

        Assert.That(result, Is.False);
    }

    #endregion
}
