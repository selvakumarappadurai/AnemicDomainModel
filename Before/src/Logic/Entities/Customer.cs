using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Logic.Entities
{
    public class Customer : Entity
    {
        // this private prop is to entity mappings. refer mapping folder class for reference.
        private string _name;
        public virtual CustomerName Name
        {
            get => (CustomerName)_name;
            set => _name = value;
        }

        // this private prop is to entity mappings. refer mapping folder class for reference.
        //private string _email;
        //public virtual Email Email
        //{
        //    get => (Email)_email;
        //    protected set => _email = value;
        //}

        // Since we are not going to change the email once created we can make that as getter alone and backend property as readonly.
        // Note : We will be setting the email while creation via outfacing contract model.
        private readonly string _email;
        public virtual Email Email => (Email)_email;


        public virtual CustomerStatus Status { get; protected set; }

        // this private prop is to entity mappings. refer mapping folder class for reference.
        private decimal _moneySpent;
        public virtual Dollars MoneySpent
        {
            get => Dollars.Of(_moneySpent);
            protected set => _moneySpent = value;
        }

        // this private prop is to entity mappings. refer mapping folder class for reference.
        private readonly IList<PurchasedMovie> _purchasedMovies;
        // By making to IReadonlyList and removing setter we can protect mutating list somewhere outside the domain.
        public virtual IReadOnlyList<PurchasedMovie> PurchasedMovies => _purchasedMovies.ToList();


        // Since ORM requires an parameterless constructor for mapping we are creating this, because we
        // can't pass the parameter to parameterized constructor while mapping. 
        // Protected will make sure the encapsulation - client can't use this constructor.
        protected Customer()
        {
            _purchasedMovies = new List<PurchasedMovie>();
        }

        // This makes sure that we are not violating anything and encapsulation(protecting the invariant).
        // like we can't create or instantiate this class without passing name and email, since it is 
        // mandotory for creating user as per business. And also prevents for interchaning email and 
        // name values. Since this is the only public construtor available to instantiate the class.

        // this() make a call to above parameterless constructor. 
        // This is to avoid avoid duplication the code in this parameterized constructor too.
        public Customer(CustomerName name, Email email) : this()
        {
            _name = name ?? throw new ArgumentException(nameof(name));
            _email = email ?? throw new ArgumentException(nameof(email));

            MoneySpent = Dollars.Of(0);
            Status = CustomerStatus.Regular; // Moved staus assigning logic to object value class.
            //StatusExpirationDate = null; // Moved expiration date logic to object value class.
        }

        public virtual bool HasPurchasedMovie(Movie movie)
        {
            return PurchasedMovies.Any(x => x.Movie == movie && !x.ExpirationDate.IsExpired);
        }

        // Changed the method name from addpurchasedmovie to purchase movie as per ebiquity language.
        public virtual void PurchaseMovie(Movie movie)
        {

            if (HasPurchasedMovie(movie))
                throw new Exception();

            // There might be chance of creating another instance and add some other customer id if we have this 
            // logic outside of the domain class. So keep this here.
            //var purchasedMovie = new PurchasedMovie
            //{
            //    Movie = movie,
            //    Customer = this, // Note - Analyse this Id generation from general entity class.
            //    ExpirationDate = expirationDate,
            //    Price = price,
            //    PurchaseDate = DateTime.UtcNow
            //};

            ExpirationDate expirationDate = movie.GetExpirationDate();
            Dollars price = movie.CalculatePrice(Status);

            // Further implenting DDD for purchased movie, drilled down to below line from above.
            var purchasedMovie = new PurchasedMovie(movie, this, price, expirationDate);

            _purchasedMovies.Add(purchasedMovie);
            // There might be chance of missing this line after adding purchasing movie if we use this logic outside the domain class.
            // So keep the logics related to domain class inside the domain class itself.
            MoneySpent += price;
        }

        //Command-query separation principle (CQS) - States that a method should either return value or mutate somthing but not both.

        // Create this method as to adhere with CQS principle
        public virtual Result CanPromote()
        {
            if (Status.IsAdvanced)
                return Result.Fail("The customer already has the advanced status");

            if (PurchasedMovies.Count(x =>
            x.ExpirationDate == ExpirationDate.Infinite || x.ExpirationDate.Date >= DateTime.UtcNow.AddDays(-30)) < 2)
                return Result.Fail("The customer has to have at least 2 active movies during the last 30 days");

            if (PurchasedMovies.Where(x => x.PurchaseDate > DateTime.UtcNow.AddYears(-1)).Sum(x => x.Price) < 100m)
                return Result.Fail("The customer has to have at least 100 dollars spent during the last year");

            return Result.Ok();
        }

        // virtual is for ORM.
        public virtual void Promote()
        {
            if (CanPromote().IsFailure)
                throw new Exception();

            Status = Status.Promote();
        }
    }
}
