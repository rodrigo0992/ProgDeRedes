using DataBase;
using Entities;
using RemoteServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Logic
{
    public class StudentLogic
    {
        private IRemote Remote { get; set; }

        private readonly object StudentConectionLock = new object();

        public StudentLogic()
        {

        }
        public StudentLogic(IRemote remote)
        {
            this.Remote = remote;
        }


        public void AddStudent(Student student)
        {
            try
            {
                this.Remote.AddStudent(student);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }  
        
        public List<Student> GetStudents()
        {
            return this.Remote.GetStudents();
        }

        public bool StudentExists(string number)
        {
            return this.Remote.StudentExists(number);
        }
        public Student GetStudentByStudentNumOrEmail(string number)
        {
            try
            {
                return this.Remote.GetStudentByStudentNumOrEmail(number);
            }
            catch (Exception e)
            {
                throw new Exception("No se encuentra el estudiante con número/email: " + number);
            }
        }

        public void AddStudentCourseFile(Student student, Course course, File file)
        {
            try
            {
                var studentCourse = Remote.GetStudentCourses().Find(x => (x.Student.StudentNum == student.StudentNum && x.Course.CourseNum == course.CourseNum));
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
                var studentCourse = Remote.GetStudentCourses().Find(x => x.Student.StudentNum == student.StudentNum && x.Course.CourseNum == course.CourseNum);
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
                Console.WriteLine("Student Num: " + student.StudentNum);
                Console.WriteLine("Course Num: " + course.CourseNum);
                var studentCourse = Remote.GetStudentCourses().Find(x => x.Student.StudentNum == student.StudentNum && x.Course.CourseNum == course.CourseNum);
                return studentCourse.Files.Where(x => x.Grade == 0).ToList();
            }
            catch (Exception e)
            {
                throw new Exception("No tiene materiales subidos");
            }
        }

        public void AssignGrade(Student student, Course course, string fileName, int Grade)
        {
            try{
                Remote.AssignGrade(student, course, fileName, Grade);
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

        public void AddStudentConection(Student student)
        {
            lock (StudentConectionLock)
            {
                try
                {
                    Remote.AddStudentConection(student);
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
                    Remote.DeleteStudentConection(student);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }

        }

        public void ClearStudentConections()
        {
            lock (StudentConectionLock)
            {
                try
                {
                    Remote.ClearStudentSockets();
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
                    return Remote.GetStudentSocket(student);
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


    }
}
