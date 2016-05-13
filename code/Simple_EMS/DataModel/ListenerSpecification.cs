using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_EMS.DataModel
{
    public class ListenerSpecification
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
        [MaxLength(2000)]
        public string URL { get; set; }
        [Required]
        [MaxLength(10)]
        public string Method { get; set; }
        [MaxLength(2000)]
        public string Headers { get; set; }
        [Required]
        public int Max_Retrials { get; set; }
        /// <summary>
        /// the duration in days to keep the listener instance
        /// </summary>
        [Required]
        public int Keep_Duration { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public DateTime ModifiedDate { get; set; }
        #endregion
    }
}
