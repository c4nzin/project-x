using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace src.Features.Auth.Token;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Token).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.ExpiresOnUtc).IsRequired();
        builder.Property(x => x.Token).HasMaxLength(300);
    }
}
