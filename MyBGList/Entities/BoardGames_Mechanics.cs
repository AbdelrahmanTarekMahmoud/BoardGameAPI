namespace MyBGList.Entities
{
    //Junction Table
    //No need for table name "Convention is good"
    public class BoardGames_Mechanics
    {
        [Key]
        [Required]
        public int BoardGameId { get; set; }
        [Key]
        [Required]
        public int MechanicId { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }

        //Navigation Property
        public BoardGame? BoardGame { get; set; }
        public Mechanic? Mechanic { get; set; }
    }
}
