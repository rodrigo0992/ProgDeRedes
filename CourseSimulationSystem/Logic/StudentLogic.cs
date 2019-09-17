using DataBase;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class StudentLogic
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
            try
            {
                this.Information.AddStudent(student);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            return student;
        }  
        
        public List<Student> GetStudents()
        {
            return this.Information.Students;
        }

        public Student GetStudentByStudentNum(int number)
        {
            return this.Information.GetStudentByStudentNum(number);
        }

    }
}
