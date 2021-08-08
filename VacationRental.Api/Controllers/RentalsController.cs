using System;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    using Infrastructure.Services.Interfaces;

    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalService _rentalService;

        public RentalsController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public RentalViewModel Get(int rentalId)
        {
            return _rentalService.GetRentalById(rentalId);
        }

        [HttpPost]
        public ResourceIdViewModel Post(RentalBindingModel model)
        {
            return _rentalService.CreateRental(model);
        }

        [HttpPut]
        [Route("{rentalId:int}")]
        public RentalViewModel Put(int rentalId, RentalBindingModel model)
        {
            return _rentalService.UpdateRental(new RentalUpdateModel
            {
                Id = rentalId,
                Units = model.Units,
                PreparationTimeInDays = model.PreparationTimeInDays
            });
        }
    }
}
