﻿namespace Absencespot.Dtos
{
    public class Company
    {
        public Guid Id { get; set; }
        public Guid SubscriptionId { get; set; }
        public string Name { get; set; }
        public string? FiscalNumber { get; set; }
        public string? EmailContact { get; set; }
        public string Industry { get; set; }
        public bool IsActive { get; set; }

        public void EnsureValidation()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ArgumentException(nameof(Name));
            }
            if (string.IsNullOrWhiteSpace(Industry))
            {
                throw new ArgumentException(nameof(Industry));
            }
            if (SubscriptionId == default)
            {
                throw new ArgumentException(nameof(SubscriptionId));
            }
        }
    }
}