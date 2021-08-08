namespace VacationRental.Api.Infrastructure.Services.Interfaces
{
    using System;
    using Models;

    public interface ICalendarService
    {
        CalendarViewModel GetCalendar(int rentalId, DateTime start, int nights);
    }
}