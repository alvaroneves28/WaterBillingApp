﻿namespace WaterBillingWebAPI.Data.Entities
{
    public class UpdateProfileRequest
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

    }
}
