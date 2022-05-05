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
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Login { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        public int? LinkAccountId { get; set; }

        [Required]
        [StringLength(5)]
        public string IsAdmin { get; set; }

        public USERS() { }
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
        public virtual PERSONAL_ACCOUNTS PERSONAL_ACCOUNTS { get; set; }
    }
}
