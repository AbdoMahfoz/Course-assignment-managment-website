using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CourseAssignmentManagmentWebsite.Models
{
    public class CourseDetailViewModel
    {
        public Course Course { get; set; }
        public IQueryable<Pair<Assignment, bool>> Assignments { get; set; }
    }
    public class CourseSetStudentViewModel
    {
        public IQueryable<Student> RegisteredStudents { get; set; }
        public IQueryable<Student> PendingStudents { get; set; }
        [Display(Name="Student ID")]
        public int NewStudentId { get; set; }
        public string CourseID { get; set; }
        public string LastError { get; set; }
    }
    public class CourseAssignmentViewModel
    {
        public Assignment Assignment { get; set; }
        public IQueryable<Submission> Submissions { get; set; }
    }
}