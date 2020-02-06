using System;
using Logic.Common;
using Logic.Customers;

namespace Logic.Movies
{

    // Two methods inside the class Movie are dealing with licensing model for two different categories. Instead of having switch case to 
    // handle the logics, we can make use of OOPS polymorphism mechanism by creating two sub classes inheriting the Movie class.

    // Please make a visit to MoiveMap class also for subclass mappings and mapping for non public members.


    public abstract class Movie : Entity
    {
        public virtual string Name { get; protected set; }

        protected virtual LicensingModel LicensingModel { get; set; }

        //public virtual ExpirationDate GetExpirationDate()
        //{
        //    //ExpirationDate result;

        //    switch (LicensingModel)
        //    {
        //        case LicensingModel.TwoDays:
        //            return (ExpirationDate)DateTime.UtcNow.AddDays(2);

        //        case LicensingModel.LifeLong:
        //            return ExpirationDate.Infinite;

        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }
        //}

        public abstract ExpirationDate GetExpirationDate();

        public virtual Dollars CalculatePrice(CustomerStatus status)
        {
            //Dollars price;
            decimal modifier = 1 - status.GetDiscount();
            return GetBasePrice() * modifier;

            //switch (LicensingModel)
            //{
            //    case LicensingModel.TwoDays:
            //        return Dollars.Of(4) * modifier;

            //    case LicensingModel.LifeLong:
            //        return Dollars.Of(8) * modifier;

            //    default:
            //        throw new ArgumentOutOfRangeException();
            //}

            //if (status == CustomerStatus.Advanced && !statusExpirationDate.IsExpired) // Moved the verification logic inside the value object.
            //if (status.IsAdvanced)
            //{
            //    price = price * 0.75m;
            //}

            //return price;
        }

        protected abstract Dollars GetBasePrice();
    }

    public class TwoDaysMovie : Movie
    {
        protected override Dollars GetBasePrice()
        {
            return Dollars.Of(4);
        }

        public override ExpirationDate GetExpirationDate()
        {
            return (ExpirationDate)DateTime.UtcNow.AddDays(2);
        }
    }

    public class LifeLongMovie : Movie
    {
        protected override Dollars GetBasePrice()
        {
            return Dollars.Of(8);
        }

        public override ExpirationDate GetExpirationDate()
        {
            return ExpirationDate.Infinite;
        }
    }
}
