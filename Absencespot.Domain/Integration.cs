﻿using Absencespot.Domain.Enums;
using Absencespot.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain
{
    public class Integration : Entity
    {
        public ProviderType Provider { get; set; }
        public bool IsActive { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
