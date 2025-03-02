﻿namespace MyBGList.Entities
{
    //Database table name
    [Table("BoardGames")]
    public class BoardGame
    {
        //Database table’s primary key
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;
        [Required]
        public int Year { get; set; }
        [Required]
        public int MinPlayers { get; set; }
        [Required]
        public int MaxPlayers { get; set; }
        [Required]
        public int PlayTime { get; set; }
        [Required]
        public int MinAge { get; set; }
        [Required]
        public int UsersRated { get; set; }
        [Required]
        [Precision(4, 2)]
        public decimal RatingAverage { get; set; }
        [Required]
        public int BGGRank { get; set; }
        [Required]
        [Precision(4, 2)]
        public decimal ComplexityAverage { get; set; }
        [Required]
        public int OwnedUsers { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public DateTime LastModifiedDate { get; set; }



        //Navigation Property
        public ICollection<BoardGames_Domains>? BoardGames_Domains { get; set; }
        public ICollection<BoardGames_Mechanics>? BoardGames_Mechanics { get; set; }

    }
}
