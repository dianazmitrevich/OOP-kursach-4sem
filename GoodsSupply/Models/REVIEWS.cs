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

        [Required]
        [StringLength(100)]
        public string LinkUserLogin { get; set; }

        [Required]
        [StringLength(200)]
        public string ReviewText { get; set; }

        [StringLength(200)]
        public string AdminText { get; set; }

        [StringLength(100)]
        public string AdminName{ get; set; }

        public REVIEWS(int productIdParameter)
        {

        }

        public REVIEWS() { }

        public virtual PRODUCTS PRODUCTS { get; set; }

        public virtual USERS USERS { get; set; }
    }
}
