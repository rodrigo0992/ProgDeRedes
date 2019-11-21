﻿using CourseAPI.Models;
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
            String remoteRoute = System.Configuration.ConfigurationManager.AppSettings["remoteRoute"];
            Remote = (IRemote)Activator.GetObject(
                        typeof(IRemote),
                        "tcp://" + remoteRoute);
        }

        // GET api/teachers
        [HttpGet]
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
        [HttpPost]
        public IHttpActionResult AddTeacher([FromBody]TeacherModel teacherModel)
        {

            Teacher teacherToInsert = teacherModel.ToEntity();
            Teacher teacherInserted = Remote.AddTeacher(teacherToInsert);
            return Ok(teacherInserted);
        }

        // POST api/teachers/login
        [HttpPost]
        [Route("api/Teachers/login")]
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

        // GET api/teachers/files
        [HttpGet]
        [Route("api/Teachers/files")]
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

        // PUT api/teachers/files
        [HttpPut]
        [Route("api/Teachers/files")]
        public IHttpActionResult EvaluateFile([FromBody]FileToEvaluate fileToEvaluate)
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
