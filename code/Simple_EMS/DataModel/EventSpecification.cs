using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_EMS.DataModel
{
    public class EventSpecification
    {
        #region properties
        [Key]
        public long ID { get; set; }
        [Required]
        [Index(IsUnique = true)]
        public Guid BusinessID { get; set; }
        [Required]
        [MaxLength(50)]
        [Index(IsUnique = true)]
        public string Name { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        #endregion
    }
}
