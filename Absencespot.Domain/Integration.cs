﻿using Absencespot.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain
{
    public class Integration : Entity
    {
        public string Platform { get; set; }
        public bool IsActive { get; set; }
    }
}
