using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Entities;

namespace CourseAPI.Models
{
    public class TeacherModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string SurName { get; set; }
        [Required]
        [StringLength(100)]
        public string Mail { get; set; }
        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        public Teacher ToEntity()
        {
            return new Teacher()
            {
                Name = this.Name,
                SurName = this.SurName,
                Mail = this.Mail,
                Password = this.Password,
            };
        }
    }
}