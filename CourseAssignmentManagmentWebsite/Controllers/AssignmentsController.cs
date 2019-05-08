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
    public class AssignmentsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        // GET: Assignments
        public ActionResult Index(int Id)
        {
            var res = new CourseAssignmentViewModel
            {
                Assignment = (from entity in db.Assignments
                              where entity.Id == Id
                              select entity).Single()
            };
            if(User.IsInRole("student"))
            {
                string userId = User.Identity.GetUserId();
                int studentId = (from entity in db.Students
                                 where entity.ApplicationUserId == userId
                                 select entity.Id).Single();
                res.Submissions = from entity in db.Submissions
                                  where entity.AssignmentId == Id && entity.StudentId == studentId
                                  select entity;
            }
            else
            {
                res.Submissions = from entity in db.Submissions
                                  where entity.AssignmentId == Id
                                  select entity;
            }
            return View(res);
        }
        public ActionResult Create(int Id)
        {
            return View();
        }
    }
}