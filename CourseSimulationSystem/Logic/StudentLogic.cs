using DataBase;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Logic
{
    public class StudentLogic
    {
        private Information Information { get; set; }

        private readonly object StudentConectionLock = new object();

        public StudentLogic()
        {

        }
        public StudentLogic(Information information)
        {
            this.Information = information;
        }


        public void AddStudent(Student student)
        {
            this.Information.AddStudent(student);
        }  
        
        public List<Student> GetStudents()
        {
            return this.Information.Students;
        }

        public bool StudentExists(string number)
        {
            return this.Information.StudentExists(number);
        }
        public Student GetStudentByStudentNum(string number)
        {
            return this.Information.GetStudentByStudentNum(number);
        }

        public void AddStudentCourseFile(Student student, Course course, File file)
        {
            try
            {
                var studentCourse = Information.GetStudentCourses().Find(x => (x.Student == student && x.Course == course));
                studentCourse.Files.Add(file);
                
            }
            catch(Exception e)
            {
                throw new Exception("No se pudo agregar el archivo");
            }
        }

        public List<File> GetStudentCourseFiles(Student student, Course course)
        {
            try
            {
                var studentCourse = Information.GetStudentCourses().Find(x => x.Student == student && x.Course == course);
                return studentCourse.Files;
            }
            catch(Exception e)
            {
                throw new Exception("No tiene materiales subidos");
            }
        }

        public List<File> GetStudentCourseFilesWithoutGrade(Student student, Course course)
        {
            try
            {
                var studentCourse = Information.GetStudentCourses().Find(x => x.Student == student && x.Course == course);
                return studentCourse.Files.Where(x => x.Grade == 0).ToList();
            }
            catch (Exception e)
            {
                throw new Exception("No tiene materiales subidos");
            }
        }

        public File GetFileByName(Student student, Course course, string fileName)
        {
            try
            {
                var studentCourse = Information.GetStudentCourses().Find(x => x.Student == student && x.Course == course);
                return studentCourse.Files.First(x => x.Name == fileName);
            }
            catch (Exception e)
            {
                throw new Exception("No se encuentra el material");
            }
        }

        public void AssignGrade(Student student, Course course, string fileName, int Grade)
        {
            try{
                var file = GetFileByName( student,  course,  fileName);
                file.Grade = Grade;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public string FileListToResponse(List<File> files)
        {
            string response = "";
            foreach (File file in files)
            {
                response += "Archivo: " + file.Name + " , Nota: " + file.Grade + "-";
            }
            return response;
        }

        public void AddStudentConection(Student student, TcpClient tcpClient)
        {
            lock (StudentConectionLock)
            {
                try
                {
                    Information.AddStudentConection(student, tcpClient);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }

        }

        public void DeleteStudentConection(Student student)
        {
            lock (StudentConectionLock)
            {
                try
                {
                    Information.DeleteStudentConection(student);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }

        }

        public StudentSocket GetStudentSocket(Student student)
        {
            lock (StudentConectionLock)
            {
                try
                {
                    return Information.GetStudentSocket(student);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }

        }

        public bool ValidateStudentNumber(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                {
                    Console.WriteLine("Debe ingresar solo numeros:");
                    return false;
                }
                    
            }
            return true;
        }

        public string ValidateUserMail(string mail)
        {
            String regExMail = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            while (String.IsNullOrEmpty(mail) ||
            !Regex.IsMatch(mail, regExMail))
            {
                Console.WriteLine("Mail invalido, ingrese nuevamente: ");
                mail = Console.ReadLine();
            }
            return mail;
        }

        public static bool isEmpty(string s)
        {
            if (s == null || s == String.Empty)
            {
                Console.WriteLine("El campo no puede estar vacio");
                return true;
            }
            else
            {
                return false;
            }
        }

        public String setNumber(string message)
        {
            var studentNum = "";
            bool isCorrect = false;
            while (!isCorrect)
            {
                Console.WriteLine(message);
                studentNum = Console.ReadLine();
                isCorrect = ValidateStudentNumber(studentNum) && !isEmpty(studentNum);
            }
            return studentNum;
        }

        public String setName(string message)
        {
            var studentString = "";
            bool isCorrect = false;
            while (!isCorrect)
            {
                Console.WriteLine(message);
                studentString = Console.ReadLine();
                isCorrect = !isEmpty(studentString);
            }
            return studentString;
        }

        public TcpClient getUserTcpClient(Student student)
        {
            return Information.StudentConections.Find(x => x.student == student).tcpClient;
        }
    }
}
