using DataBase;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class CourseLogic
    {
        private Information Information { get; set; }

        private readonly object StudentCourseLock = new object();
        private readonly object CourseLock = new object();

        public CourseLogic()
        {

        }
        public CourseLogic(Information information)
        {
            this.Information = information;
        }

        public void AddCourse(Course course)
        {
            lock (CourseLock){
                try
                {
                    this.Information.AddCourse(course);
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
                return this.Information.GetCourses();
            }
        }

        public Course getCourseByCourseNumber(int number)
        {
            lock (CourseLock)
            {
                try
                {
                    return this.Information.GetCourseByCourseNumber(number);
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
                    return Information.GetCourseByCourseName(name);

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
                return this.Information.CourseExistsByName(courseName);
            }
        }
        public void DeleteCourse(int courseIndex)
        {
            lock (StudentCourseLock)
            {
                try
                {
                    this.Information.DeleteCourse(courseIndex);
                }
                catch (Exception e)
                {
                    throw new Exception("No se encuentra curso para borrar");
                }
            }
        }
        public bool existsStudentsAndCourses()
        {
            return this.Information.existsStudentsAndCourses();
        }
        public void AddStudentToCourse(StudentCourse studentCourse)
        {
            lock (StudentCourseLock)
            {
                this.Information.AddStudentCourse(studentCourse);
            }
        }

        public List<Course> GetAvailablesCourses(Student student)
        {
            lock (StudentCourseLock)
            {
                var studentCourses = Information.GetStudentCourses().Where(x => x.Student == student).ToList();
                var coursesOfStudent = new List<Course>();
                foreach (var item in studentCourses)
                {
                    coursesOfStudent.Add(item.Course);
                }

                var allCourses = GetCourses();

                return allCourses.Except(coursesOfStudent).ToList();
            }

        }

        public List<Course> GetEnrolledCourses(Student student)
        {
            lock (StudentCourseLock)
            {
                var studentCourses = Information.GetStudentCourses().Where(x => x.Student == student).ToList();
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
                var studentCourses = Information.GetStudentCourses().Where(x => x.Student == student).ToList();
                foreach (var item in studentCourses)
                {
                    coursesOfStudent.Add(item.Course);
                    var courseName = item.Course.Name;
                    var grade = GetGradeOfCourse(item);
                    var state = "Anotado";
                    listToReturn.Add("Curso: " + courseName +  " , Estado: " + state + " , Nota: " + grade);
                }

                var allCourses = GetCourses();

                foreach(var item in allCourses.Except(coursesOfStudent).ToList())
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
            var files = Information.GetStudentCourseFiles(studentCourse);
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
