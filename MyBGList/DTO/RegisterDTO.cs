namespace MyBGList.DTO
{
    public class RegisterDTO
    {
        [Required]
        public string UserName { get; set; } = default!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
        [Required]
        public string Password { get; set; } = default!;
    }
}
