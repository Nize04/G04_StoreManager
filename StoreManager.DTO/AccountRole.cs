using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.DTO
{
    public class AccountRole
    {
        [Key]
        public int AccountId { get; set; }
        [Key]
        public int RoleId { get; set; }
    }
}
