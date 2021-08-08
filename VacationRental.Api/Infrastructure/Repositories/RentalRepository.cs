namespace VacationRental.Api.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using Interfaces;
    using Models;

    public class RentalRepository : IRentalRepository
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;

        public RentalRepository(IDictionary<int, RentalViewModel> rentals)
        {
            _rentals = rentals;
        }

        public RentalViewModel GetRentalById(int rentalId)
        {
            if (!_rentals.ContainsKey(rentalId))
                throw new ApplicationException("Rental not found");

            return _rentals[rentalId];
        }

        public ResourceIdViewModel CreateRental(RentalBindingModel model)
        {
            var key = new ResourceIdViewModel { Id = _rentals.Keys.Count + 1 };

            _rentals.Add(key.Id, new RentalViewModel
            {
                Id = key.Id,
                Units = model.Units,
                PreparationTimeInDays = model.PreparationTimeInDays
            });

            return key;
        }

        public RentalViewModel UpdateRental(RentalUpdateModel model)
        {
            _rentals[model.Id].Units = model.Units;
            _rentals[model.Id].PreparationTimeInDays = model.PreparationTimeInDays;

            return _rentals[model.Id];
        }
    }
}