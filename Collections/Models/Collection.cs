using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection.Data.Models
{
    public class Collection
    {
        [Key]
        public int CollectionId { get; set; }
        public decimal Amount { get; set; }
        public int UserId { get; set; }
        public int CollectionTypeId { get; set; }
        public DateTime CollectionDate { get; set; }

        public virtual CollectionType CollectionType { get; set; }
    }
}
