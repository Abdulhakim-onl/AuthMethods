using JWTPermissionBased.Application.Common.Enums.AuthEnums;
using JWTPermissionBased.Domain.AuthEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JWTPermissionBased.Infrastructure.Configurations;

public class RoleConfiguration: IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(c => c.IsActive == true);

        builder.HasData(new Role[]
        {
            new()
            {
                Id = RoleEnum.Client.Key,
                Name = RoleEnum.Client.Value,
                IsActive = true
            },
            new Role()
            {
                Id = RoleEnum.SuperAdmin.Key,
                Name = RoleEnum.SuperAdmin.Value,
                IsActive = true
            }
        });
    }
}