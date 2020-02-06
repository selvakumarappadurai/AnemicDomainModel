using FluentNHibernate.Mapping;
using System;

namespace Logic.Customers
{
    public class CustomerMap : ClassMap<Customer>
    {
        public CustomerMap()
        {
            Id(x => x.Id);

            Map(x => x.Name).CustomType<string>().Access.CamelCaseField(Prefix.Underscore);
            Map(x => x.Email).CustomType<string>().Access.CamelCaseField(Prefix.Underscore);

            // we don't have these two properties now, instead we have them in single value object class.            
            //Map(x => x.Status).CustomType<int>();
            //Map(x => x.StatusExpirationDate).CustomType<DateTime?>().Access.CamelCaseField(Prefix.Underscore).Nullable();

            Component(x => x.Status, y =>
            {
                y.Map(x => x.Type, "Status").CustomType<int>();
                y.Map(x => x.ExpirationDate, "StatusExpirationDate").CustomType<DateTime?>()
                     .Access.CamelCaseField(Prefix.Underscore)
                     .Nullable();
            });

            Map(x => x.MoneySpent).CustomType<decimal>().Access.CamelCaseField(Prefix.Underscore);

            HasMany(x => x.PurchasedMovies).Access.CamelCaseField(Prefix.Underscore);
        }
    }
}
