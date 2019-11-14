using Entities;
using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteServiceInterfaces
{
    public interface IRemote
    {
        void SetTeacherLogic(TeacherLogic teacherLogic);
        void AddTeacher(Teacher teacher);
        List<Teacher> GetTeachers();
    }
}
