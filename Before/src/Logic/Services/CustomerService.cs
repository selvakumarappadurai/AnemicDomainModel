using System;
using System.Linq;
using Logic.Entities;

namespace Logic.Services
{
    public class CustomerService
    {
        private readonly MovieService _movieService;

        public CustomerService(MovieService movieService)
        {
            _movieService = movieService;
        }


        // This logic is supposed to be in Movie entity, since price calculation is related to movie.
        // So moving this to movie entity.
        //private Dollars CalculatePrice(CustomerStatus status, LicensingModel licensingModel)
        //{
        //    Dollars price;
        //    switch (licensingModel)
        //    {
        //        case LicensingModel.TwoDays:
        //            price = Dollars.Of(4);
        //            break;

        //        case LicensingModel.LifeLong:
        //            price = Dollars.Of(8);
        //            break;

        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }

        //    //if (status == CustomerStatus.Advanced && !statusExpirationDate.IsExpired) // Moved the verification logic inside the value object.
        //    if (status.IsAdvanced)
        //    {
        //        price = price * 0.75m;
        //    }

        //    return price;
        //}

        public void PurchaseMovie(Customer customer, Movie movie)
        {
            //ExpirationDate expirationDate = _movieService.GetExpirationDate(movie.LicensingModel);
            ExpirationDate expirationDate = movie.GetExpirationDate();
            //Dollars price = CalculatePrice(customer.Status, movie.LicensingModel);
            Dollars price = movie.CalculatePrice(customer.Status);

            // There might be chance of creating another instance and add some other customer id if we have this 
            // logic outside of the domain class. So moved the logic inside the domain class itself.

            //var purchasedMovie = new PurchasedMovie
            //{
            //    MovieId = movie.Id,
            //    CustomerId = customer.Id,
            //    ExpirationDate = expirationDate,
            //    Price = price,
            //    PurchaseDate = DateTime.UtcNow
            //};

            // moved the adding logic to Customer doamin class itself in order to avoid mutation.
            customer.AddPurchasedMovie(movie, expirationDate, price);

            // There might be chance of missing this line after adding purchasing movie if we use this logic outside the domain class.
            // So moved the logic inside the domain class itself.

            //customer.MoneySpent += price;
        }

        // moved the adding logic to Customer doamin class itself in order to avoid mutation.
        //public bool PromoteCustomer(Customer customer)
        //{
        //    // at least 2 active movies during the last 30 days
        //    if (customer.PurchasedMovies.Count(x =>
        //        x.ExpirationDate == ExpirationDate.Infinite || x.ExpirationDate.Date >= DateTime.UtcNow.AddDays(-30)) < 2)
        //        return false;

        //    // at least 100 dollars spent during the last year
        //    if (customer.PurchasedMovies.Where(x => x.PurchaseDate > DateTime.UtcNow.AddYears(-1)).Sum(x => x.Price) < 100m)
        //        return false;

        //    // Made this as separate method inside the object value itself as per DDD.
        //    //customer.Status = CustomerStatus.Advanced;
        //    //customer.StatusExpirationDate = (ExpirationDate)DateTime.UtcNow.AddYears(1);

        //    customer.Status = customer.Status.Promote();

        //    return true;
        //}
    }
}
