using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFan.Models
{
    public class FanModel
    {
        public int Id { get; set; }

        public bool IsActive { get; set; }

        public string Speed { get; set; }
    }
}
