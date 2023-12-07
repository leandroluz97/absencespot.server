using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class Team
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAutoApproved { get; set; }
        public IEnumerable<BaseUser>? Users { get; set; }

        public void EnsureValidation()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ArgumentException(nameof(Name));
            }
        }
    }


}
