using Absencespot.Utils;

namespace Absencespot.Domain.Enums
{
    //public class StatusType : Enumeration
    //{
    //    public static readonly StatusType APPROVED = new StatusType(1, "APPROVED");
    //    public static readonly StatusType PENDING = new StatusType(2, "PENDING");
    //    public static readonly StatusType REJECTED = new StatusType(3, "REJECTED");

    //    public StatusType(int id, string name) : base(id, name)
    //    {

    //    }
    //}

    public enum StatusType
    {
        Approved = 1,
        Pending = 2,
        Rejected = 3,
    }
}
