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
        private QueueLogic QueueLogic { get; set; }

        public TeacherLogic(IRemote remote, QueueLogic QueueLogic)
        {
            this.Remote = remote;
            this.QueueLogic = QueueLogic;
        }

        public void Add(Teacher teacher)
        {
            this.Remote.AddTeacher(teacher);
            QueueLogic.AddToQueue("2", "Se agregó el docente " + teacher.Name + " "
                      + teacher.SurName + " " + teacher.TeacherNum);
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
