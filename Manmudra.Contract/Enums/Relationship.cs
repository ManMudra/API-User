using System.ComponentModel;

namespace Manmudra.Contract.Enums
{
    public enum Relationship : byte
    {
        [Description("S/D")]
        SonDaughter = 1,

        [Description("Spouse")]
        Spouse
    }
}
