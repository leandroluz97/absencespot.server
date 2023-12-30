using Absencespot.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class Integration
    {
        public ProviderType Provider { get; set; }
        public bool IsActive { get; set; }

    }
}
