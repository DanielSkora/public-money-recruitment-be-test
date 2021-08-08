namespace VacationRental.Api.Tests.Unit.Builders
{
    using Models;

    public class RentalBuilder
    {
        private readonly RentalViewModel _rental;

        public RentalBuilder()
        {
            _rental = new RentalViewModel();
        }

        public RentalBuilder WithId(int id)
        {
            _rental.Id = id;
            return this;
        }

        public RentalBuilder WithUnit(int unit)
        {
            _rental.Units = unit;
            return this;
        }

        public RentalBuilder WithPreparationTimeInDays(int preparationTimeInDays)
        {
            _rental.PreparationTimeInDays = preparationTimeInDays;
            return this;
        }

        public RentalViewModel Build()
        {
            return _rental;
        }
    }
}