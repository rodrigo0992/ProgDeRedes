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
            return this.Information.GetCourseByCourseName(name);
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

        public string prepareCourseListResponse()
        {
            string response = "";
            foreach (Course course in this.Information.Courses)
            {
                response += course.Name + "-";
            }
            return response;
        }
    }
}
