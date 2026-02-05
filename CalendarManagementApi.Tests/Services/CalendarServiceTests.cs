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

    #region GetBirthdaysForDate Tests

    [Test]
    public async Task GetBirthdaysForDate_ReturnsBirthdaysWithin14DayWindow()
    {
        var context = TestDbContextFactory.Create();
        var service = new CalendarService(context);

        // Date: Feb 5, 2026 - window is Feb 5-19
        var birthday1 = new Birthday { Name = "Alice", Month = 2, Day = 5 };   // Day 0
        var birthday2 = new Birthday { Name = "Bob", Month = 2, Day = 10 };    // Day 5
        var birthday3 = new Birthday { Name = "Charlie", Month = 2, Day = 19 }; // Day 14
        var birthday4 = new Birthday { Name = "David", Month = 2, Day = 20 };   // Day 15 - outside window
        context.Birthdays.AddRange(birthday1, birthday2, birthday3, birthday4);
        await context.SaveChangesAsync();

        var result = await service.GetBirthdaysForDate(new DateOnly(2026, 2, 5));

        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result.Select(b => b.Name), Is.EquivalentTo(new[] { "Alice", "Bob", "Charlie" }));
    }

    [Test]
    public async Task GetBirthdaysForDate_ReturnsEmptyListWhenNoBirthdaysInRange()
    {
        var context = TestDbContextFactory.Create();
        var service = new CalendarService(context);

        context.Birthdays.Add(new Birthday { Name = "Alice", Month = 6, Day = 15 });
        await context.SaveChangesAsync();

        var result = await service.GetBirthdaysForDate(new DateOnly(2026, 2, 5));

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetBirthdaysForDate_CorrectlyCalculatesDaysAway()
    {
        var context = TestDbContextFactory.Create();
        var service = new CalendarService(context);

        var birthday1 = new Birthday { Name = "Same Day", Month = 2, Day = 5 };
        var birthday2 = new Birthday { Name = "Tomorrow", Month = 2, Day = 6 };
        var birthday3 = new Birthday { Name = "In 14 Days", Month = 2, Day = 19 };
        context.Birthdays.AddRange(birthday1, birthday2, birthday3);
        await context.SaveChangesAsync();

        var result = await service.GetBirthdaysForDate(new DateOnly(2026, 2, 5));

        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result.First(b => b.Name == "Same Day").DaysAway, Is.EqualTo(0));
        Assert.That(result.First(b => b.Name == "Tomorrow").DaysAway, Is.EqualTo(1));
        Assert.That(result.First(b => b.Name == "In 14 Days").DaysAway, Is.EqualTo(14));
    }

    [Test]
    public async Task GetBirthdaysForDate_HandlesYearBoundary()
    {
        var context = TestDbContextFactory.Create();
        var service = new CalendarService(context);

        // Date: Dec 28, 2026 - window includes Dec 28-31 and Jan 1-11
        var birthday1 = new Birthday { Name = "Dec 28", Month = 12, Day = 28 }; // Day 0
        var birthday2 = new Birthday { Name = "Dec 31", Month = 12, Day = 31 }; // Day 3
        var birthday3 = new Birthday { Name = "Jan 1", Month = 1, Day = 1 };    // Day 4
        var birthday4 = new Birthday { Name = "Jan 5", Month = 1, Day = 5 };    // Day 8
        var birthday5 = new Birthday { Name = "Jan 11", Month = 1, Day = 11 };  // Day 14
        var birthday6 = new Birthday { Name = "Jan 12", Month = 1, Day = 12 };  // Day 15 - outside window
        context.Birthdays.AddRange(birthday1, birthday2, birthday3, birthday4, birthday5, birthday6);
        await context.SaveChangesAsync();

        var result = await service.GetBirthdaysForDate(new DateOnly(2026, 12, 28));

        Assert.That(result, Has.Count.EqualTo(5));
        Assert.That(result.Select(b => b.Name), Is.EquivalentTo(new[] { "Dec 28", "Dec 31", "Jan 1", "Jan 5", "Jan 11" }));
    }

    [Test]
    public async Task GetBirthdaysForDate_CalculatesDaysAwayCorrectlyAcrossYearBoundary()
    {
        var context = TestDbContextFactory.Create();
        var service = new CalendarService(context);

        var birthday = new Birthday { Name = "Jan 5", Month = 1, Day = 5 };
        context.Birthdays.Add(birthday);
        await context.SaveChangesAsync();

        var result = await service.GetBirthdaysForDate(new DateOnly(2026, 12, 28));

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].DaysAway, Is.EqualTo(8)); // Dec 28 to Jan 5 = 8 days
    }

    [Test]
    public async Task GetBirthdaysForDate_ReturnsResultsOrderedByDaysAway()
    {
        var context = TestDbContextFactory.Create();
        var service = new CalendarService(context);

        // Add in reverse order
        var birthday1 = new Birthday { Name = "Last", Month = 2, Day = 15 };
        var birthday2 = new Birthday { Name = "Middle", Month = 2, Day = 10 };
        var birthday3 = new Birthday { Name = "First", Month = 2, Day = 5 };
        context.Birthdays.AddRange(birthday1, birthday2, birthday3);
        await context.SaveChangesAsync();

        var result = await service.GetBirthdaysForDate(new DateOnly(2026, 2, 5));

        Assert.That(result[0].Name, Is.EqualTo("First"));
        Assert.That(result[1].Name, Is.EqualTo("Middle"));
        Assert.That(result[2].Name, Is.EqualTo("Last"));
    }

    [Test]
    public async Task GetBirthdaysForDate_ExcludesBirthdaysOutsideWindow()
    {
        var context = TestDbContextFactory.Create();
        var service = new CalendarService(context);

        var insideWindow = new Birthday { Name = "Inside", Month = 2, Day = 10 };
        var outsideWindow = new Birthday { Name = "Outside", Month = 3, Day = 1 };
        context.Birthdays.AddRange(insideWindow, outsideWindow);
        await context.SaveChangesAsync();

        var result = await service.GetBirthdaysForDate(new DateOnly(2026, 2, 5));

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Inside"));
    }

    [Test]
    public async Task GetBirthdaysForDate_ReturnsCorrectDtoFields()
    {
        var context = TestDbContextFactory.Create();
        var service = new CalendarService(context);

        var birthday = new Birthday { Name = "Alice", Month = 2, Day = 10 };
        context.Birthdays.Add(birthday);
        await context.SaveChangesAsync();

        var result = await service.GetBirthdaysForDate(new DateOnly(2026, 2, 5));

        Assert.That(result, Has.Count.EqualTo(1));
        var dto = result[0];
        Assert.That(dto.Id, Is.EqualTo(birthday.Id));
        Assert.That(dto.Name, Is.EqualTo("Alice"));
        Assert.That(dto.Month, Is.EqualTo(2));
        Assert.That(dto.Day, Is.EqualTo(10));
        Assert.That(dto.DaysAway, Is.EqualTo(5));
    }

    #endregion

    #region CalculateDaysAway Tests

    [Test]
    public void CalculateDaysAway_SameDay_ReturnsZero()
    {
        var result = _service.CalculateDaysAway(new DateOnly(2026, 2, 5), 2, 5);
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void CalculateDaysAway_FutureDate_ReturnsPositiveDays()
    {
        var result = _service.CalculateDaysAway(new DateOnly(2026, 2, 5), 2, 10);
        Assert.That(result, Is.EqualTo(5));
    }

    [Test]
    public void CalculateDaysAway_CrossYearBoundary_ReturnsCorrectDays()
    {
        // From Dec 28 to Jan 5 (next year) = 8 days
        var result = _service.CalculateDaysAway(new DateOnly(2026, 12, 28), 1, 5);
        Assert.That(result, Is.EqualTo(8));
    }

    [Test]
    public void CalculateDaysAway_PastDateInYear_ReturnsNextYearOccurrence()
    {
        // Birthday was Jan 1, current date is Feb 5 - next occurrence is Jan 1 next year
        var result = _service.CalculateDaysAway(new DateOnly(2026, 2, 5), 1, 1);
        // Feb 5 2026 to Jan 1 2027 = 330 days
        Assert.That(result, Is.EqualTo(330));
    }

    #endregion
}
