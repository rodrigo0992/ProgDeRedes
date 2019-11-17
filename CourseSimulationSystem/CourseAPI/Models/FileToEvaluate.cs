using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Entities;

namespace CourseAPI.Models
{
    public class FileToEvaluate
    {
        [Required]
        public int StudentNum { get; set; }
        [Required]
        public int CourseNum { get; set; }
        [Required]
        [StringLength(100)]
        public string FileName { get; set; }
        [Required]
        public int Grade { get; set; }

    }
}