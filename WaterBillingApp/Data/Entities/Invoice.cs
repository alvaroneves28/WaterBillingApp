﻿using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Data.Entities
{
    public enum InvoiceStatus
    {
        Pending,
        Approved,
        Rejected
    }
    public class Invoice
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }

        [Required]
        [Range(0.01, 100000)]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(20)]
        public InvoiceStatus Status { get; set; }
        [Required]
        public int ConsumptionId { get; set; }
        public Consumption Consumption { get; set; }
    }
}
