namespace MyBGList.Entities
{
    //Junction Table
    //No need for table name "Convention is good"
    public class BoardGames_Domains
    {
        [Key]
        [Required]
        public int BoardGameId { get; set; }
        [Key]
        [Required]
        public int DomainId { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }

        //Navigation Property
        public BoardGame? BoardGame { get; set; }
        public Domain? Domain { get; set; }
    }
}
