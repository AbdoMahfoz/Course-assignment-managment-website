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
        private bool ValidateAccess<T>(string CourseId)
        {
            string userId = User.Identity.GetUserId();
            if(typeof(T) == typeof(Professor))
            {
                int id = (from e in db.Professors
                          where e.ApplicationUserId == userId
                          select e.Id).Single();
                return (from e in db.Courses
                        where e.ProfessorId == id && e.Id == CourseId
                        select e).Any();
            }
            else
            {
                int studentId = (from e in db.Students
                                 where e.ApplicationUserId == userId
                                 select e.Id).Single();
                return (from e in db.CourseStudents
                        where e.StudentId == studentId && e.CourseId == CourseId && e.IsEnrolled == true
                        select e).Any();
            }
        }
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
        [Authorize(Roles="professor")]
        public ActionResult Create(string Id)
        {
            if (!ValidateAccess<Professor>(Id))
            {
                return RedirectToAction("Index", "Home", new { });
            }
            Assignment ass = new Assignment
            {
                CourseId = Id,
                DateInitiated = DateTime.Now,
            };
            return View(ass);
        }
        [HttpPost]
        [Authorize(Roles = "professor")]
        public ActionResult Create(Assignment ass)
        {
            if(!ValidateAccess<Professor>(ass.CourseId))
            {
                return RedirectToAction("Index", "Home", new { });
            }
            ass.Statement = new byte[ass.file.ContentLength];
            ass.file.InputStream.Read(ass.Statement, 0, ass.Statement.Length);
            ass.StatementType = ass.file.ContentType;
            db.Assignments.Add(ass);
            db.SaveChanges();
            return RedirectToAction("Detail", "Courses", new { Id = ass.CourseId });
        }
    }
}