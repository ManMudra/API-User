using Manmudra.Contract.Enums;
using Manmudra.Data.Base;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manmudra.Data.Entities
{
    public class ApplicationUser : IdentityUser, IStamp
    {
        [Column(TypeName = "varchar(36)")]
        public override string Id { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? FirstName { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? Surname { get; set; }
        [Column(TypeName = "varchar(512)")]
        public string? Photo { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? FatherOrHusbandName { get; set; }
        public bool? IsMember { get; set; }

        public long? AadhaarNo { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? Organization { get; set; }

        public long? LandlinePhone { get; set; }
        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }
        public bool? IsAge { get; set; }
        [NotMapped]
        public int? Age
        {
            get
            {
                if (!DateOfBirth.HasValue) return null;
                DateTime today = DateTime.Today;
                int age = today.Year - DateOfBirth.Value.Year;
                if (DateOfBirth.Value.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
        public Gender? Gender { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
        public int? AddressId { get; set; }
        public WorkType? WorkType { get; set; }
        [Column(TypeName = "smallint")]
        public int? YearsInWork { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? WorkLocality { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? EmployerName { get; set; }
        public TransportMeans? TransportMeans { get; set; }
        public Category? Category { get; set; }
        public int? MigrantFrom { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? Caste { get; set; }
        public RationCardType? RationCardType { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string? RationCardNo { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string? PensionPayOrder { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string? JobCardNo { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string? JanAadhaarNo { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string? ShramikDiaryNo { get; set; }
        public bool PapersVerified { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string? BankAccountNo { get; set; }
        [Column(TypeName = "varchar(11)")]
        public string? Ifsc { get; set; }
        public Education? Education { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string? Skills { get; set; }
        public Handicap? Handicap { get; set; }
        [Column(TypeName = "smallint")]
        public int? EarningAdults { get; set; }
        [Column(TypeName = "smallint")]
        public int? NonEarningAdults { get; set; }
        [Column(TypeName = "smallint")]
        public int? Children { get; set; }
        public DateTime VerificationDate { get; set; } = DateTime.MinValue;
        public int? UnionId { get; set; }
        public long? IncomeId { get; set; }
        [Column(TypeName = "smallint")]
        public int WrongAttempts { get; set; } = 0;
        public DateTime BlockedTill { get; set; } = DateTime.MinValue;
        public Relationship? RelationShip { get; set; }
        [Column(TypeName = "varchar(8)")]
        public string? Language { get; set; }
        [Column(TypeName = "varchar(36)")]
        public string? CreatedBy { get; set; }
        [Column(TypeName = "varchar(36)")]
        public string? LastUpdatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public int? OldUserId { get; set; }

        [Column(TypeName = "varchar(20)")]
        public override string? PhoneNumber { get; set; }

        [Column(TypeName = "varchar(256)")]
        public override string? SecurityStamp { get; set; }

        [Column(TypeName = "varchar(256)")]
        public override string? ConcurrencyStamp { get; set; }

        public virtual Address? Address { get; set; }
    }
}
