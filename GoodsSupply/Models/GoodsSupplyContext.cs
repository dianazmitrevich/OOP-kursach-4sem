using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace GoodsSupply.Models
{
    public partial class GoodsSupplyContext : DbContext
    {
        public GoodsSupplyContext()
            : base("name=GoodsSupplyContext")
        {
        }

        public virtual DbSet<CATEGORIES> CATEGORIES { get; set; }
        public virtual DbSet<COUPONS> COUPONS { get; set; }
        public virtual DbSet<ORDERED_PRODUCTS> ORDERED_PRODUCTS { get; set; }
        public virtual DbSet<ORDERS> ORDERS { get; set; }
        public virtual DbSet<PERSONAL_ACCOUNTS> PERSONAL_ACCOUNTS { get; set; }
        public virtual DbSet<PRODUCTS> PRODUCTS { get; set; }
        public virtual DbSet<PRODUCTS_DETAIL> PRODUCTS_DETAIL { get; set; }
        public virtual DbSet<REVIEWS> REVIEWS { get; set; }
        public virtual DbSet<USERS> USERS { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CATEGORIES>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<CATEGORIES>()
                .HasMany(e => e.PRODUCTS)
                .WithRequired(e => e.CATEGORIES)
                .HasForeignKey(e => e.LinkToCategoryId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<COUPONS>()
                .Property(e => e.CouponCode)
                .IsUnicode(false);

            modelBuilder.Entity<COUPONS>()
                .Property(e => e.CouponText)
                .IsUnicode(false);

            modelBuilder.Entity<COUPONS>()
                .Property(e => e.IsPercent)
                .IsUnicode(false);

            modelBuilder.Entity<ORDERS>()
                .Property(e => e.Coupon)
                .IsUnicode(false);

            modelBuilder.Entity<ORDERS>()
                .Property(e => e.PaymentMethod)
                .IsUnicode(false);

            modelBuilder.Entity<ORDERS>()
                .Property(e => e.Adress)
                .IsUnicode(false);

            modelBuilder.Entity<ORDERS>()
                .HasMany(e => e.ORDERED_PRODUCTS)
                .WithRequired(e => e.ORDERS)
                .HasForeignKey(e => e.LinkToOrderId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PERSONAL_ACCOUNTS>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<PERSONAL_ACCOUNTS>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<PERSONAL_ACCOUNTS>()
                .HasMany(e => e.ORDERS)
                .WithRequired(e => e.PERSONAL_ACCOUNTS)
                .HasForeignKey(e => e.LinkAccountId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PERSONAL_ACCOUNTS>()
                .HasMany(e => e.USERS)
                .WithOptional(e => e.PERSONAL_ACCOUNTS)
                .HasForeignKey(e => e.LinkAccountId);

            modelBuilder.Entity<PRODUCTS>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<PRODUCTS>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<PRODUCTS>()
                .HasMany(e => e.PRODUCTS_DETAIL)
                .WithRequired(e => e.PRODUCTS)
                .HasForeignKey(e => e.LinkToProductId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PRODUCTS>()
                .HasMany(e => e.REVIEWS)
                .WithRequired(e => e.PRODUCTS)
                .HasForeignKey(e => e.LinkToProductId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PRODUCTS_DETAIL>()
                .Property(e => e.Material)
                .IsUnicode(false);

            modelBuilder.Entity<PRODUCTS_DETAIL>()
                .Property(e => e.Package)
                .IsUnicode(false);

            modelBuilder.Entity<PRODUCTS_DETAIL>()
                .Property(e => e.Size)
                .IsUnicode(false);

            modelBuilder.Entity<PRODUCTS_DETAIL>()
                .Property(e => e.BigDescription)
                .IsUnicode(false);

            modelBuilder.Entity<REVIEWS>()
                .Property(e => e.LinkUserLogin)
                .IsUnicode(false);

            modelBuilder.Entity<REVIEWS>()
                .Property(e => e.ReviewText)
                .IsUnicode(false);

            modelBuilder.Entity<REVIEWS>()
                .Property(e => e.AdminText)
                .IsUnicode(false);

            modelBuilder.Entity<REVIEWS>()
                .Property(e => e.AdminName)
                .IsUnicode(false);

            modelBuilder.Entity<USERS>()
                .Property(e => e.Login)
                .IsUnicode(false);

            modelBuilder.Entity<USERS>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<USERS>()
                .Property(e => e.IsAdmin)
                .IsUnicode(false);

            modelBuilder.Entity<USERS>()
                .HasMany(e => e.REVIEWS)
                .WithRequired(e => e.USERS)
                .HasForeignKey(e => e.LinkUserLogin)
                .WillCascadeOnDelete(false);
        }
    }
}
