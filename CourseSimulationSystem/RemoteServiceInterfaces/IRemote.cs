using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RemoteServiceInterfaces
{
    public interface IRemote
    {
        void AddStudent(Student student);
        void AddCourse(Course course);
        Teacher AddTeacher(Teacher teacher);
        void AddStudentCourse(StudentCourse studentCourse);
        bool StudentExists(string studentNum);
        Student GetStudentByStudentNumOrEmail(string studentNum);
        Course GetCourseByCourseNumber(int courseNum);
        Course GetCourseByCourseName(string courseName);
        bool ValidDeletedCourseNumber(int courseNum, string courseName);
        Course GetDeletedCourseByCourseName(string courseName);
        bool CourseExistsByName(string courseName);
        bool CourseExistsByNumber(int courseNumber);
        void DeleteCourse(int courseNum);
        bool existsStudentsAndCourses();
        List<StudentCourse> GetStudentCourses();
        List<File> GetStudentCourseFiles(StudentCourse studentCourse);
        List<Course> GetCourses();
        List<Student> GetStudents();
        bool ExistStudentConection(Student student);
        void AddStudentConection(Student student);
        void DeleteStudentConection(Student student);
        StudentSocket GetStudentSocket(Student student);
        void ClearStudentSockets();
        Teacher GetTeacherByNameOrEmail(string teacher);
        List<Teacher> GetTeachers();
        Boolean LoginTeacher(Teacher teacher);
        void AssignGrade(Student student, Course course, string fileName, int Grade);
        ICollection<String> GetStudentCourseFilesWithoutGrade();
        void AssignGradeByNums(int studentNum, int courseNum, string fileName, int Grade);
        void AddStudentCourseFile(Student student, Course course, File file);
        void setMSMQ(String queuePath);
        void AddLog(Log log);
        List<Log> GetHistoryLog();
    }
}
