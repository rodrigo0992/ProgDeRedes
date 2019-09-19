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



        public void Login(string data, NetworkStream networkStreamResponse)
        {
            Console.WriteLine(data);
            var json = JObject.Parse(data);
            var studentNum = json["studentNum"].ToString();
            var password = json["password"].ToString();

            var studentToLogin = this.studentLogic.GetStudentByStudentNum(Convert.ToInt32(studentNum));

            if (studentToLogin.Password == password)
            {
                Message.SendMessage(networkStreamResponse,"RES",01, "Pasword correcta");
            }
            else
            {
                Message.SendMessage(networkStreamResponse, "RES", 01, "Pasword incorrecta");
            }
                
        }

        public void AddStudent()
        {
            Console.WriteLine("CREAR USUARIO");
            var studentNum= setStudentNumber();
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
                Console.WriteLine("Volver a menú");
                Console.ReadLine();
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
            newCourse.StudentCourses = null;
            try
            {
                this.courseLogic.AddCourse(newCourse);
                Console.WriteLine("Curso creado con éxito");
                Console.WriteLine("Volver a menú");
                Console.ReadLine();
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
            
        }
    }
}
