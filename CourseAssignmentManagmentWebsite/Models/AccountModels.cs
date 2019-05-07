using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CourseAssignmentManagmentWebsite.Models
{
    public class Person
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Name { get => FirstName + ' ' + LastName; }
        public virtual ApplicationUser User { get; set; }
        public string ApplicationUserId { get; set; }
    }
    public class Student : Person
    {
    }
    public class Professor : Person
    {
    }
}