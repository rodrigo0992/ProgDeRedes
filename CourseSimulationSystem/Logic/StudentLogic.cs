using DataBase;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    class StudentLogic
    {
        private Information Information { get; set; }

        public StudentLogic(Information information)
        {
            this.Information = information;
        }

        public string Login(Student student)
        {
            return "";
        }

        public Student AddStudent(Student student)
        {
            this.Information.Students.Add(student);
            return student;
        }    

    }
}
