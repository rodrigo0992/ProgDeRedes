using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    [Serializable]
    public class Course
    {
        public int CourseNum { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }

        public Course()
        {
            Deleted = false;
        }
    }
}
