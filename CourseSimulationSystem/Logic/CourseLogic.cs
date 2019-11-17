using DataBase;
using Entities;
using RemoteServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class CourseLogic
    {
        private IRemote Remote { get; set; }

        private readonly object StudentCourseLock = new object();
        private readonly object CourseLock = new object();

        private QueueLogic QueueLogic { get; set; }

        public CourseLogic()
        {

        }
        public CourseLogic(IRemote remote, QueueLogic queueLogic)
        {
            this.Remote = remote;
            this.QueueLogic = queueLogic;
        }

        public void AddCourse(Course course)
        {
            lock (CourseLock){
                try
                {
                    this.Remote.AddCourse(course);
                    QueueLogic.AddToQueue("3", "Se agregó el curso " + course.Name + " "
                                            + course.CourseNum);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }

        public List<Course> GetCourses()
        {
            lock (CourseLock)
            {
                return this.Remote.GetCourses();
            }
        }

        public Course getCourseByCourseNumber(int number)
        {
            lock (CourseLock)
            {
                try
                {
                    return this.Remote.GetCourseByCourseNumber(number);
                }
                catch (Exception e)
                {
                    throw new Exception("No se puede encontrar el curso con el número: " + number);
                }
            }
        }

        public Course getCourseByCourseName(string name)
        {
            lock (CourseLock) {
                try
                {
                    return Remote.GetCourseByCourseName(name);

                }
                catch (Exception e)
                {
                    throw new Exception("No se puedo obtener el curso de nombre: " + name);
                }
            }
        }

        public bool CourseExistsByName(string courseName)
        {
            lock (CourseLock)
            {
                return this.Remote.CourseExistsByName(courseName);
            }
        }
        public void DeleteCourse(int courseIndex)
        {
            lock (StudentCourseLock)
            {
                try
                {
                    this.Remote.DeleteCourse(courseIndex);
                    QueueLogic.AddToQueue("4", "Se borró el curso " + courseIndex);
                }
                catch (Exception e)
                {
                    throw new Exception("No se encuentra curso para borrar");
                }
            }
        }
        public bool existsStudentsAndCourses()
        {
            return this.Remote.existsStudentsAndCourses();
        }
        public void AddStudentToCourse(StudentCourse studentCourse)
        {
            lock (StudentCourseLock)
            {
                this.Remote.AddStudentCourse(studentCourse);
                QueueLogic.AddToQueue("5", "Se agregó el estudiante " + studentCourse.Student.Name + " "
                      + studentCourse.Student.SurName + " " + studentCourse.Student.StudentNum + " al curso "
                      + studentCourse.Course.Name + " " + studentCourse.Course.CourseNum);
            }
        }

        public List<Course> GetAvailablesCourses(Student student)
        {
            lock (StudentCourseLock)
            {
                var studentCourses = Remote.GetStudentCourses().Where(x => x.Student.StudentNum == student.StudentNum).ToList();
                var coursesOfStudent = new List<Course>();
                foreach (var item in studentCourses)
                {
                    coursesOfStudent.Add(item.Course);
                }

                var allCourses = GetCourses();

                List<Course> difCourses = new List<Course>();
                foreach (var item in allCourses)
                {
                    if (coursesOfStudent.Find(x => x.CourseNum == item.CourseNum) == null)
                    {
                        difCourses.Add(item);
                    }
                }

                return difCourses;
            }

        }

        public List<Course> GetEnrolledCourses(Student student)
        {
            lock (StudentCourseLock)
            {
                var studentCourses = Remote.GetStudentCourses().Where(x => x.Student.StudentNum == student.StudentNum).ToList();
                var listToReturn = new List<Course>();
                foreach (var item in studentCourses)
                {
                    listToReturn.Add(item.Course);
                }
                return listToReturn;
            }
        }

        public List<String> GetCoursesWithDetails(Student student)
        {
            lock (StudentCourseLock)
            {
                var listToReturn = new List<String>();
                var coursesOfStudent = new List<Course>();
                var studentCourses = Remote.GetStudentCourses().Where(x => x.Student.StudentNum == student.StudentNum).ToList();
                foreach (var item in studentCourses)
                {
                    coursesOfStudent.Add(item.Course);
                    var courseName = item.Course.Name;
                    var grade = GetGradeOfCourse(item);
                    var state = "Anotado";
                    listToReturn.Add("Curso: " + courseName +  " , Estado: " + state + " , Nota: " + grade);
                }

                var allCourses = GetCourses();

                List<Course> difCourses = new List<Course>();
                foreach (var item in allCourses)
                {
                    if (coursesOfStudent.Find(x => x.CourseNum == item.CourseNum) == null)
                    {
                        difCourses.Add(item);
                    }
                }

                foreach(var item in difCourses)
                {
                    var courseName = item.Name;
                    var state = "No anotado";
                    listToReturn.Add("Curso: " + courseName + " , Estado: " + state);
                }

                return listToReturn;
            }
        }

        public int GetGradeOfCourse(StudentCourse studentCourse)
        {
            int totalGrade = 0;
            var files = Remote.GetStudentCourseFiles(studentCourse);
            foreach(var item in files)
            {
                totalGrade += item.Grade;
            }
            return totalGrade;
        }

        public string prepareCourseListResponse(List<Course> courses)
        {
            string response = "";
            foreach (Course course in courses)
            {
                response += course.Name + "-";
            }
            return response;
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
