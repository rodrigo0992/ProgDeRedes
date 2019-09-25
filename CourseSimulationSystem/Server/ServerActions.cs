using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic;
using System.Net.Sockets;
using Entities;
using Newtonsoft.Json.Linq;
using Protocol;

namespace Server
{
    public class ServerActions
    {
        CourseLogic courseLogic;
        StudentLogic studentLogic;
        public ServerActions(CourseLogic courseLogic, StudentLogic studentLogic)
        {
            this.courseLogic = courseLogic;
            this.studentLogic = studentLogic;
        }

        // Start response server methods
        public Student Login(string data, NetworkStream networkStreamResponse, TcpClient tcpClient)
        {
            var studentToLogin = new Student();
            var json = JObject.Parse(data);
            var studentNum = json["studentNum"].ToString();
            var password = json["password"].ToString();
            var studentExists = this.studentLogic.StudentExists(Convert.ToInt32(studentNum));
            if (!studentExists)
            {
                Message.SendMessage(networkStreamResponse, "RES", 1, "Estudiante no existe");
            }
            else
            {
                try
                {
                    studentToLogin = this.studentLogic.GetStudentByStudentNum(Convert.ToInt32(studentNum));
                    studentLogic.AddStudentConection(studentToLogin, tcpClient);
                    if (studentToLogin.Password == password)
                    {
                        Message.SendMessage(networkStreamResponse, "RES", 1, "Password correcta");
                    }
                    else
                    {
                        Message.SendMessage(networkStreamResponse, "RES", 1, "Password incorrecta");
                    }
                }
                catch (Exception e)
                {
                    Message.SendMessage(networkStreamResponse, "RES", 1, e.Message);
                }
            }
            return studentToLogin;
                
        }

        public void ListCoursesRequest(Student student,NetworkStream networkStreamResponse)
        {
            var courseList = this.courseLogic.prepareCourseListResponse(courseLogic.GetAvailablesCourses(student));
            Message.SendMessage(networkStreamResponse, "RES", 2, courseList);
        }


        public void AddStudentToCourse(Student student, string course, NetworkStream networkStreamResponse)
        {
            Course courseToSuscribe = courseLogic.getCourseByCourseName(course);
            StudentCourse studentCourse = new StudentCourse();
            studentCourse.Course = courseToSuscribe;
            studentCourse.Student = student;
            courseLogic.AddStudentToCourse(studentCourse);
            Message.SendMessage(networkStreamResponse, "RES", 3, "Curso agregado exitosamente");
        }


