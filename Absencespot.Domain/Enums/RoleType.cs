using Absencespot.Utils;

namespace Absencespot.Domain.Enums
{
    public class RoleType : Enumeration
    {
        public static readonly RoleType OWNER = new RoleType(1, "OWNER");
        public static readonly RoleType ADMIN = new RoleType(2, "ADMIN");
        public static readonly RoleType MANAGER = new RoleType(3, "MANAGER");
        public static readonly RoleType USER = new RoleType(4, "USER");
        public static readonly RoleType ACCOUNTING = new RoleType(5, "ACCOUNTING");

        public RoleType(int id, string name) : base(id, name)
        {

        }
    }
}
