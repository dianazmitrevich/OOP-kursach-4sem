namespace GoodsSupply.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PRODUCTS_DETAIL
    {
        [Key]
        public int DetailId { get; set; }

        public int LinkToProductId { get; set; }

        public int ProductCode { get; set; }

        [Required]
        [StringLength(100)]
        public string Material { get; set; }

        [Required]
        [StringLength(100)]
        public string Package { get; set; }

        [Required]
        [StringLength(100)]
        public string Size { get; set; }

        [Required]
        [StringLength(200)]
        public string BigDescription { get; set; }

        public virtual PRODUCTS PRODUCTS { get; set; }
    }
}
