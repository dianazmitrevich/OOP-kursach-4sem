namespace GoodsSupply.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ORDERS
    {
        [Key]
        public int OrderId { get; set; }

        public int LinkAccountId { get; set; }

        [Required]
        [StringLength(100)]
        public string OrderedProducts { get; set; }

        public double OrderPrice { get; set; }

        [StringLength(100)]
        public string Coupon { get; set; }

        public double FinalOrderPrice { get; set; }

        [Required]
        [StringLength(100)]
        public string PaymentMethod { get; set; }

        public virtual PERSONAL_ACCOUNTS PERSONAL_ACCOUNTS { get; set; }
    }
}
