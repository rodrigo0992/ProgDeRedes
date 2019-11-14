using Entities;
using Logic;
using RemoteServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteService
{
    public class Remote : MarshalByRefObject, IRemote
    {
        private TeacherLogic TeacherLogic { get; set; }

        public Remote()
        {
            TeacherLogic = null;
        }

        public void SetTeacherLogic(TeacherLogic teacherLogic)
        {
            if (TeacherLogic == null)
                TeacherLogic = teacherLogic;
        }

        public void AddTeacher(Teacher teacher)
        {
            TeacherLogic.Add(teacher);
        }

        public List<Teacher> GetTeachers()
        {
            return TeacherLogic.GetTeachers();
        }
    }
}
