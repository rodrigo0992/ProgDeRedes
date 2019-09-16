using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace DataBase
{
    public class Information
    {
        public List<Student>  Students { get; set; }
        public List<Course> Courses { get; set; }

        public void AddStudent(Student student)
        {
            if (GetStudentByStudentNum(student.StudentNum) != null)
                throw new Exception("El estudiante con " + student.StudentNum + " ya existe en el sistema");
            if (GetStudentByMail(student.Mail) != null)
                throw new Exception("El estudiante con " + student.Mail + " ya existe en el sistema");

            this.Students.Add(student);
        }

        public Student GetStudentByStudentNum(int studentNum)
        {
           var studentToReturn = this.Students.First(x => x.StudentNum == studentNum);
           if (studentToReturn == null)
                throw new Exception("El estudiante con " + studentNum + " no existe en el sistema");
            return studentToReturn;
        }

        public Student GetStudentByMail(string mail)
        {
            var studentToReturn = this.Students.First(x => x.Mail == mail);
            if (studentToReturn == null)
                throw new Exception("El estudiante con " + mail + " no existe en el sistema");
            return studentToReturn;
        }

    }
}
