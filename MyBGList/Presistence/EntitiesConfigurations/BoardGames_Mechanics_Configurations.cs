using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyBGList.Presistence.EntitiesConfigurations
{
    public class BoardGames_Mechanics_Configurations : IEntityTypeConfiguration<BoardGames_Mechanics>
    {
        public void Configure(EntityTypeBuilder<BoardGames_Mechanics> builder)
        {
            builder.HasKey(x => new { x.BoardGameId, x.MechanicId });

            //One To Many Relation with both ("BoardGame" & "Mechanic);
            builder.HasOne(x => x.BoardGame)
                .WithMany(x => x.BoardGames_Mechanics)
                .HasForeignKey(x => x.BoardGameId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Mechanic)
                .WithMany(x => x.BoardGames_Mechanics)
                .HasForeignKey(x => x.MechanicId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
