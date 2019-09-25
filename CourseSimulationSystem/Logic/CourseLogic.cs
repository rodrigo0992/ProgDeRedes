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

        public CourseLogic(Information information)
        {
            this.Information = information;
        }

        public Course AddCourse(Course course)
        {
            try
            {
                this.Information.AddCourse(course);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return course;
        }

        public List<Course> GetCourses()
        {
            return this.Information.Courses;
        }

        public Course getCourseByCourseNumber(int number)
        {
            return this.Information.GetCourseByCourseNumber(number);
        }

        public Course getCourseByCourseName(string name)
        {
            try
            {
            return Information.GetCourseByCourseName(name);

            }
            catch (Exception e)
            {
                throw new Exception("No se puedo obtener el curso de nombre: " + name);
            }
        }

        public void DeleteCourse(int courseIndex)
        {
            this.Information.DeleteCourse(courseIndex);
        }
        public bool existsStudentsAndCourses()
        {
            return this.Information.existsStudentsAndCourses();
        }
        public void AddStudentToCourse(StudentCourse studentCourse)
        {
            this.Information.AddStrudentCourse(studentCourse);
        }

        public List<Course> GetAvailablesCourses(Student student)
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

        public string prepareCourseListResponse(List<Course> courses)
        {
            string response = "";
            foreach (Course course in courses)
            {
                response += course.Name + "-";
            }
            return response;
        }
    }
}
