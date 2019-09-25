﻿using DataBase;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class StudentLogic
    {
        private Information Information { get; set; }

        public StudentLogic(Information information)
        {
            this.Information = information;
        }

        public string Login(Student student)
        {
            return "";
        }

        public Student AddStudent(Student student)
        {
            try
            {
                this.Information.AddStudent(student);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            return student;
        }  
        
        public List<Student> GetStudents()
        {
            return this.Information.Students;
        }

        public bool StudentExists(int number)
        {
            return this.Information.StudentExists(number);
        }
        public Student GetStudentByStudentNum(int number)
        {
            return this.Information.GetStudentByStudentNum(number);
        }

        public List<Course> GetEnrolledCourses(Student student)
        {
            var studentCourses = Information.GetStudentCourses().Where(x=>x.Student==student).ToList();
            var listToReturn = new List<Course>();
            foreach (var item in studentCourses)
            {
                listToReturn.Add(item.Course); 
            }
            return listToReturn;
        }

        public void AddStudentCourseFile(Student student, Course course, File file)
        {


            try
            {
                //var studentCourse = Information.GetStudentCourses().Find(x => (x.Student == student && x.Course == course));
                foreach (var item in Information.GetStudentCourses())
                {
                    Console.WriteLine("----" + item.Course.Name);
                    Console.WriteLine("----" + item.Student.Name);
                    if (item.Course == course && item.Student == student)
                        item.Files.Add(file);
                }
                
            }
            catch(Exception e)
            {
                //throw new Exception("No se pudo agregar el archivo");
                throw new Exception(e.Message);
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


        public string FileListToResponse(List<File> files)
        {
            string response = "";
            foreach (File file in files)
            {
                response += "Archivo: " + file.Name + "Nota: " + file.Grade + "-";
            }
            return response;
        }


        public bool ValidateStudentNumber(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }

    }
}
