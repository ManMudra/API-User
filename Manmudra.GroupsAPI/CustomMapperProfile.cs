using AutoMapper;
using Manmudra.Data.Entities;
using Manmudra.DTO.Account;
using Manmudra.DTO.Address;

namespace Manmudra.GroupsAPI
{
    public class CustomMapperProfile : Profile
    {
        public CustomMapperProfile()
        {
            CreateMap<RegisterDto, ApplicationUser>()
                .ForMember(d => d.FirstName, o => o.MapFrom(s => string.IsNullOrEmpty(s.FirstName) ? null : s.FirstName.Trim()))
                .ForMember(d => d.Surname, o => o.MapFrom(s => string.IsNullOrEmpty(s.Surname) ? null : s.Surname.Trim()))
                .ForMember(d => d.Email, o => o.MapFrom(s => string.IsNullOrEmpty(s.Email) ? null : s.Email.Trim()))
                .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => string.IsNullOrEmpty(s.MobileNo) ? null : s.MobileNo.Replace(" ", string.Empty)))
                .ForMember(d => d.UserName, o => o.MapFrom((s, d) => string.IsNullOrEmpty(d.UserName) ? Guid.NewGuid().ToString() : d.UserName))
                .ForMember(d => d.PasswordHash, o => o.Ignore());

            CreateMap<AddressDto, Address>().ForMember(d => d.Id, o => o.Ignore());
            CreateMap<Address, AddressDto>();

            CreateMap<UserDto, ApplicationUser>()
                .ForMember(d => d.FirstName, o => o.MapFrom(s => string.IsNullOrEmpty(s.FirstName) ? null : s.FirstName.Trim()))
                .ForMember(d => d.Surname, o => o.MapFrom(s => string.IsNullOrEmpty(s.Surname) ? null : s.Surname.Trim()))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Address == null || string.IsNullOrEmpty(s.Address.Email) ? null : s.Address.Email.Trim()))
                .ForMember(d => d.UserName, o => o.MapFrom((s, d) => string.IsNullOrEmpty(d.UserName) ? Guid.NewGuid().ToString() : d.UserName))
                .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => string.IsNullOrEmpty(s.MobileNo) ? null : s.MobileNo.Replace(" ", string.Empty)))
                .ForMember(d => d.AadhaarNo, o => o.MapFrom(s => string.IsNullOrEmpty(s.AadhaarNo) ? null : ConvertStringToLong(s.AadhaarNo)))
                .ForMember(d => d.LandlinePhone, o => o.MapFrom(s => string.IsNullOrEmpty(s.LandlinePhone) ? null : ConvertStringToLong(s.LandlinePhone)))
                .ForMember(d => d.DateOfBirth, o => o.MapFrom((s, d) =>
                {
                    if (s.DateOfBirth == null) return (DateTime?)null;
                    var dt = Convert.ToDateTime(s.DateOfBirth);
                    dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                    return dt;
                }))
                .ForMember(d => d.CreationDate, o => o.MapFrom((s, d) =>
                {
                    if (s.CreationDate == null) return (DateTime?)null;
                    var dt = Convert.ToDateTime(s.CreationDate);
                    dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                    return dt;
                }))
                .ForMember(d => d.LastUpdateDate, o => o.MapFrom((s, d) =>
                {
                    if (s.LastUpdateDate == null) return (DateTime?)null;
                    var dt = Convert.ToDateTime(s.LastUpdateDate);
                    dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                    return dt;
                }))
                .ForMember(d => d.Address, o => o.Ignore())
                .ForMember(d => d.PasswordHash, o => o.Ignore());

            CreateMap<ApplicationUser, UserDto>()
                .ForMember(d => d.MobileNo, o => o.MapFrom(s => s.PhoneNumber))
                .ForMember(d => d.DateOfBirth, o => o.MapFrom((s, d) => s.DateOfBirth?.ToString("F") ?? null))
                .ForMember(d => d.CreationDate, o => o.MapFrom((s, d) =>
                {
                    if (s.CreationDate == null) return (DateTime?)null;
                    var dt = Convert.ToDateTime(s.CreationDate);
                    dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                    return dt;
                }))
                .ForMember(d => d.LastUpdateDate, o => o.MapFrom((s, d) =>
                {
                    if (s.LastUpdateDate == null) return (DateTime?)null;
                    var dt = Convert.ToDateTime(s.LastUpdateDate);
                    dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                    return dt;
                }));

            CreateMap<ApplicationUser, ApplicationUser>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.PasswordHash, o => o.Ignore());
        }

        private long? ConvertStringToLong(string value)
        {
            // Add error handling or validation as needed
            if (long.TryParse(value?.Replace("-", ""), out long result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
