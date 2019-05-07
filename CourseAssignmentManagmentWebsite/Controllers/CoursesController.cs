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
        public ActionResult Index(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                var Userid = User.Identity.GetUserId();
                var courses = (from entity in db.CourseStudents
                               where entity.Student.ApplicationUserId == Userid
                               select entity.Course);
                return View(courses);
            }
            return View("Detail", (from entity in db.Courses
                                   where entity.Id == id
                                   select entity).Single());
        }
    }
}