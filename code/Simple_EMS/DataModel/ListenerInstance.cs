using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_EMS.DataModel
{
    public class ListenerInstance
    {
        #region properties
        [Key]
        public long ID { get; set; }
        [Required]
        [Index(IsUnique = true)]
        public Guid BusinessID { get; set; }
        [Required]
        public long EventInstance_ID { get; set; }
        [Required]
        public long ListenerSpecification_ID { get; set; }
        [MaxLength(50)]
        public string Identifier1Name { get; set; }
        [MaxLength(50)]
        public string Identifier1Value { get; set; }
        [MaxLength(50)]
        public string Identifier2Name { get; set; }
        [MaxLength(50)]
        public string Identifier2Value { get; set; }
        [MaxLength(50)]
        public string Identifier3Name { get; set; }
        [MaxLength(50)]
        public string Identifier3Value { get; set; }
        [Required]
        [MaxLength(2000)]
        public string URL { get; set; }
        [Required]
        [MaxLength(10)]
        public string Method { get; set; }
        [MaxLength(2000)]
        public string Headers { get; set; }
        [Required]
        public string EventData { get; set; }
        [Required]
        public int RemainingRetrials { get; set; }
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