        public void AddStudent()
        {
            Console.WriteLine("CREAR USUARIO");
            var studentNum = setStudentNumber();
            Console.WriteLine("Ingrese nombre:");
            var studentName = Console.ReadLine();
            Console.WriteLine("Ingrese password:");
            var studentPassword = Console.ReadLine();
            Student newStudent = new Student();
            newStudent.StudentNum = Convert.ToInt32(studentNum);
            newStudent.Name = studentName;
            newStudent.Password = studentPassword;
            try
            {
                this.studentLogic.AddStudent(newStudent);
                Console.WriteLine("Usuario creado con éxito");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        private String setStudentNumber()
        {
            var studentNum="";
            bool isCorrect = false;
            while (!isCorrect)
            {
                Console.WriteLine("Ingrese numero de estudiante:");
                studentNum = Console.ReadLine();
                isCorrect= studentLogic.ValidateStudentNumber(studentNum);
            }
            return studentNum;
        }

        public void ListStudents()
        {
            Console.WriteLine("Lista de estudiantes:");
            var students = this.studentLogic.GetStudents();
            foreach (Student item in students)
            {
                Console.WriteLine(item.StudentNum);
            }
        }

        public void AddCourse()
        {
            Console.WriteLine("CREAR CURSO");
            Console.WriteLine("Indique numero del curso:");
            var courseNumber = Console.ReadLine();
            Console.WriteLine("Indique nombre del curso:");
            var courseName = Console.ReadLine();
            Course newCourse = new Course();
            newCourse.CourseNum = Convert.ToInt32(courseNumber);
            newCourse.Name = courseName;
            try
            {
                this.courseLogic.AddCourse(newCourse);
                Console.WriteLine("Curso creado con éxito");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void ListCourses()
        {
            Console.WriteLine("Lista de cursos:");
            int index = 0;
            var courses = this.courseLogic.GetCourses();
            foreach (Course course in courses)
            {
                Console.WriteLine(course.CourseNum + " - " + course.Name);
                index++;
            }
        }

        public void DeleteCourse()
        {
            ListCourses();
            Console.WriteLine("Escriba el numero del curso que desea eliminar: ");
            int numCourseToDelete= Convert.ToInt32(Console.ReadLine());
            this.courseLogic.DeleteCourse(numCourseToDelete);
        }

        public void AssignStudentToCourse()
        {
            if (!courseLogic.existsStudentsAndCourses())
            {
                Console.WriteLine("No existen estudiantes y/o cursos.");
            }
            else
            {
                Console.WriteLine("Seleccione el numero del alumno:");
                ListStudents();
                int studentNum = Convert.ToInt32(Console.ReadLine());
                Student student = studentLogic.GetStudentByStudentNum(studentNum);
                Console.WriteLine("Seleccione el curso al que desea inscribir al alumno:");
                ListCourses();
                int courseNum = Convert.ToInt32(Console.ReadLine());
                Course course = courseLogic.getCourseByCourseNumber(courseNum);
                StudentCourse studentCourse = new StudentCourse();
                studentCourse.Course = course;
                studentCourse.Student = student;
                try
                {
                    this.courseLogic.AddStudentToCourse(studentCourse);
                    Console.WriteLine("Usuario " + student.Name + " agregado a curso " + course.Name + " existosamente");

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

        }

        public void GetEnrolledCourses(Student student,NetworkStream networkStreamResponse)
        {
            var enrolledCourses = this.studentLogic.GetEnrolledCourses(student);
            var courseList = courseLogic.prepareCourseListResponse(enrolledCourses);
            Message.SendMessage(networkStreamResponse, "RES", 4, courseList);
        }


        public void AddFileToStudentCourse(Student student, string data, NetworkStream networkStreamResponse)
        {
            var json = JObject.Parse(data);
            var nameRecived = json["name"].ToString();
            var courseRecived = json["course"].ToString();
            var fileSourceRecived = json["filesource"].ToString();

            try
            {
                Course course = courseLogic.getCourseByCourseName(courseRecived);
                File file = new File();
                file.Name = nameRecived;
                file.FileSource = fileSourceRecived;
                studentLogic.AddStudentCourseFile(student, course, file);
                Message.SendMessage(networkStreamResponse, "RES", 5, "Archivo agregado exitosamente");

            }
            catch (Exception e)
            {
                Message.SendMessage(networkStreamResponse, "RES", 5, e.Message);
            }

        }

        public void GetStudentCourseFiles(Student student, string data, NetworkStream networkStreamResponse)
        {
            var courseRecived = data;
            try
            {
                Course course = courseLogic.getCourseByCourseName(courseRecived);
                var files = studentLogic.GetStudentCourseFiles(student, course);
                var listToResponse = studentLogic.FileListToResponse(files);
                Message.SendMessage(networkStreamResponse, "RES", 6, listToResponse);
            }
            catch (Exception e)
            {
                Message.SendMessage(networkStreamResponse, "RES", 6, e.Message);
            }

        }

        public void ListFiles(List<File> listFiles)
        {
            Console.WriteLine("Lista de materiales:");
            foreach (File file in listFiles)
            {
                Console.WriteLine("Nombre:" + file.Name);
            }
        }

        public void AssignGrade()
        {
            if (!courseLogic.existsStudentsAndCourses())
            {
                Console.WriteLine("No existen estudiantes y/o cursos.");
            }
            else
            {
                try
                {
                    Console.WriteLine("Seleccione el numero del alumno para asignar una nota:");
                    ListStudents();
                    int studentNum = Convert.ToInt32(Console.ReadLine());
                    Student student = studentLogic.GetStudentByStudentNum(studentNum);
                    Console.WriteLine("Seleccione el curso al que desea asignar una nota:");
                    ListCourses();
                    int courseNum = Convert.ToInt32(Console.ReadLine());
                    Course course = courseLogic.getCourseByCourseNumber(courseNum);

                    Console.WriteLine("Tiene los siguientes materiales para asignar nota, seleccione uno:");
                    var listFiles = studentLogic.GetStudentCourseFilesWithoutGrade(student, course);
                    ListFiles(listFiles);
                    var fileName = Console.ReadLine();

                    Console.WriteLine("Asigne una nota al material seleccionado:");
                    int grade = Convert.ToInt32(Console.ReadLine());

                    studentLogic.AssignGrade(student,course,fileName,grade);

                    Console.WriteLine("Nota asignada con éxito");
                    Console.WriteLine("Desea notificar al alumno? (S/N)");
                    var answer = Console.ReadLine().ToLower();
                    if (answer == "s")
                    {

                        Console.WriteLine("Se notificó al alumno");
                    }
                    else {
                        Console.WriteLine("No se notificó al alumno");
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

        }
    }


}
