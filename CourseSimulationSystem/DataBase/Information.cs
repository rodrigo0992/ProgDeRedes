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
            string studentNum = student.StudentNum.ToString();
            string mail = student.Mail;

            if (StudentExists(studentNum))
                throw new Exception("El estudiante con número " + studentNum + " ya existe");

            if (StudentExists(mail))
                throw new Exception("El estudiante con mail " + mail + " ya existe");

            this.Students.Add(student);
        }

        public void AddCourse(Course course)
        {
            string courseName = course.Name;
            int courseNum = course.CourseNum;

            if (CourseExistsByName(courseName))
                throw new Exception("El curso con nombre " + courseName + " ya existe");

            if (CourseExistsByNumber(courseNum))
                throw new Exception("El curso con número " + courseNum + " ya existe");

            if (!ValidDeletedCourseNumber(courseNum, courseName))
                throw new Exception("El número " + courseNum + " no puede ser utilizado");

            try
            {
                Course courseToReinsert = GetDeletedCourseByCourseName(courseName);
                courseToReinsert.Deleted = false;
                Console.WriteLine("El curso fue reactivado al igual que todas sus dependencias");
            }
            catch(Exception e)
            {
                this.Courses.Add(course);
            }

        }
        public void AddStudentCourse(StudentCourse studentCourse)
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

        public Student GetStudentByStudentNumOrEmail(string studentNum)
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
            var courseToReturn = this.Courses.First(x => x.CourseNum == courseNum && x.Deleted == false);
            return courseToReturn;
        }
        public Course GetCourseByCourseName(string courseName)
        {
            var courseToReturn = this.Courses.First(x => x.Name.Equals(courseName) && x.Deleted == false);
            return courseToReturn;
        }

        public bool ValidDeletedCourseNumber(int courseNum, string courseName)
        {
            try
            {
                this.Courses.First(x => x.CourseNum == courseNum && x.Name != courseName);
                return false;
            }
            catch (Exception e)
            {
                return true;
            }
        }
        public Course GetDeletedCourseByCourseName(string courseName)
        {
            var courseToReturn = this.Courses.First(x => x.Name.Equals(courseName) );
            return courseToReturn;
        }

        public bool CourseExistsByName(string courseName)
        {
            return this.Courses.Exists(x => x.Name == courseName && x.Deleted == false);
        }

        public bool CourseExistsByNumber(int courseNumber)
        {
            return this.Courses.Exists(x => x.CourseNum == courseNumber && x.Deleted == false);
        }

        public void DeleteCourse(int courseNum)
        {
            Course courseToDelete = Courses.First(x => x.CourseNum == courseNum);
            courseToDelete.Deleted = true;
        }
        public bool existsStudentsAndCourses()
        {
            List<Course> coursesNotDeleted = this.Courses.FindAll(x=> x.Deleted == false);
            return (coursesNotDeleted.Count > 0 && this.Students.Count > 0);
        }

        public List<StudentCourse> GetStudentCourses()
        {
            return StudentCourses.FindAll(x => x.Course.Deleted == false);
        }

        public List<File> GetStudentCourseFiles(StudentCourse studentCourse)
        {
            return StudentCourses.Find(x => x == studentCourse && x.Course.Deleted == false).Files;
        }

        public List<Course> GetCourses()
        {
            return Courses.FindAll(x => x.Deleted == false);
        }

        public List<Student> GetStudents()
        {
            return Students;
        }

        public bool ExistStudentConection(Student student)
        {
            return StudentConections.Exists(x => x.student == student);
        }

        public void AddStudentConection(Student student, TcpClient tcpClient, TcpClient tcpClientBackground)
        {
            if (!ExistStudentConection(student))
            {
                StudentSocket studentSocket = new StudentSocket(student, tcpClient, tcpClientBackground);
                StudentConections.Add(studentSocket);
            }
            else
            {
                throw new Exception("El estudiante ya se encuentra conectado");
            }
        }

        public void DeleteStudentConection(Student student)
        {
            try
            {
                var studentSocket = StudentConections.First(x => x.student == student);

                StudentConections.Remove(studentSocket);
            }
            catch (Exception e)
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

        public void ClearStudentSockets()
        {
            foreach (var item in StudentConections)
            {
                item.tcpClientBackground.Dispose();
                item.tcpClient.Dispose();
            }
            StudentConections.Clear();
        }
    }
}
