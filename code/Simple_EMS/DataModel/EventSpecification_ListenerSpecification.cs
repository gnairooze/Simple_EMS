using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_EMS.DataModel
{
    public class EventSpecification_ListenerSpecification
    {
        #region properties
        [Key]
        public long ID { get; set; }
        [Required]
        [Index("EventSpec_ListenerSpec_Unique", IsUnique = true, Order = 1)]
        public long EventSpecification_ID { get; set; }
        [Required]
        [Index("EventSpec_ListenerSpec_Unique", IsUnique = true, Order = 2)]
        public long ListenerSpecification_ID { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        #endregion
    }
}
