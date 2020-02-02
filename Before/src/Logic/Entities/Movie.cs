using Newtonsoft.Json;
using System;

namespace Logic.Entities
{
    public class Movie : Entity
    {
        public virtual string Name { get; protected set; }

        public virtual LicensingModel LicensingModel { get; protected set; }

        public virtual ExpirationDate GetExpirationDate()
        {
            ExpirationDate result;

            switch (LicensingModel)
            {
                case LicensingModel.TwoDays:
                    result = (ExpirationDate)DateTime.UtcNow.AddDays(2);
                    break;

                case LicensingModel.LifeLong:
                    result = ExpirationDate.Infinite;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        public virtual Dollars CalculatePrice(CustomerStatus status)
        {
            Dollars price;
            switch (LicensingModel)
            {
                case LicensingModel.TwoDays:
                    price = Dollars.Of(4);
                    break;

                case LicensingModel.LifeLong:
                    price = Dollars.Of(8);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            //if (status == CustomerStatus.Advanced && !statusExpirationDate.IsExpired) // Moved the verification logic inside the value object.
            if (status.IsAdvanced)
            {
                price = price * 0.75m;
            }

            return price;
        }
    }
}
