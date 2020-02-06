using Logic.Common;

namespace Logic.Customers
{
    public class Dollars : ValueObject<Dollars>
    {
        private const decimal MaxDollarAmount = 1_000_000;

        public decimal Value { get; }

        // This is because we created a type as Dollars(value object) instead of using decimal. So that we can use this 
        // property when we want to check for zero value without worrying about decmial conversion.
        public bool IsZero => Value == 0;

        private Dollars(decimal value)
        {
            Value = value;
        }

        public static Result<Dollars> Create(decimal dollarAmount)
        {
            if (dollarAmount < 0)
                return Result.Fail<Dollars>("Dollar amount cannot be negative");
            if (dollarAmount > MaxDollarAmount)
                return Result.Fail<Dollars>("Dollar amount cannot be greater than " + MaxDollarAmount);
            if (dollarAmount % 0.01m > 0)
                return Result.Fail<Dollars>("Dollar amount cannoat contain part of a penny");

            return Result.Ok(new Dollars(dollarAmount));
        }

        public static Dollars Of(decimal dollarAmount)
        {
            return Create(dollarAmount).Value;
        }

        public static Dollars operator *(Dollars dollars, decimal multiplier)
        {
            return new Dollars(dollars.Value * multiplier);
        }

        public static Dollars operator +(Dollars dollars1, Dollars dollars2)
        {
            return new Dollars(dollars1.Value + dollars2.Value);
        }

        protected override bool EqualsCore(Dollars other)
        {
            return Value == other.Value;
        }

        protected override int GetHashCodeCore()
        {
            return Value.GetHashCode();
        }

        public static implicit operator decimal(Dollars dollars)
        {
            return dollars.Value;
        }

        // Implemented this logic in of method above -- just for naming convention.
        //public static explicit operator Dollars(decimal dollars)
        //{
        //    return Create(dollars).Value;
        //}
    }
}
