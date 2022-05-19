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

        [Required]
        [StringLength(100)]
        public string CouponText { get; set; }

        public COUPONS(string code, string isPercent, int percentOff, double moneyOff, string text)
        {
            this.CouponCode = code;
            this.IsPercent = isPercent;
            this.PercentOff = percentOff;
            this.MoneyOff = moneyOff;
            this.CouponText = text;
        }

        public COUPONS() { }
    }
}
