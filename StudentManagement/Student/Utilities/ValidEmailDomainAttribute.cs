using System.ComponentModel.DataAnnotations;

namespace Student.Utilities
{
    public class ValidEmailDomainAttribute : ValidationAttribute
    {
        private readonly string _allowedDomain;

        public ValidEmailDomainAttribute( string allowedDomain )
        {
            _allowedDomain = allowedDomain;
        }
        public override bool IsValid( object value )
        {
            string[] strings = value.ToString()?.Split( '@' );
            return strings != null && strings[1].ToUpper() == _allowedDomain.ToUpper();
        }
    }
}
