﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Models
{
    [Table("CreditCard")]
    public partial class CreditCard
    {
        public long? Value { get; set; }
        [Key]
        [StringLength(16)]
        public string Number { get; set; }
        [Required]
        [StringLength(20)]
        public string Name { get; set; }
        public int CVV { get; set; }
        [Required]
        [StringLength(10)]
        public string Month { get; set; }
        [Required]
        [StringLength(10)]
        public string Year { get; set; }
        [Required]
        [StringLength(50)]
        public string ZipCode { get; set; }
        public int? CityId { get; set; }
        [StringLength(450)]
        public string UserId { get; set; }

        [ForeignKey("CityId")]
        [InverseProperty("CreditCards")]
        public virtual City City { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("CreditCards")]
        public virtual AspNetUser User { get; set; }
    }
}