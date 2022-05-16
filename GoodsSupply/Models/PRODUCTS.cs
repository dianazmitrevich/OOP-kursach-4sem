namespace GoodsSupply.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PRODUCTS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PRODUCTS()
        {
            PRODUCTS_DETAIL = new HashSet<PRODUCTS_DETAIL>();
            REVIEWS = new HashSet<REVIEWS>();
        }

        [Key]
        public int ProductId { get; set; }

        public int LinkToCategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        public byte[] Image { get; set; }

        public double Price { get; set; }

        public int Quantity { get; set; }

        public virtual CATEGORIES CATEGORIES { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUCTS_DETAIL> PRODUCTS_DETAIL { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REVIEWS> REVIEWS { get; set; }

        public PRODUCTS(int category, string name, string description, double price, int quantity)
        {
            this.LinkToCategoryId = category;
            this.Name = name;
            this.Description = description;
            this.Price = price;
            this.Quantity = quantity;
        }
    }
}
