using Manmudra.Data.Base;
using Manmudra.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Manmudra.Data.Context
{
    public class ManmudraContext(DbContextOptions<ManmudraContext> options, IHttpContextAccessor httpContextAccessor) : IdentityDbContext<ApplicationUser>(options)
    {
        private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

        public DbSet<Address>? Addresses { get; set; }
        public DbSet<UserLogin>? UserLogins { get; set; }
        public DbSet<ApplicationUserRoles>? ApplicationUserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var entries = ChangeTracker.Entries<IStamp>();
            foreach (var entry in entries)
            {
                foreach (var property in entry.Entity.GetType().GetProperties())
                {
                    if (property.PropertyType == typeof(DateTime))
                    {
                        var dateTimeValue = (DateTime)property.GetValue(entry.Entity);
                        if (dateTimeValue.Kind == DateTimeKind.Unspecified)
                        {
                            property.SetValue(entry.Entity, DateTime.SpecifyKind(dateTimeValue, DateTimeKind.Utc));
                        }
                        else if (dateTimeValue.Kind == DateTimeKind.Local)
                        {
                            property.SetValue(entry.Entity, dateTimeValue.ToUniversalTime());
                        }
                    }
                    else if (property.PropertyType == typeof(DateTime?))
                    {
                        var nullableDateTimeValue = (DateTime?)property.GetValue(entry.Entity);
                        if (nullableDateTimeValue.HasValue)
                        {
                            var dtValue = nullableDateTimeValue.Value;
                            if (dtValue.Kind == DateTimeKind.Unspecified)
                            {
                                property.SetValue(entry.Entity, (DateTime?)DateTime.SpecifyKind(dtValue, DateTimeKind.Utc));
                            }
                            else if (dtValue.Kind == DateTimeKind.Local)
                            {
                                property.SetValue(entry.Entity, (DateTime?)dtValue.ToUniversalTime());
                            }
                        }
                    }
                }
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    var user = httpContext.User;
                    if (user == null)
                    {
                        if (entry.State == EntityState.Added)
                        {
                            entry.Entity.CreationDate = DateTime.UtcNow;
                        }
                        else if (entry.State == EntityState.Modified)
                        {
                            entry.Property("CreatedBy").IsModified = false;
                            entry.Property("CreationDate").IsModified = false;
                            entry.Entity.LastUpdateDate = DateTime.UtcNow;
                        }
                        return await base.SaveChangesAsync(cancellationToken);
                    }
                    var currentUserId = user.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(currentUserId))
                    {
                        if (entry.State == EntityState.Added)
                        {
                            {
                                entry.Entity.CreatedBy = currentUserId;
                                entry.Entity.CreationDate = DateTime.UtcNow;
                            }
                        }
                        else if (entry.State == EntityState.Modified)
                        {
                            entry.Property("CreatedBy").IsModified = false;
                            entry.Property("CreationDate").IsModified = false;
                            entry.Entity.LastUpdatedBy = currentUserId;
                            entry.Entity.LastUpdateDate = DateTime.UtcNow;
                        }
                    }
                    else
                    {
                        if (entry.State == EntityState.Added)
                        {
                            entry.Entity.CreationDate = DateTime.UtcNow;
                        }
                        else if (entry.State == EntityState.Modified)
                        {
                            entry.Property("CreatedBy").IsModified = false;
                            entry.Property("CreationDate").IsModified = false;
                            entry.Entity.LastUpdateDate = DateTime.UtcNow;
                        }
                    }
                }
                else
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Entity.CreationDate = DateTime.UtcNow;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        entry.Property("CreatedBy").IsModified = false;
                        entry.Property("CreationDate").IsModified = false;
                        entry.Entity.LastUpdateDate = DateTime.UtcNow;
                    }
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
