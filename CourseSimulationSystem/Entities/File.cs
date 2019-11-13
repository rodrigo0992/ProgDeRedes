using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class File
    {
        public StudentCourse StudentCourse { get; set; }
        public String Name { get; set; }
        public int Grade { get; set; }
        public string FileSource { get; set; }
    }
}
