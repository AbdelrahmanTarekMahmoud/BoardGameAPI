using System.Linq;

namespace MyBGList.Helpers.CustomValidators
{
    public class SortColumnValidator : ValidationAttribute
    {
        private  Type EntityType { get; set; }
        
        public SortColumnValidator(Type EntityType) : base()
        {
            this.EntityType = EntityType;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var parameterName = value as string;

            if(!string.IsNullOrEmpty(parameterName) 
                && EntityType.GetProperties().Any(s => s.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase))
)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult($"The column which is need to be sorted must be in those GameBoard properties " +
                $"{string.Join("," , EntityType.GetProperties().Select(x=>x.Name))}");
        }
    }
}
