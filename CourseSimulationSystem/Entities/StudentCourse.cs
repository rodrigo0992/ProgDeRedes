﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    class StudentCourse
    {
        public Student Student { get; set; }
        public Course Course { get; set; }
        public List<File> Files { get; set; }
    }
}
