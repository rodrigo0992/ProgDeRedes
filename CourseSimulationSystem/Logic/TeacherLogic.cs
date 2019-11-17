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
    public class TeacherLogic
    {
        private IRemote Remote { get; set; }

        public TeacherLogic(IRemote remote)
        {
            this.Remote = remote;
        }

        public void Add(Teacher teacher)
        {
            this.Remote.AddTeacher(teacher);
        }

        public List<Teacher> GetTeachers()
        {
            return this.Remote.GetTeachers();
        }

        public Teacher GetTeacherByNameOrEmail(String teacher)
        {
            return this.Remote.GetTeacherByNameOrEmail(teacher);
        }

    }
}
