using System;

namespace Logic.Entities
{
    public class Email : ValueObject<Email>
    {
        public string Value { get; }
        public Email(string value)
        {
            Value = value;
        }

        protected override bool EqualsCore(Email other)
        {
            return Value.Equals(other.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        protected override int GetHashCodeCore()
        {
            return Value.GetHashCode();
        }
    }
}
