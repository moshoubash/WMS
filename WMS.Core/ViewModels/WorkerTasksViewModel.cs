using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Core.ViewModels
{
    public class WorkerTasksViewModel
    {
        public ApplicationUser User { get; set; }
        public IList<WorkerTask>? Tasks { get; set;}
    }
}
