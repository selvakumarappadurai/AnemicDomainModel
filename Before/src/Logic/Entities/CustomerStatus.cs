using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Entities
{
    public class CustomerStatus : ValueObject<CustomerStatus>
    {
        // This parameterless constructor is for entity mapping.
        private CustomerStatus() { }

        private CustomerStatus(CustomerStatusType type, ExpirationDate expirationDate) : this()
        {
            Type = type;
            _expirationDate = expirationDate ?? throw new ArgumentNullException(nameof(expirationDate));
        }

        public static readonly CustomerStatus Regular = new CustomerStatus(CustomerStatusType.Regular, ExpirationDate.Infinite);

        public CustomerStatusType Type { get; }

        // this private prop is to entity mappings. refer mapping folder class for reference.
        private readonly DateTime? _expirationDate;
        // get operation only. Another way instead of normal get process.
        public ExpirationDate ExpirationDate => (ExpirationDate)_expirationDate;
        //public virtual ExpirationDate ExpirationDate
        //{ 
        // get => (ExpirationDate)_expirationDate;
        //}

        // checking the status for customer type is advanced. Since this is specific to customer status keep the logic here itself.
        public bool IsAdvanced => Type == CustomerStatusType.Advanced && !ExpirationDate.IsExpired;



        protected override bool EqualsCore(CustomerStatus other)
        {
            return Type == other.Type && ExpirationDate == other.ExpirationDate;
        }

        protected override int GetHashCodeCore()
        {
            return Type.GetHashCode() ^ ExpirationDate.GetHashCode(); // ^ ==> combines the two hashcode values.
        }

        public decimal GetDiscount() => IsAdvanced ? 0.23m : 0m;

        public CustomerStatus Promote()
        {
            return new CustomerStatus(CustomerStatusType.Advanced, (ExpirationDate)DateTime.UtcNow.AddYears(1));
        }

    }

    public enum CustomerStatusType
    {
        Regular = 1,
        Advanced = 2
    }
}
