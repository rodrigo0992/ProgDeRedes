using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Student
    {
        public int StudentNum { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public bool Logged { get; set; }
        public List<StudentCourse> StudentCourses { get; set; }

        public Student()
        {
        }
    }
}
