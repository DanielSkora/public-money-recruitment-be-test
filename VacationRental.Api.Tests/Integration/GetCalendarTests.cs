namespace VacationRental.Api.Tests.Integration
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Models;
    using Xunit;

    [Collection("Integration")]
    public class GetCalendarTests : TestBase
    {
        private readonly HttpClient _client;

        public GetCalendarTests(IntegrationFixture fixture) : base(fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenGetCalendar_ThenAGetReturnsTheCalculatedCalendar()
        {

            ResourceIdViewModel postRentalResult = await CreateRental(2, 1);

            ResourceIdViewModel postBooking1Result = await CreateBooking(postRentalResult.Id, 2, new DateTime(2000, 01, 02));

            ResourceIdViewModel postBooking2Result = await CreateBooking(postRentalResult.Id, 2, new DateTime(2000, 01, 03));

            using (var getCalendarResponse = await _client.GetAsync($"/api/v1/calendar?rentalId={postRentalResult.Id}&start=2000-01-01&nights=6"))
            {
                Assert.True(getCalendarResponse.IsSuccessStatusCode);

                var getCalendarResult = await getCalendarResponse.Content.ReadAsAsync<CalendarViewModel>();
                
                Assert.Equal(postRentalResult.Id, getCalendarResult.RentalId);
                Assert.Equal(6, getCalendarResult.Dates.Count);

                Assert.Equal(new DateTime(2000, 01, 01), getCalendarResult.Dates[0].Date);
                Assert.Empty(getCalendarResult.Dates[0].Bookings);
                Assert.Empty(getCalendarResult.Dates[0].PreparationTimes);

                Assert.Equal(new DateTime(2000, 01, 02), getCalendarResult.Dates[1].Date);
                Assert.Single(getCalendarResult.Dates[1].Bookings);
                Assert.Contains(getCalendarResult.Dates[1].Bookings, x => x.Id == postBooking1Result.Id && x.Unit == 1);
                Assert.Empty(getCalendarResult.Dates[1].PreparationTimes);
                
                Assert.Equal(new DateTime(2000, 01, 03), getCalendarResult.Dates[2].Date);
                Assert.Equal(2, getCalendarResult.Dates[2].Bookings.Count);
                Assert.Contains(getCalendarResult.Dates[2].Bookings, x => x.Id == postBooking1Result.Id && x.Unit == 1);
                Assert.Contains(getCalendarResult.Dates[2].Bookings, x => x.Id == postBooking2Result.Id && x.Unit == 2);
                Assert.Empty(getCalendarResult.Dates[2].PreparationTimes);

                Assert.Equal(new DateTime(2000, 01, 04), getCalendarResult.Dates[3].Date);
                Assert.Single(getCalendarResult.Dates[3].Bookings);
                Assert.Contains(getCalendarResult.Dates[3].Bookings, x => x.Id == postBooking2Result.Id && x.Unit == 2);
                Assert.Contains(getCalendarResult.Dates[3].PreparationTimes, x => x.Unit == 1);
                
                Assert.Equal(new DateTime(2000, 01, 05), getCalendarResult.Dates[4].Date);
                Assert.Empty(getCalendarResult.Dates[4].Bookings);
                Assert.Contains(getCalendarResult.Dates[4].PreparationTimes, x => x.Unit == 2);

                Assert.Equal(new DateTime(2000, 01, 06), getCalendarResult.Dates[5].Date);
                Assert.Empty(getCalendarResult.Dates[5].Bookings);
                Assert.Empty(getCalendarResult.Dates[5].PreparationTimes);
            }
        }

        [Fact]
        public async Task GivenWrongRentalRequest_WhenGetCalendar_ThenThrowsApplicationException()
        {
            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                using (await _client.GetAsync($"/api/v1/calendar?rentalId={-1}&start=2000-01-01&nights=5"))
                {
                }
            });
        }

        [Fact]
        public async Task GivenWrongNightsRequest_WhenGetCalendar_ThenThrowsApplicationException()
        {
            ResourceIdViewModel postRentalResult = await CreateRental(2, 0);

            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                using (await _client.GetAsync($"/api/v1/calendar?rentalId={postRentalResult.Id}&start=2000-01-01&nights=-5"))
                {
                }
            });
        }
    }
}
