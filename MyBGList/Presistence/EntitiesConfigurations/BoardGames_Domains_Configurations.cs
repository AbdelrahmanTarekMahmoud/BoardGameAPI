using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBGList.Entities;

namespace MyBGList.Presistence.EntitiesConfigurations
{
    public class BoardGames_Domains_Configurations : IEntityTypeConfiguration<BoardGames_Domains>
    {
        public void Configure(EntityTypeBuilder<BoardGames_Domains> builder)
        {
            //Composite Key for the Junction Table (Many to Many)
            builder.HasKey(x => new { x.BoardGameId, x.DomainId });

            //One To Many Relation with both ("BoardGame" & "Domain);
            builder.HasOne(x => x.BoardGame)
                   .WithMany(x => x.BoardGames_Domains)
                   .HasForeignKey(x => x.BoardGameId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Domain)
                   .WithMany(x => x.BoardGames_Domains)
                   .HasForeignKey(x => x.DomainId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
