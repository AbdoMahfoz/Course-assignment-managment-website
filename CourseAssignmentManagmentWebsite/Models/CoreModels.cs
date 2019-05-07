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
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
    public class Assignment
    {
        [Key]
        public int Id { get; set; }
        public virtual Course Course { get; set; }
        public string CourseId { get; set; }
        [Required]
        [DataType(DataType.Upload)]
        public byte[] Statment { get; set; }
    }
    public class Submission
    {
        [Key]
        public int Id { get; set; }
        public virtual Assignment Assignment { get; set; }
        public int AssignmentId { get; set; }
        [Required]
        [DataType(DataType.Upload)]
        public byte[] Solution { get; set; }
    }
    public class CourseAssignment
    {
        public virtual Course Course { get; set; }
        public virtual Assignment Assignment { get; set; }
        [Key]
        [Column(Order = 0)]
        public string CourseId { get; set; }
        [Key]
        [Column(Order = 1)]
        public int AssignmentId { get; set; }
    }
    public class AssignmentSubmission
    {
        public virtual Assignment Assignment { get; set; }
        public virtual Submission Submission { get; set; }
        [Key]
        [Column(Order = 0)]
        public int AssignmentId { get; set; }
        [Key]
        [Column(Order = 1)]
        public int SubmissionId { get; set; }
    }
    public class CourseStudent
    {
        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
        [Key]
        [Column(Order = 0)]
        public int StudentId { get; set; }
        [Key]
        [Column(Order = 1)]
        public string CourseId { get; set; }
    }
    public class CourseProfessor
    {
        public virtual Course Course { get; set; }
        public virtual Professor Professor { get; set; }
        [Key]
        [Column(Order = 0)]
        public int ProfessorId { get; set; }
        [Key]
        [Column(Order = 1)]
        public string CourseId { get; set; }
    }
}