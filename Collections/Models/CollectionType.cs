using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection.Data.Models
{
    public class CollectionType
    {
        [Key]
        public int CollectionTypeId { get; set; }
        [Column("CollectionType")]
        public string Type { get; set; }
        public int? UserId { get; set; }
        public bool Sales { get; set; }
    }
}
