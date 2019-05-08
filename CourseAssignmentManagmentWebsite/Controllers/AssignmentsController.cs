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
        private int GetStudentId()
        {
            string userId = User.Identity.GetUserId();
            return (from e in db.Students
                    where e.ApplicationUserId == userId
                    select e.Id).Single();
        }
        [Authorize(Roles = "professor")]
        public ActionResult Index(int Id)
        {
            var res = new CourseAssignmentViewModel
            {
                Assignment = (from entity in db.Assignments
                              where entity.Id == Id
                              select entity).Single(),
                Submissions = from entity in db.Submissions
                              where entity.AssignmentId == Id
                              select entity
            };
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
        public ActionResult Create(Assignment ass, HttpPostedFileBase file)
        {
            if(!ValidateAccess<Professor>(ass.CourseId))
            {
                return RedirectToAction("Index", "Home", new { });
            }
            ass.Statement = new byte[file.ContentLength];
            file.InputStream.Read(ass.Statement, 0, ass.Statement.Length);
            ass.StatementType = file.ContentType;
            db.Assignments.Add(ass);
            db.SaveChanges();
            return RedirectToAction("Detail", "Courses", new { Id = ass.CourseId });
        }
        public ActionResult Statement(int Id)
        {
            var assignment = (from e in db.Assignments
                              where e.Id == Id
                              select e).Single();
            if ((User.IsInRole("student") && !ValidateAccess<Student>(assignment.CourseId)) ||
                (User.IsInRole("professor") && !ValidateAccess<Professor>(assignment.CourseId)))
            {
                return RedirectToAction("Index", "Home");
            }
            return File(assignment.Statement, assignment.StatementType);
        }
        [HttpPost]
        [Authorize(Roles="student")]
        public ActionResult Solution(int Id, HttpPostedFileBase file)
        {
            var assignment = (from e in db.Assignments
                              where e.Id == Id
                              select e).Single();
            if(file == null)
            {
                return RedirectToAction("Detail", "Courses", new { Id = assignment.CourseId });
            }
            if(!ValidateAccess<Student>(assignment.CourseId))
            {
                return RedirectToAction("Index", "Home");
            }
            int studentId = GetStudentId();
            if((from e in db.Submissions
                where e.StudentId == studentId && e.AssignmentId == Id
                select e).Any())
            {
                return RedirectToAction("Index", "Courses");
            }
            var submission = new Submission
            {
                StudentId = studentId,
                AssignmentId = Id,
                Solution = new byte[file.ContentLength],
                SolutionType = file.ContentType
            };
            file.InputStream.Read(submission.Solution, 0, file.ContentLength);
            db.Submissions.Add(submission);
            db.SaveChanges();
            return RedirectToAction("Detail", "Courses", new { Id = assignment.CourseId });
        }
    }
}