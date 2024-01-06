using Absencespot.Domain.Enums;
using System.Text.Json.Serialization;
using Absencespot.Utils;

namespace Absencespot.Dtos
{
    public class Request
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Note { get; set; }
        public string File { get; set; }
        public StatusType Status { get; set; }

        public Guid LeaveId { get; set; }
        public Guid UserId { get; set; }
        public Guid OnBehalfOfId { get; set; }
        public Guid OfficeId { get; set; }
        public Guid? ApproverId { get; set; }


        public void EnsureValidation()
        {
            if (string.IsNullOrEmpty(Note))
            {
                throw new ArgumentException(nameof(Note));
            }
        }
    }
}
