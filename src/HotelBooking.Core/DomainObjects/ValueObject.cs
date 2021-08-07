
using FluentValidation.Results;

namespace HotelBooking.Core.DomainObjects
{
    public abstract class ValueObject
    {
        public ValidationResult ValidationResult { get; set; }

        public abstract bool IsValid();

        public override bool Equals(object obj)
        {
            var compareTo = obj as ValueObject;

            if (ReferenceEquals(this, compareTo)) return true;
            if (ReferenceEquals(null, compareTo)) return false;

            return (this == compareTo);
        }

        public static bool operator ==(ValueObject a, ValueObject b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(ValueObject a, ValueObject b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            decimal hashCode = this.GetHashCode();
            return (int)hashCode;
        }

    }
}
