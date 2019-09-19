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

        public Information()
        {
            Students = new List<Student>();
            Courses = new List<Course>();
        }

        public void AddStudent(Student student)
        {

            //if (GetStudentByStudentNum(student.StudentNum) != null)
            //    throw new Exception("El estudiante con " + student.StudentNum + " ya existe en el sistema");
            //if (GetStudentByMail(student.Mail) != null)
            //   throw new Exception("El estudiante con " + student.Mail + " ya existe en el sistema");
            
            this.Students.Add(student);

        }

        public void AddCourse(Course course)
        {
            this.Courses.Add(course);
        }
        public Student GetStudentByStudentNum(int studentNum)
        {
            var studentToReturn = this.Students.First(x => x.StudentNum == studentNum);
            return studentToReturn;
        }
        public Course GetCourseByCourseNumber(int courseNum)
        {
            var courseToReturn = this.Courses.First(x => x.CourseNum == courseNum);
            return courseToReturn;
        }

        public Student GetStudentByMail(string mail)
        {
            var studentToReturn = this.Students.First(x => x.Mail == mail);
            return studentToReturn;
        }

        public void DeleteCourse(int courseNum)
        {
            bool searching= true;
            for (int i = Courses.Count - 1; i >= 0 && searching; i--)
            {
                if (Courses[i].CourseNum == courseNum)
                {
                    Courses.RemoveAt(i);
                    searching=false;
                }
            }
            Console.WriteLine("El curso no existe.");
        }
    }
}
