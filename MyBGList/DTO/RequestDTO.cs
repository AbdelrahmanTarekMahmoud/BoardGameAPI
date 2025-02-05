using MyBGList.Helpers.CustomValidators;

namespace MyBGList.DTO
{
    public class RequestDTO<T> : IValidatableObject
    {

        public int pageNumber { get; set; } = 0;

        [Range(1, 100)]
        public int pageSize { get; set; } = 10;

        //[SortColumnValidator(typeof(T))]
        //we cant do this cus its evaulated in compile time while generics in run time
        public string? sortColumn { get; set; } = "Name";

        [SortOrderValidator]
        public string? sortOrder { set; get; } = "ASC";

        public string? filterQuery { set; get; } = null;

        public IEnumerable<ValidationResult> Validate( 
        ValidationContext validationContext)
        {
            var validator = new SortColumnValidator(typeof(T));
            var result = validator
            .GetValidationResult(sortColumn, validationContext);
            return (result != null)
            ? new[] { result }
            : new ValidationResult[0];
        }
    }
}
