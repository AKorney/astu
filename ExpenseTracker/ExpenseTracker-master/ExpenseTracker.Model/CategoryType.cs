using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace ExpenseTracker.Model
{
    [Table("Type")]
    [DataContract]
    public class CategoryType
    {
        [Key]
        [Column("id_type")]
        [DataMember]
        public int IdType { get; set; }
        [Column("name")]
        [DataMember]
        public string Name { get; set; }
    }
}