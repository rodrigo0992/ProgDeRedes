using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    [Serializable]
    public class Log
    {
        public string Type { get; set; }
        public string Description { get; set; }
    }
}
