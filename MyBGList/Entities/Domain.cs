namespace MyBGList.Entities
{
    [Table("Domains")]
    public class Domain
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public DateTime LastModifiedDate { get; set; }

        //Navigation Property
        public ICollection<BoardGames_Domains>? BoardGames_Domains { get; set; }



        

    }
}
