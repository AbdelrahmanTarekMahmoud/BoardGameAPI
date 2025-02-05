namespace MyBGList.Helpers.CustomValidators
{
    public class SortOrderValidator : ValidationAttribute
    {
        private readonly HashSet<string> AllowedOrders 
            = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "ASC", "DESC" };


        protected override ValidationResult? IsValid(object? value
            , ValidationContext validationContext) // must add validation context to return validation result not bool
        {
            var parameterOrder = value as string;
            if(!string.IsNullOrEmpty(parameterOrder)&&AllowedOrders.Contains(parameterOrder))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult($"You can only use those orders {string.Join("," , AllowedOrders)}");
            }
        }
    }
}
