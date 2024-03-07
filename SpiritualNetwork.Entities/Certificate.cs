using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
    public class Certificate : BaseEntity
    {
        public int UserId { get; set; }
        public string? CertificateName { get; set; }
    }
}
