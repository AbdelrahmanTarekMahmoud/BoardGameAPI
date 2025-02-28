namespace MyBGList.DTO
{
    public class LoginDTO
    {
        [Required]
        [MaxLength(255)]
        public string? UserName { get; set; } = default;
        [Required]
        public string? Password { get; set; } = default;
    }
}
