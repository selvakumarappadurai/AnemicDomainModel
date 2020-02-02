using System;
using Newtonsoft.Json;

namespace Logic.Entities
{
    public class PurchasedMovie : Entity
    {
        // Since we are doing almost process in constructor we can make all properties setters to protected 
        // (not private since we are using ORM for mappings).


        // MovieId and Movie are meant to same (i.e) represents a movie. But this may lead to some error, like MovieId will be 5
        // and we may assign as 6 in the movie accidently sometimes.This also violates Don't Repeat Yourself (DRY) policy. 
        //public virtual long MovieId { get; set; }
        public virtual Movie Movie { get; protected set; }

        // As per business customer is realted to movie not id.
        //public virtual long CustomerId { get; set; }
        public virtual Customer Customer { get; protected set; }

        private decimal _price;
        public virtual Dollars Price
        {
            get => Dollars.Of(_price);
            protected set => _price = value;
        }

        public virtual DateTime PurchaseDate { get; protected set; } // Here we no need to change as value object as we did for expiration date.
        // So if we valid reasons we can go for value object since value object is going to be another new class creation.

        private DateTime? _expirationDate;
        public virtual ExpirationDate ExpirationDate
        {
            get => (ExpirationDate)_expirationDate;
            protected set => _expirationDate = value;
        }


        // This parameterless constructor is for ORM to instantiate this entity (here we using hibernate for ORM).
        protected PurchasedMovie()
        {

        }

        // Reason for making this constructor as internal ==> This is PurchasedMovie entity should be immutable and its belong to customer aggregate.
        // So it shouldn't be instantiated from any other clients.

        // we are going to place the logics that is required to make valid purchased movie inside the contructor. So we can protect invariant.
        internal PurchasedMovie(Movie movie, Customer customer, Dollars price, ExpirationDate expirationDate)
        {
            // This is because we created a type as Dollars(value object) instead of using decimal. So that we can use this 
            // property when we want to check for zero value without worrying about decmial conversion.
            // ==> price.IsZero

            // Here we can't have negative value for price unless business is going to provide for free under offers.
            if (price == null || price.IsZero)
                throw new ArgumentException(nameof(price));

            // Expiration date shouldn't be null or expired at the time of purchase.
            if (expirationDate == null || expirationDate.IsExpired)
                throw new ArgumentException(nameof(expirationDate));

            Movie = movie ?? throw new ArgumentNullException(nameof(movie));
            Customer = customer ?? throw new ArgumentNullException(nameof(customer));
            Price = price;
            ExpirationDate = expirationDate;
            PurchaseDate = DateTime.UtcNow;
        }
    }
}
