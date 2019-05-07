using CourseAssignmentManagmentWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace CourseAssignmentManagmentWebsite.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Courses
        public ActionResult Index()
        {
            Course[] courses = (from entity in db.CourseStudents
                                where entity.Student.ApplicationUserId == User.Identity.GetUserId()
                                select entity.Course).ToArray();
            return View();
        }
    }
}