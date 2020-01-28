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
        private string _email;
        public virtual Email Email
        {
            get => (Email)_email;
            set => _email = value;
        }
        public virtual CustomerStatus Status { get; set; }

        // this private prop is to entity mappings. refer mapping folder class for reference.
        private DateTime? _statusExpirationDate;
        public virtual ExpirationDate StatusExpirationDate
        {
            get => (ExpirationDate)_statusExpirationDate;
            set => _statusExpirationDate = value;
        }

        // this private prop is to entity mappings. refer mapping folder class for reference.
        private decimal _moneySpent;
        public virtual Dollars MoneySpent
        {
            get => Dollars.Of(_moneySpent);
            set => _moneySpent = value;
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
            Status = CustomerStatus.Regular;
            StatusExpirationDate = null;
        }

        public virtual void AddPurchasedMovie(Movie movie, ExpirationDate expirationDate, Dollars price)
        {
            // There might be chance of creating another instance and add some other customer id if we have this 
            // logic outside of the domain class. So keep this here.
            var purchasedMovie = new PurchasedMovie
            {
                MovieId = movie.Id,
                CustomerId = Id, // Note - Analyse this Id generation from general entity class.
                ExpirationDate = expirationDate,
                Price = price,
                PurchaseDate = DateTime.UtcNow
            };

            _purchasedMovies.Add(purchasedMovie);
            // There might be chance of missing this line after adding purchasing movie if we use this logic outside the domain class.
            // So keep the logics related to domain class inside the domain class itself.
            MoneySpent += price;
        }
    }
}
