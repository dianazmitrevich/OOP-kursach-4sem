namespace GoodsSupply.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class REVIEWS
    {
        [Key]
        public int ReviewId { get; set; }

        public int LinkToProductId { get; set; }

        public int LinkAccountId { get; set; }

        [Required]
        [StringLength(200)]
        public string ReviewText { get; set; }

        [StringLength(200)]
        public string AdminText { get; set; }

        public virtual PERSONAL_ACCOUNTS PERSONAL_ACCOUNTS { get; set; }

        public virtual PRODUCTS PRODUCTS { get; set; }
    }
}
