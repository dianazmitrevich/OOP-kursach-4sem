namespace GoodsSupply.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

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
            this.Password = password;
            this.LinkAccountId = linkAccountId;
            this.IsAdmin = isAdmin;
        }

        public virtual PERSONAL_ACCOUNTS PERSONAL_ACCOUNTS { get; set; }
    }
}
