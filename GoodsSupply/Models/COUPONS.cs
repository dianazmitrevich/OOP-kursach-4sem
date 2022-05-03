namespace GoodsSupply.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class COUPONS
    {
        [Key]
        public int CouponId { get; set; }

        [Required]
        [StringLength(100)]
        public string CouponCode { get; set; }

        [Required]
        [StringLength(5)]
        public string IsPercent { get; set; }

        public int PercentOff { get; set; }

        public double? MoneyOff { get; set; }
    }
}
