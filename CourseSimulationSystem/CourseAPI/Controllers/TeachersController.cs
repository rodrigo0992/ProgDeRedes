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
        private IRemote Remote { get; set; }

        public TeachersController()
        {
            Remote = (IRemote)Activator.GetObject(
                        typeof(IRemote),
                        "tcp://127.0.0.1:7000/Remote");
        }

        // GET api/teachers
        public IHttpActionResult GetTeachers()
        {

            List<TeacherModel> listTeachers= new List<TeacherModel>();
            ICollection<Teacher> teachers = Remote.GetTeachers();
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

        // POST api/teachers
        public IHttpActionResult AddTeacher([FromBody]TeacherModel teacherModel)
        {

            Teacher teacherToInsert = teacherModel.ToEntity();
            Teacher teacherInserted = Remote.AddTeacher(teacherToInsert);
            return Ok(teacherInserted);
        }

        // POST api/teachers
        public IHttpActionResult Login([FromBody]TeacherModel teacherModel)
        {
            try
            {
                Teacher teacherToLogin = teacherModel.ToEntity();
                Boolean correctLogin = Remote.LoginTeacher(teacherToLogin);
                return Ok(teacherToLogin);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET api/files
        [Route("files")]
        public IHttpActionResult GetFilesToEvaluate()
        {
            try
            {
                List<String> listTeachers = new List<String>();
                ICollection<String> filesToEvaluate = Remote.GetStudentCourseFilesWithoutGrade();

                return Ok(filesToEvaluate);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT
        [Route("files")]
        public IHttpActionResult EvaluateFile(int id, [FromBody]FileToEvaluate fileToEvaluate)
        {
            try
            {
                
                Remote.AssignGradeByNums(fileToEvaluate.StudentNum,fileToEvaluate.CourseNum,fileToEvaluate.FileName, 
                                            fileToEvaluate.Grade);

                return Ok("Material evaluado correctamente");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
