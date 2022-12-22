using JWTPermissionBased.Application.Common.Enums.AuthEnums;
using JWTPermissionBased.Domain.AuthEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JWTPermissionBased.Infrastructure.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.PermissionGroup).WithMany(x => x.Permissions).HasForeignKey(x => x.PermissionGroupsID);
        builder.HasQueryFilter(c => c.IsActive == true);
            
        builder.HasData(new Permission[]
        {
            new()
            {
                Id = PermissionEnum.GetWeather.Key,
                Name =nameof(PermissionEnum.GetWeather),
                Description = PermissionEnum.GetWeather.Value,
                PermissionGroupsID = PermissionGroupsEnum.Weather.Key,
                IsActive = true
            }
        });
    }
}