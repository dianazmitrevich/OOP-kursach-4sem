namespace GoodsSupply.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Security.Cryptography;
    using System.Text;

    public partial class USERS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public USERS()
        {
            REVIEWS = new HashSet<REVIEWS>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Key]
        [StringLength(100)]
        public string Login { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        public int? LinkAccountId { get; set; }

        [Required]
        [StringLength(5)]
        public string IsAdmin { get; set; }

        public virtual PERSONAL_ACCOUNTS PERSONAL_ACCOUNTS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REVIEWS> REVIEWS { get; set; }

        public USERS(string login, string password, int linkAccountId, string isAdmin = "N")
        {
            this.Login = login;
            this.Password = getHash(password);
            this.LinkAccountId = linkAccountId;
            this.IsAdmin = isAdmin;
        }

        public static string getHash(string password)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hash);
        }
    }
}