using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_EMS.DataModel
{
    public class EventInstance
    {
        #region properties
        [Key]
        public long ID { get; set; }
        [Index(IsUnique =true)]
        [Required]
        public Guid BusinessID { get; set; }
        [Required]
        public long EventSpecification_ID { get; set; }
        [MaxLength(50)]
        public string Identifier1Name { get; set; }
        [MaxLength(50)]
        [Index]
        public string Identifier1Value { get; set; }
        [MaxLength(50)]
        public string Identifier2Name { get; set; }
        [MaxLength(50)]
        [Index]
        public string Identifier2Value { get; set; }
        [MaxLength(50)]
        public string Identifier3Name { get; set; }
        [MaxLength(50)]
        [Index]
        public string Identifier3Value { get; set; }
        [Required]
        public string EventData { get; set; }
        [Required]
        [Index]
        public int Status { get; set; }
        [MaxLength(50)]
        public string Comment { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public DateTime ModifiedDate { get; set; }
        #endregion
    }
}
