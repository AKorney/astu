using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model
{
    [Table("Transaction")]
    [DataContract]
    public class Transaction
    {
        [Key]
        [Column("id_transaction")]
        [DataMember]
        public int IdTransaction { get; set; }
        [Column("id_account")]
        [DataMember]
        public int FKAccount { get; set; }
        [Column("id_category")]
        [DataMember]
        public int FkCategory { get; set; }
        [Column("name")]
        [DataMember]
        public string Name { get; set; }
        [Column("date")]
        [DataMember]
        public System.DateTime Date { get; set; }
        [Column("notes")]
        [DataMember]
        public string Notes { get; set; }
        [ForeignKey("FKAccount")]
        [DisplayName("Account")]
        [DataMember]
        public  Account Account { get; set; }
        [ForeignKey("FkCategory")]
        [DisplayName("Category")]
        [DataMember]
        public  Category Category { get; set; }
    }
}
