namespace GoodsSupply.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ORDERED_PRODUCTS
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OrderedProductId { get; set; }

        // [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OrderedQuantity { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int LinkToOrderId { get; set; }

        [Key]
        [Column(Order = 3)]
        public double OrderedProductPrice { get; set; }

        public virtual ORDERS ORDERS { get; set; }

        public ORDERED_PRODUCTS(int orderedProductCode, int orderedQuantity, int orderId, float orderedProductPrice)
        {
            this.OrderedProductId = orderedProductCode;
            this.OrderedQuantity = orderedQuantity;
            this.LinkToOrderId = orderId;
            this.OrderedProductPrice = orderedProductPrice;
        }

        public ORDERED_PRODUCTS() { }
    }
}
