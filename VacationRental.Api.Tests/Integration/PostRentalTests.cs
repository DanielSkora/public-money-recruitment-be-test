namespace VacationRental.Api.Tests.Integration
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Models;
    using Xunit;

    [Collection("Integration")]
    public class PostRentalTests : TestBase
    {
        private readonly HttpClient _client;

        public PostRentalTests(IntegrationFixture fixture) : base(fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostRental_ThenAGetReturnsTheCreatedRental()
        {
            var request = new RentalBindingModel
            {
                Units = 25,
                PreparationTimeInDays = 5
            };

            ResourceIdViewModel postResult = await CreateRental(25, 5);

            using (var getResponse = await _client.GetAsync($"/api/v1/rentals/{postResult.Id}"))
            {
                Assert.True(getResponse.IsSuccessStatusCode);

                var getResult = await getResponse.Content.ReadAsAsync<RentalViewModel>();
                Assert.Equal(request.Units, getResult.Units);
                Assert.Equal(request.PreparationTimeInDays, getResult.PreparationTimeInDays);
            }
        }

        [Fact]
        public async Task GivenWrongUnitsRequest_WhenPostRental_ThenThrowsApplicationException()
        {
            var request = new RentalBindingModel
            {
                Units = -25,
                PreparationTimeInDays = 5
            };

            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                using (await _client.PostAsJsonAsync($"/api/v1/rentals", request))
                {
                }
            });
        }

        [Fact]
        public async Task GivenWrongPreparationTimeInDaysRequest_WhenPostRental_ThenThrowsApplicationException()
        {
            var request = new RentalBindingModel
            {
                Units = 25,
                PreparationTimeInDays = -5
            };

            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                using (await _client.PostAsJsonAsync($"/api/v1/rentals", request))
                {
                }
            });
        }

        [Fact]
        public async Task GivenWrongRentalIdRequest_WhenGetRental_ThenThrowsApplicationException()
        {
            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                using (await _client.GetAsync($"/api/v1/rentals/{-1}"))
                {
                }
            });
        }

        [Fact]
        public async Task GivenWrongRentalIdRequest_WhenPutRental_ThenThrowsApplicationException()
        {
            var request = new RentalBindingModel
            {
                Units = 25,
                PreparationTimeInDays = 5
            };

            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                using (await _client.PutAsJsonAsync($"/api/v1/rentals/{-1}", request))
                {
                }
            });
        }

        [Fact]
        public async Task GivenWrongPreparationTimeInDaysRequest_WhenPutRental_ThenThrowsApplicationException()
        {
            var createRequest = new RentalBindingModel
            {
                Units = 25,
                PreparationTimeInDays = 5
            };

            ResourceIdViewModel postResult = await CreateRental(createRequest);

            var request = new RentalBindingModel
            {
                Units = 25,
                PreparationTimeInDays = -5
            };

            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                using (await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", request))
                {
                }
            });
        }

        [Fact]
        public async Task GivenWrongUnitsRequest_WhenPutRental_ThenThrowsApplicationException()
        {
            var createRequest = new RentalBindingModel
            {
                Units = 25,
                PreparationTimeInDays = 5
            };

            ResourceIdViewModel postResult = await CreateRental(createRequest);

            var updateRequest = new RentalBindingModel
            {
                Units = -5,
                PreparationTimeInDays = 15
            };

            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                using (await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", updateRequest))
                {
                }
            });
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPutRental_ThenAGetReturnsTheUpdatedRentalWithoutBookings()
        {
            var createRequest = new RentalBindingModel
            {
                Units = 25,
                PreparationTimeInDays = 5
            };

            ResourceIdViewModel postResult = await CreateRental(createRequest);

            var updateRequest = new RentalBindingModel
            {
                Units = 5,
                PreparationTimeInDays = 1
            };
            RentalViewModel putResult;
            using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", updateRequest))
            {
                Assert.True(putResponse.IsSuccessStatusCode);
                putResult = await putResponse.Content.ReadAsAsync<RentalViewModel>();
                Assert.Equal(updateRequest.Units, putResult.Units);
                Assert.Equal(updateRequest.PreparationTimeInDays, putResult.PreparationTimeInDays);
            }

            using (var getResponse = await _client.GetAsync($"/api/v1/rentals/{postResult.Id}"))
            {
                Assert.True(getResponse.IsSuccessStatusCode);

                var getResult = await getResponse.Content.ReadAsAsync<RentalViewModel>();
                Assert.Equal(updateRequest.Units, getResult.Units);
                Assert.Equal(updateRequest.PreparationTimeInDays, getResult.PreparationTimeInDays);
            }
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPutRental_ThenAGetReturnsTheUpdatedRentalWithBookings()
        {
            var createRequest = new RentalBindingModel
            {
                Units = 2,
                PreparationTimeInDays = 1
            };
            ResourceIdViewModel postResult = await CreateRental(createRequest);
            
            var postBooking1Request = new BookingBindingModel
            {
                RentalId = postResult.Id,
                Nights = 2,
                Start = new DateTime(2000, 01, 02)
            };
            await CreateBooking(postBooking1Request);

            var postBooking2Request = new BookingBindingModel
            {
                RentalId = postResult.Id,
                Nights = 2,
                Start = new DateTime(2000, 01, 03)
            };
            await CreateBooking(postBooking2Request);

            var updateRequest = new RentalBindingModel
            {
                Units = 3,
                PreparationTimeInDays = 2
            };
            using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", updateRequest))
            {
                Assert.True(putResponse.IsSuccessStatusCode);
                var putResult = await putResponse.Content.ReadAsAsync<RentalViewModel>();
                Assert.Equal(updateRequest.Units, putResult.Units);
                Assert.Equal(updateRequest.PreparationTimeInDays, putResult.PreparationTimeInDays);
            }

            using (var getResponse = await _client.GetAsync($"/api/v1/rentals/{postResult.Id}"))
            {
                Assert.True(getResponse.IsSuccessStatusCode);

                var getResult = await getResponse.Content.ReadAsAsync<RentalViewModel>();
                Assert.Equal(updateRequest.Units, getResult.Units);
                Assert.Equal(updateRequest.PreparationTimeInDays, getResult.PreparationTimeInDays);
            }
        }

        [Fact]
        public async Task GivenCompleteRequestWithOverlappingBookings_WhenPutRental_ThenThrowsApplicationException()
        {
            var createRequest = new RentalBindingModel
            {
                Units = 2,
                PreparationTimeInDays = 1
            };

            ResourceIdViewModel postResult = await CreateRental(createRequest);

            var postBooking1Request = new BookingBindingModel
            {
                RentalId = postResult.Id,
                Nights = 2,
                Start = new DateTime(2000, 01, 02)
            };
            await CreateBooking(postBooking1Request);

            var postBooking2Request = new BookingBindingModel
            {
                RentalId = postResult.Id,
                Nights = 2,
                Start = new DateTime(2000, 01, 03)
            };
            await CreateBooking(postBooking2Request);

            var request = new RentalBindingModel
            {
                Units = 2,
                PreparationTimeInDays = 2
            };
            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                using (await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", request)) ;
                {
                }
            });
        }

        [Fact]
        public async Task GivenCompleteRequestWithOverlappingBooking_WhenPutRental_ThenThrowsApplicationException()
        {
            var createRequest = new RentalBindingModel
            {
                Units = 2,
                PreparationTimeInDays = 1
            };

            ResourceIdViewModel postResult = await CreateRental(createRequest);

            var postBooking1Request = new BookingBindingModel
            {
                RentalId = postResult.Id,
                Nights = 2,
                Start = new DateTime(2000, 01, 02)
            };
            await CreateBooking(postBooking1Request);

            var postBooking2Request = new BookingBindingModel
            {
                RentalId = postResult.Id,
                Nights = 2,
                Start = new DateTime(2000, 01, 03)
            };
            await CreateBooking(postBooking2Request);

            var updateRequest = new RentalBindingModel
            {
                Units = 1,
                PreparationTimeInDays = 1
            };
            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                using (await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", updateRequest)) ;
                {
                }
            });
        }
    }
}
