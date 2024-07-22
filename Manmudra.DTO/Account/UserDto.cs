using Manmudra.Contract.Enums;
using Manmudra.DTO.Address;

namespace Manmudra.DTO.Account
{
    public class UserDto
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? Surname { get; set; }
        public string? Photo { get; set; }
        public string? FatherOrHusbandName { get; set; }
        public Relationship? RelationShip { get; set; }
        public bool? IsMember { get; set; }
        public string? AadhaarNo { get; set; }
        public string? Organization { get; set; }
        public string? LandlinePhone { get; set; }
        public string? MobileNo { get; set; }
        public string? DateOfBirth { get; set; }
        public int? Age { get; set; }
        public bool? IsAge { get; set; }
        public Gender? Gender { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
        public int? AddressId { get; set; }
        public AddressDto? Address { get; set; }
        public WorkType? WorkType { get; set; }
        public int? YearsInWork { get; set; }
        public string? WorkLocality { get; set; }
        public string? EmployerName { get; set; }
        public TransportMeans? TransportMeans { get; set; }
        public Category? Category { get; set; }
        public int? MigrantFrom { get; set; }
        public string? Caste { get; set; }
        public RationCardType? RationCardType { get; set; }
        public string? RationCardNo { get; set; }
        public string? PensionPayOrder { get; set; }
        public string? JobCardNo { get; set; }
        public string? JanAadhaarNo { get; set; }
        public string? ShramikDiaryNo { get; set; }
        public bool PapersVerified { get; set; }
        public string? BankAccountNo { get; set; }
        public string? Ifsc { get; set; }
        public Education? Education { get; set; }
        public string? Skills { get; set; }
        public Handicap? Handicap { get; set; }
        public int? EarningAdults { get; set; }
        public int? NonEarningAdults { get; set; }
        public int? Children { get; set; }
        public bool Seen { get; set; }
        public int? UnionId { get; set; }
        public long? IncomeId { get; set; }
        public string? Email { get; set; }
        public bool? CheckMergeConflict { get; set; }
        public bool? MakeGroupAdmin { get; set; }
        public string? Language { get; set; }
        public string? UnionName { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public ICollection<string>? Roles { get; set; }
    }
}
