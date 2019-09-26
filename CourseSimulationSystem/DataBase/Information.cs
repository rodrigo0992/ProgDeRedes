using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using System.Net.Sockets;

namespace DataBase
{
    public class Information
    {
        public List<Student>  Students { get; set; }
        public List<Course> Courses { get; set; }
        public List<StudentCourse> StudentCourses { get; set; }
        public List<StudentSocket> StudentConections { get; set; }

        public Information()
        {
            Students = new List<Student>();
            Courses = new List<Course>();
            StudentCourses = new List<StudentCourse>();
            StudentConections = new List<StudentSocket>();
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
        public void AddStrudentCourse(StudentCourse studentCourse)
        {
            this.StudentCourses.Add(studentCourse);
        }
        public bool StudentExists(string studentNum)
        {
            try
            {
                this.Students.First(x => x.StudentNum == Convert.ToInt32(studentNum));
                return true;
            }
            catch(Exception e)
            {
                try
                {
                    this.Students.First(x => x.Mail == studentNum);
                    return true;
                }
                catch (Exception e1)
                {
                    return false;
                }
            }
        }

        public Student GetStudentByStudentNum(string studentNum)
        {
            try
            {
                var studentToReturn = this.Students.First(x => x.StudentNum == Convert.ToInt32(studentNum));
                return studentToReturn;
            }
            catch (Exception e)
            {
                var studentToReturn = this.Students.First(x => x.Mail == studentNum);
                return studentToReturn;
            }
        }

        public Course GetCourseByCourseNumber(int courseNum)
        {
            var courseToReturn = this.Courses.First(x => x.CourseNum == courseNum);
            return courseToReturn;
        }
        public Course GetCourseByCourseName(string courseName)
        {
            var courseToReturn = this.Courses.First(x => x.Name.Equals(courseName));
            return courseToReturn;
        }

        public Student GetStudentByMail(string mail)
        {
            var studentToReturn = this.Students.First(x => x.Mail == mail);
            return studentToReturn;
        }

        public void DeleteCourse(int courseNum)
        {
            bool searching = true;
            for (int i = Courses.Count - 1; i >= 0 && searching; i--)
            {
                if (Courses[i].CourseNum == courseNum)
                {
                    Courses.RemoveAt(i);
                    searching = false;
                }
            }
            Console.WriteLine("El curso no existe.");
        }
        public bool existsStudentsAndCourses()
        {
            return (this.Courses.Count > 0 && this.Students.Count > 0);
        }

        public List<StudentCourse> GetStudentCourses()
        {
            return StudentCourses;
        }

        public bool ExistStudentConection(Student student)
        {
            return StudentConections.Exists(x => x.student == student);
        }

        public void AddStudentConection(Student student, TcpClient tcpClient)
        {
            if (!ExistStudentConection(student))
            {
                StudentSocket studentSocket = new StudentSocket(student, tcpClient);
                StudentConections.Add(studentSocket);
            }
            else
            {
                throw new Exception("El estudiante ya se encuentra conectado");
            }
        }

        public bool CourseExists(string courseName)
        {
            return this.Courses.Exists(x => x.Name == courseName);
           
        }
        public void DeleteStudentConection(Student student)
        {
            if (ExistStudentConection(student))
            {
                var studentSocket = StudentConections.First(x => x.student == student);
                StudentConections.Remove(studentSocket);
            }
            else
            {
                throw new Exception("No se puede borrar conexión de estudiante no conectado");
            }
        }

        public StudentSocket GetStudentSocket(Student student)
        {
            if (ExistStudentConection(student))
            {
                return StudentConections.First(x => x.student == student);
            }
            else
            {
                throw new Exception("No se puede obtener conexión de estudiante no conectado");
            }
        }
    }
}
