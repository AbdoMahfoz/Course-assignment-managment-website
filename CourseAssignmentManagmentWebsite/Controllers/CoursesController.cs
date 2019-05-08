using CourseAssignmentManagmentWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Web.UI;

namespace CourseAssignmentManagmentWebsite.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Courses
        public ActionResult Index()
        {
            var Userid = User.Identity.GetUserId();
            IQueryable<Course> courses = null;
            if(User.IsInRole("student"))
            {
                courses = from entity in db.CourseStudents
                          where entity.Student.ApplicationUserId == Userid
                          select entity.Course;
            }
            else
            {
                courses = from entity in db.Courses
                          where entity.Professor.ApplicationUserId == Userid
                          select entity;
            }
            return View(courses);
        }
        [Authorize(Roles = "professor")]
        public ActionResult Create()
        {
            return View(new Course());
        }
        [Authorize(Roles = "professor")]
        [HttpPost]
        public ActionResult Create(Course course)
        {
            string userId = User.Identity.GetUserId();
            course.ProfessorId = (from entity in db.Professors
                                  where entity.ApplicationUserId == userId
                                  select entity.Id).Single();
            db.Courses.Add(course);
            db.SaveChanges();
            return RedirectToAction("SetStudents", "Courses", new { course.Id });
        }
        [Authorize(Roles = "professor")]
        public ActionResult SetStudents(string Id)
        {
            return View(new CourseSetStudentViewModel
            {
                CourseID = Id,
                Students = from entity in db.CourseStudents
                           where entity.CourseId == Id
                           select entity.Student,
                LastError = ""
            });
        }
        [HttpPost]
        [Authorize(Roles = "professor")]
        public ActionResult SetStudents(int NewStudentId, string CourseId)
        {
            var res = new CourseSetStudentViewModel
            {
                CourseID = CourseId,
                Students = from entity in db.CourseStudents
                           where entity.CourseId == CourseId
                           select entity.Student
            };
            if((from entity in db.Students where entity.Id == NewStudentId select entity).Any())
            {
                if ((from entity in db.CourseStudents where entity.StudentId == NewStudentId && entity.CourseId == CourseId select entity).Any())
                {
                    res.LastError = $"Specified student already registered";
                }
                else
                {
                    db.CourseStudents.Add(new CourseStudent
                    {
                        CourseId = CourseId,
                        StudentId = NewStudentId
                    });
                    db.SaveChanges();
                }
            }
            else
            {
                res.LastError = $"Student of ID = {NewStudentId} doesn't exist";
            }
            return View(res);
        }
        public ActionResult Detail(string Id)
        {
            return View(new CourseDetailViewModel
            {
                Course = (from entity in db.Courses
                          where entity.Id == Id
                          select entity).Single(),
                Assignments = from entity in db.Assignments
                              where entity.CourseId == Id
                              orderby entity.Id ascending
                              select new Pair<Assignment, bool>
                              {
                                  First = entity,
                                  Second = (from e in db.Submissions
                                            where e.AssignmentId == entity.Id
                                            select e).Any()
                              }
            });
        }
    }
}