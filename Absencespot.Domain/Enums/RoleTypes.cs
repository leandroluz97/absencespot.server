using Absencespot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain.Enums
{
    public class RoleTypes : Enumeration
    {
        public static readonly RoleTypes OWNER = new RoleTypes(1, "OWNER");
        public static readonly RoleTypes ADMIN = new RoleTypes(2, "ADMIN");
        public static readonly RoleTypes MANAGER = new RoleTypes(3, "MANAGER");
        public static readonly RoleTypes USER = new RoleTypes(4, "USER");
        public static readonly RoleTypes ACCOUNTING = new RoleTypes(5, "ACCOUNTING");

        public RoleTypes(int id, string name) : base(id, name)
        {

        }
    }
}
