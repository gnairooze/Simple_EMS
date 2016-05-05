using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_EMS.EMS_Data
{
    public class EMS_Context:DbContext
    {
        public DbSet<DataModel.EventSpecification> EventSpecifications { get; set; }
        public DbSet<DataModel.ListenerSpecification> ListenerSpecifications { get; set; }
        public DbSet<DataModel.EventSpecification_ListenerSpecification> EventSpecification_ListenerSpecifications { get; set; }
        public DbSet<DataModel.EventInstance> EventInstances { get; set; }
        public DbSet<DataModel.ListenerInstance> ListenerInstances { get; set; }
    }
}
