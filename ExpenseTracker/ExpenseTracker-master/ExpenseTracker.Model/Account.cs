using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace ExpenseTracker.Model
{
    [Table("Account")]
    [DataContract]
    public class Account
    { 
        [Key]
        [Column("id_account")]
        [DataMember]
        public int IdAccount { get; set; }
        [Required]
        [MaxLength(300)]
        [Column("name")]
        [DataMember]
        public string Name { get; set; }
    }
}
