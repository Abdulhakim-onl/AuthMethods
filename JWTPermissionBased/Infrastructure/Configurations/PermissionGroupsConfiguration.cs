using JWTPermissionBased.Application.Common.Enums.AuthEnums;
using JWTPermissionBased.Domain.AuthEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JWTPermissionBased.Infrastructure.Configurations;

public class PermissionGroupsConfiguration: IEntityTypeConfiguration<PermissionGroup>
{
    public void Configure(EntityTypeBuilder<PermissionGroup> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(c => c.IsActive == true);

        builder.HasData(new PermissionGroup[]
        {
            new()
            {
                Id = PermissionGroupsEnum.Weather.Key,
                Name = nameof(PermissionGroupsEnum.Weather),
                Description = PermissionGroupsEnum.Weather.Value,
                IsActive = true
            }
        });
    }
}