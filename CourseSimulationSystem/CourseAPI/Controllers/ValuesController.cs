using CourseAPI.Models;
using Entities;
using RemoteServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CourseAPI.Controllers
{
    public class TeachersController : ApiController
    {


        // GET api/teachers
        public IHttpActionResult Get()
        {
            var remote = (IRemote)Activator.GetObject(
                typeof(IRemote),
                "tcp://127.0.0.1:7000/Remote");

            List<TeacherModel> listTeachers= new List<TeacherModel>();
            ICollection<Teacher> teachers = remote.GetTeachers();
            foreach (var teacher in teachers)
            {
                var teacherModel = new TeacherModel()
                {
                    Name = teacher.Name,
                    SurName = teacher.SurName,
                    Mail = teacher.Mail,
                    Password = teacher.Password,
                };
                listTeachers.Add(teacherModel);
            }
            return Ok(listTeachers);
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/teachers
        public void Post([FromBody]TeacherModel teacherModel)
        {
            var remote = (IRemote)Activator.GetObject(
                typeof(IRemote),
                "tcp://127.0.0.1:7000/Remote");

            Teacher teacherToInsert = teacherModel.ToEntity();
            remote.AddTeacher(teacherToInsert);
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
