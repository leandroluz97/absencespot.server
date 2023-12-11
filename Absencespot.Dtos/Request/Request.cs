using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class Request
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Note { get; set; }
        public string File { get; set; }
        public int Status { get; set; }

        public int? LeaveId { get; set; }
        public Leave? Leave { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int? OnBehalfOfId { get; set; }
        public User? OnBehalfOf { get; set; }

        public int? ApproverId { get; set; }
        public User? Approver { get; set; }


        public void EnsureValidation()
        {
            if (string.IsNullOrEmpty(Note))
            {
                throw new ArgumentException(nameof(Note));
            }
        }
    }
}
