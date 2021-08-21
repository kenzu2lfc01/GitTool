using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitTool.Web.Models
{
    public class CreateBranchNameModel
    {
        public string TicketType { get; set; }
        public string TicketCode { get; set; }
        public string TicketName { get; set; }
        public string ProjectDirectory { get; set; }
    }
}
