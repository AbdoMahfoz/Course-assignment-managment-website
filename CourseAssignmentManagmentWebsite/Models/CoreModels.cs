using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CourseAssignmentManagmentWebsite.Models
{
    public class Course
    {
        [Key]
        [Display(Name="Course ID")]
        public string Id { get; set; }

        [Required]
        [Display(Name="Course Name")]
        public string Name { get; set; }

        [Required]
        public int ProfessorId { get; set; }
        public virtual Professor Professor { get; set; }
    }
    public class Assignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CourseId { get; set; }
        public virtual Course Course { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public byte[] Statment { get; set; }

        [Required]
        public DateTime DateInitiated { get; set; }

        [Required]
        public DateTime DateDue { get; set; }
    }
    public class Submission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AssignmentId { get; set; }
        public virtual Assignment Assignment { get; set; }

        [Required]
        public int StudentId { get; set; }
        public virtual Student Student { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public byte[] Solution { get; set; }
    }
    public class CourseStudent
    {
        [Key]
        [Column(Order = 0)]
        public int StudentId { get; set; }
        public virtual Student Student { get; set; }

        [Key]
        [Column(Order = 1)]
        public string CourseId { get; set; }
        public virtual Course Course { get; set; }

        [Required]
        public bool IsEnrolled { get; set; }
    }
}