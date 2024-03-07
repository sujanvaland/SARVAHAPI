using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
    public class Recuiter : BaseEntity
    {
        public int RecuiterId { get; set; }
        public string CompanyName { get; set; }
        public string designation { get; set; }
        public string CompanyType { get; set; }
        public string NoOfEmployee { get; set; }
        public string WebsiteLink { get; set; }
    }
}
