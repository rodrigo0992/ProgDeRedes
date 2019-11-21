using Entities;
using Logic;
using RemoteServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RemoteService
{
    public class Remote : MarshalByRefObject, IRemote
    {
        public List<Student> Students { get; set; }
        public List<Course> Courses { get; set; }
        public List<StudentCourse> StudentCourses { get; set; }
        public List<StudentSocket> StudentConections { get; set; }
        public List<Teacher> Teachers { get; set; }
        public Boolean LoggedTeacher { get; set; }
        public QueueLogic queueLogic { get; set; }
        public List<Log> historyLog { get; set; }

        public Remote()
        {
            Students = new List<Student>();
            Courses = new List<Course>();
            StudentCourses = new List<StudentCourse>();
            StudentConections = new List<StudentSocket>();
            Teachers = new List<Teacher>();
            historyLog = new List<Log>();
            LoggedTeacher = false;
        }

        public void setMSMQ(String queuePath)
        {
            queueLogic = new QueueLogic(queuePath);
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
            catch (Exception e)
            {
                this.Courses.Add(course);
            }

        }

        public Teacher AddTeacher(Teacher teacher)
        {
            this.Teachers.Add(teacher);
            return teacher;
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
            catch (Exception e)
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
            var courseToReturn = this.Courses.First(x => x.Name.Equals(courseName));
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
            List<Course> coursesNotDeleted = this.Courses.FindAll(x => x.Deleted == false);
            return (coursesNotDeleted.Count > 0 && this.Students.Count > 0);
        }

        public List<StudentCourse> GetStudentCourses()
        {
            return StudentCourses.FindAll(x => x.Course.Deleted == false);
        }

        public List<File> GetStudentCourseFiles(StudentCourse studentCourse)
        {
            return StudentCourses.Find(x => x.Student.StudentNum == studentCourse.Student.StudentNum
                                        && x.Course.CourseNum == studentCourse.Course.CourseNum
                                        && x.Course.Deleted == false).Files;
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
            return StudentConections.Exists(x => x.student.StudentNum == student.StudentNum);
        }

        public void AddStudentConection(Student student)
        {
            
            if (!ExistStudentConection(student))
            {
                StudentSocket studentSocket = new StudentSocket(student);
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
                var studentSocket = StudentConections.First(x => x.student.StudentNum == student.StudentNum);

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
                return StudentConections.First(x => x.student.StudentNum == student.StudentNum);
            }
            else
            {
                throw new Exception("No se puede obtener conexión de estudiante no conectado");
            }
        }

        public File GetFileByName(Student student, Course course, string fileName)
        {
            try
            {
                var studentCourse = StudentCourses.Find(x => x.Student.StudentNum == student.StudentNum && x.Course.CourseNum == course.CourseNum);
                return studentCourse.Files.First(x => x.Name == fileName);
            }
            catch (Exception e)
            {
                throw new Exception("No se encuentra el material");
            }
        }

        public void AssignGrade(Student student, Course course, string fileName, int Grade)
        {
            try
            {
                var file = GetFileByName(student, course, fileName);
                file.Grade = Grade;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void ClearStudentSockets()
        {
            StudentConections.Clear();
        }

        public Teacher GetTeacherByNameOrEmail(string teacher)
        {

            try
            {
                var teacherToReturn = this.Teachers.First(x => x.Name == teacher);
                return teacherToReturn;
            }
            catch (Exception e)
            {
                var teacherToReturn = this.Teachers.First(x => x.Mail == teacher);
                return teacherToReturn;
            }

        }

        public List<Teacher> GetTeachers()
        {
            return Teachers;
        }

        public Boolean LoginTeacher(Teacher teacher)
        {
            Boolean correctLogin = false;
            try
            {
                var teacherToLogin = Teachers.Find(x => x.TeacherNum == teacher.TeacherNum);
                if (teacherToLogin.Password == teacher.Password)
                {
                    correctLogin = true;
                    LoggedTeacher = true;
                }
            }
            catch (Exception e)
            {
                throw new Exception("No se encuentra el profesor ");
            }

            return correctLogin;
        }

        public ICollection<String> GetStudentCourseFilesWithoutGrade()
        {
            if (LoggedTeacher)
            {
                try
                {
                    ICollection<String> listToReturn = new List<String>();
                    foreach (var sc in StudentCourses)
                    {
                        foreach(var f in sc.Files)
                        {
                            if (f.Grade == 0)
                            {
                                var line = "Estudiante: " + sc.Student.Name + " " + sc.Student.SurName + " " + sc.Student.StudentNum
                                           + " - Curso: " + sc.Course.Name + " " + sc.Course.CourseNum
                                           + " - Material: " + f.Name;

                                listToReturn.Add(line);
                            }

                        }
                    }
                    return listToReturn;
                }
                catch (Exception e)
                {
                    throw new Exception("No hay materiales para corregir");
                }
            }else
            {
                throw new Exception("Docente debe iniciar sesión");
            }
        }

        public void AssignGradeByNums(int studentNum, int courseNum, string fileName, int Grade)
        {
            if (LoggedTeacher)
            {
                try
                {
                    Student studentToEvaluate = new Student
                    {
                        StudentNum = studentNum,
                        Name = "",
                        Password = "",
                        SurName = "",
                        Mail = "",
                    };

                    Course courseToEvaluate = new Course
                    {
                        CourseNum = courseNum,
                    };

                    var file = GetFileByName(studentToEvaluate, courseToEvaluate, fileName);
                    file.Grade = Grade;
                    queueLogic.AddToQueue("6", "Se corrigió el material " + fileName + " del estudiante "
                                        + studentNum + " en el curso " + courseNum);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            else
            {
                throw new Exception("Docente debe iniciar sesión");
            }
        }

        public void AddStudentCourseFile(Student student, Course course, File file)
        {
            try
            {
                var studentCourse = StudentCourses.Find(x => (x.Student.StudentNum == student.StudentNum && x.Course.CourseNum == course.CourseNum));
                studentCourse.Files.Add(file);
            }
            catch (Exception e)
            {
                throw new Exception("No se pudo agregar el archivo");
            }
        }

        public void AddLog(Log log)
        {
            historyLog.Add(log);
        }

        public List<Log> GetHistoryLog()
        {
            return historyLog;
        }

    }
}
