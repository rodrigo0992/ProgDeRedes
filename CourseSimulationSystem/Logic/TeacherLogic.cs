using DataBase;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class TeacherLogic
    {
        private Information Information { get; set; }

        public TeacherLogic(Information information)
        {
            this.Information = information;
        }

        public void Add(Teacher teacher)
        {
            this.Information.AddTeacher(teacher);
        }

        public List<Teacher> GetTeachers()
        {
            return this.Information.GetTeachers();
        }

        public Teacher GetTeacherByNameOrEmail(String teacher)
        {
            return this.Information.GetTeacherByNameOrEmail(teacher);
        }

    }
}
