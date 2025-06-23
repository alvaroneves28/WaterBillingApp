using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WaterBillingApp.Data.Entities;

namespace WaterBillingApp.Models
{
    public class MeterViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Número de Série")]
        public string SerialNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Instalação")]
        public DateTime InstallationDate { get; set; }

        [Required]
        [Display(Name = "Ativo")]
        public bool IsActive { get; set; }

        [Required]
        [Display(Name = "Cliente")]
        public int CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public MeterStatus Status { get; set; }

        public IEnumerable<SelectListItem>? CustomersList { get; set; }

        public decimal? LastConsumptionValue { get; set; }
        public DateTime? LastConsumptionDate { get; set; }

        public ConsumptionViewModel LastConsumption { get; set; }

    }
}
