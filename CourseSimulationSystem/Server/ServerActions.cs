using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic;
using System.Net.Sockets;
using Entities;
using Protocol;
using System.IO;

namespace Server
{
    public class ServerActions
    {
        CourseLogic courseLogic;
        StudentLogic studentLogic;
        private string file;
        private int fileLenght;
        private string nameFile;
        private List<StudentSocket> StudentsSocketsList { get; set; }

        public ServerActions(CourseLogic courseLogic, StudentLogic studentLogic)
        {
            this.courseLogic = courseLogic;
            this.studentLogic = studentLogic;
            this.StudentsSocketsList = new List<StudentSocket>();
        }

        // Start response server methods
        public Student Login(string data, NetworkStream networkStreamResponse, TcpClient tcpClient, TcpClient tcpClientBackground)
        {
            var studentToLogin = new Student();
            try
            {
                var dataArray = Message.Deserialize(data);
                var student = dataArray[0];
                var password = dataArray[1];
                var studentExists = this.studentLogic.StudentExists(student);
                if (!studentExists)
                {
                    Message.SendMessage(networkStreamResponse, "RES", 1, "Estudiante no existe");
                }
                else
                {
                    try
                    {
                        studentToLogin = this.studentLogic.GetStudentByStudentNumOrEmail(student);
                        if (studentToLogin.Password == password)
                        {
                           
                            studentLogic.AddStudentConection(studentToLogin);
                            var studentSocket = new StudentSocket
                            {
                                student = studentToLogin,
                                tcpClient = tcpClient,
                                tcpClientBackground = tcpClientBackground,
                            };
                            this.StudentsSocketsList.Add(studentSocket);

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
                
            }
            catch(Exception e)
            {
                Console.WriteLine("Faltan datos para loguearse");   
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
            if (!courseLogic.CourseExistsByName(course))
            {
                Message.SendMessage(networkStreamResponse, "RES", 3, "No existe ningun curso con ese nombre.");
            }
            else
            {
                Course courseToSuscribe = courseLogic.getCourseByCourseName(course);
                StudentCourse studentCourse = new StudentCourse();
                studentCourse.Course = courseToSuscribe;
                studentCourse.Student = student;
                courseLogic.AddStudentToCourse(studentCourse);
                Message.SendMessage(networkStreamResponse, "RES", 3, "Curso agregado exitosamente");
            }
        }


        public void AddStudent()
        {
            try
            {
                Console.WriteLine("CREAR USUARIO");
                var studentNum = studentLogic.setNumber("Ingrese número de usuario");
                var studentName = studentLogic.setName("Ingrese nombre de usuario");
                var studentSurname = studentLogic.setName("Ingrese apellido");
                Console.WriteLine("Ingrese mail:");
                var mail= Console.ReadLine();
                mail = studentLogic.ValidateUserMail(mail);
                var studentPassword = studentLogic.setName("Ingrese contraseña");
                Student newStudent = new Student();
                newStudent.StudentNum = Convert.ToInt32(studentNum);
                newStudent.Name = studentName;
                newStudent.Password = studentPassword;
                newStudent.SurName = studentSurname;
                newStudent.Mail = mail;
                this.studentLogic.AddStudent(newStudent);
                Console.WriteLine("Usuario creado con éxito");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
            
        public void ListStudents()
        {
            Console.WriteLine("Lista de estudiantes:");
            var students = this.studentLogic.GetStudents();
            foreach (Student item in students)
            {
                Console.WriteLine(item.StudentNum + " - " + item.Name + " " + item.SurName + " - " + item.Mail);
            }
        }

        public void AddCourse()
        {
            try
            {
                Console.WriteLine("CREAR CURSO");
                var courseNumber = courseLogic.setNumber("Ingrese numero de curso");
                var courseName = courseLogic.setName("Igrese nombre de curso");
                Course newCourse = new Course();
                newCourse.CourseNum = Convert.ToInt32(courseNumber);
                newCourse.Name = courseName;
                courseLogic.AddCourse(newCourse);
                Console.WriteLine("Curso creado con éxito");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public void ListAllCourses()
        {
            Console.WriteLine("Lista de cursos:");
            var courses = courseLogic.GetCourses();
            foreach (Course course in courses)
            {
                Console.WriteLine(course.CourseNum + " - " + course.Name);
            }
        }

        public void ListCourses(List<Course> courses)
        {
            Console.WriteLine("Lista de cursos:");
            
            foreach (Course course in courses)
            {
                Console.WriteLine(course.CourseNum + " - " + course.Name);
            }
        }

        public void DeleteCourse()
        {
            try
            {
                var courses = this.courseLogic.GetCourses();
                ListCourses(courses);
                Console.WriteLine("Borrar curso ");
                int numCourseToDelete = Convert.ToInt32(courseLogic.setNumber("Ingrese numero de curso:"));
                this.courseLogic.DeleteCourse(numCourseToDelete);
                Console.WriteLine("Curso eliminado exitosamente");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void AssignStudentToCourse()
        {
            if (!courseLogic.existsStudentsAndCourses())
            {
                Console.WriteLine("No existen estudiantes y/o cursos.");
            }
            else
            {
                try
                {
                    Console.WriteLine("Seleccione el numero del alumno:");
                    ListStudents();
                    var studentNum = Console.ReadLine();
                    Student student = studentLogic.GetStudentByStudentNumOrEmail(studentNum);
                    Console.WriteLine("Seleccione el curso al que desea inscribir al alumno:");
                    var courses = courseLogic.GetAvailablesCourses(student);
                    ListCourses(courses);
                    int courseNum = Convert.ToInt32(Console.ReadLine());
                    Course course = courseLogic.getCourseByCourseNumber(courseNum);
                    StudentCourse studentCourse = new StudentCourse();
                    studentCourse.Course = course;
                    studentCourse.Student = student;

                    this.courseLogic.AddStudentToCourse(studentCourse);
                    Console.WriteLine("Usuario " + student.Name + " agregado a curso " + course.Name + " existosamente");

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

        }

        public void GetCoursesWithDetails(Student student,NetworkStream networkStreamResponse)
        {
            var listWithDetails = courseLogic.GetCoursesWithDetails(student);
            string listToResponse = "";
            foreach (var item in listWithDetails)
            {
                listToResponse += item + "-"; 
            }

            Message.SendMessage(networkStreamResponse, "RES", 4, listToResponse);
        }

        public void GetEnrolledCourses(Student student, NetworkStream networkStreamResponse)
        {
            var enrolledCourses = this.courseLogic.GetEnrolledCourses(student);
            var courseList = courseLogic.prepareCourseListResponse(enrolledCourses);
            Message.SendMessage(networkStreamResponse, "RES", 4, courseList);
        }


        public void AddFileToStudentCourse(Student student, string data, NetworkStream networkStreamResponse)
        {
            try
            {
                var dataArray = Message.Deserialize(data);
                var courseRecived = dataArray[0];
                var fileSourceRecived = dataArray[1];
                var nameRecived = dataArray[2];

                Course course = courseLogic.getCourseByCourseName(courseRecived);
                Entities.File file = new Entities.File();
                file.Name = nameRecived;
                file.FileSource = fileSourceRecived;
                studentLogic.AddStudentCourseFile(student, course, file);
                Message.SendMessage(networkStreamResponse, "RES", 5, "OK");

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

        public void ListFiles(List<Entities.File> listFiles)
        {
            Console.WriteLine("Lista de materiales:");
            foreach (Entities.File file in listFiles)
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
                    var studentNum = studentLogic.setName("Ingrese numero de estudiante:");
                    Student student = studentLogic.GetStudentByStudentNumOrEmail(studentNum);
                    Console.WriteLine("Seleccione el curso al que desea asignar una nota:");
                    var courses = courseLogic.GetEnrolledCourses(student);
                    ListCourses(courses);
                    int courseNum = Convert.ToInt32(studentLogic.setNumber("Seleccione numero de curso:"));
                    Course course = courseLogic.getCourseByCourseNumber(courseNum);

                    Console.WriteLine("Tiene los siguientes materiales para asignar nota, seleccione uno:");
                    var listFiles = studentLogic.GetStudentCourseFilesWithoutGrade(student, course);
                    ListFiles(listFiles);
                    var fileName = Console.ReadLine();

                    Console.WriteLine("Asigne una nota al material seleccionado:");
                    int grade = Convert.ToInt32(studentLogic.setNumber("Indicar nota:"));

                    studentLogic.AssignGrade(student, course, fileName, grade);

                    Console.WriteLine("Nota asignada con éxito");

                    try
                    {
                        var networStream = StudentsSocketsList.Find(x=>x.student == student).tcpClientBackground.GetStream();
                        var notification = course.Name + ";" + fileName + ";" + grade;
                        Message.SendMessage(networStream, "REQ", 0, notification);

                        Console.WriteLine("Se notificó al alumno");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("No se notificó al alumno");
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

        }

        public void GetFileInitialData(string data, NetworkStream networkStreamResponse)
        {
            try
            {
                var dataArray = Message.Deserialize(data);
                var fileLengthRecived = dataArray[0];
                var nameRecived = dataArray[1];

                fileLenght = Convert.ToInt32(fileLengthRecived);
                nameFile = nameRecived;

                Message.SendMessage(networkStreamResponse, "RES", 7, "OK");

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public void GetFilePartData(string data, NetworkStream networkStreamResponse)
        {
            file = file + data;
            Message.SendMessage(networkStreamResponse, "RES", 8, "OK");
        }

        public void GetFileFinalData(string data, NetworkStream networkStreamResponse)
        {
            file = file + data;
            try
            {
                string folder = Environment.CurrentDirectory;
                string path = Path.Combine(folder, nameFile);

                System.IO.File.WriteAllBytes(path, Convert.FromBase64String(file));
                Message.SendMessage(networkStreamResponse, "RES", 9, "OK");
                file = "";
                nameFile = "";

            }
            catch(Exception e)
            {
                Message.SendMessage(networkStreamResponse, "RES", 9, "FAIL");
            }
        }

        public void DisconectClient(Student student,NetworkStream networkStreamResponse)
        {
            try
            {
                studentLogic.DeleteStudentConection(student);
                Message.SendMessage(networkStreamResponse, "RES", 10, "OK");
            }
            catch (Exception e)
            {
                Message.SendMessage(networkStreamResponse, "RES", 10, "FAIL");
            }
        }

        public void DisconectClient(Student student)
        {
            try
            {
                studentLogic.DeleteStudentConection(student);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void ClearStudentConections()
        {
            try
            {
                studentLogic.ClearStudentConections();
                foreach (var item in StudentsSocketsList)
                {
                    item.tcpClient.Dispose();
                    item.tcpClientBackground.Dispose();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }


}
