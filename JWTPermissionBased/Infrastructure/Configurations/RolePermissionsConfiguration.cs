using JWTPermissionBased.Application.Common.Enums.AuthEnums;
using JWTPermissionBased.Domain.AuthEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JWTPermissionBased.Infrastructure.Configurations;

public class RolePermissionsConfiguration: IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasOne(x => x.Role).WithMany(x => x.RolePermissionsList).HasForeignKey(x => x.RoleId);
        builder.HasOne(x => x.Permission).WithMany(x => x.RolePermissionsList).HasForeignKey(x => x.PermissionId);
        builder.HasQueryFilter(c => c.IsActive == true);

        builder.HasData(new RolePermission[]
        {
            new()
            {
                RoleId = RoleEnum.Client.Key,
                PermissionId = PermissionEnum.GetWeather.Key,
                IsActive = true
            }
        });
    }
}