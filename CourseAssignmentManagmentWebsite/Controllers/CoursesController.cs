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
        private bool ValidateAccess<T>(string CourseId)
        {
            string userId = User.Identity.GetUserId();
            if (typeof(T) == typeof(Professor))
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
        private int GetProfessorId()
        {
            string userId = User.Identity.GetUserId();
            return (from e in db.Professors
                    where e.ApplicationUserId == userId
                    select e.Id).Single();
        }
        // GET: Courses
        public ActionResult Index()
        {
            var Userid = User.Identity.GetUserId();
            IQueryable<Pair<bool, Course>> courses = null;
            if(User.IsInRole("student"))
            {
                courses = from entity in db.CourseStudents
                          where entity.Student.ApplicationUserId == Userid
                          select new Pair<bool, Course>
                          {
                              First = entity.IsEnrolled,
                              Second = entity.Course
                          };
            }
            else
            {
                courses = from entity in db.Courses
                          where entity.Professor.ApplicationUserId == Userid
                          select new Pair<bool, Course>
                          {
                              First = true,
                              Second = entity
                          };
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
            if(ValidateAccess<Professor>(Id))
            {
                return View(new CourseSetStudentViewModel
                {
                    CourseID = Id,
                    RegisteredStudents = from entity in db.CourseStudents
                                         where entity.CourseId == Id && entity.IsEnrolled == true
                                         select entity.Student,
                    PendingStudents = from entity in db.CourseStudents
                                      where entity.CourseId == Id && entity.IsEnrolled == false
                                      select entity.Student,
                    LastError = ""
                });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        [Authorize(Roles = "professor")]
        public ActionResult SetStudents(int NewStudentId, string CourseId)
        {
            if(!ValidateAccess<Professor>(CourseId))
            {
                return RedirectToAction("Index");
            }
            var res = new CourseSetStudentViewModel
            {
                CourseID = CourseId,
                RegisteredStudents = from entity in db.CourseStudents
                                     where entity.CourseId == CourseId && entity.IsEnrolled == true
                                     select entity.Student,
                PendingStudents = from entity in db.CourseStudents
                                  where entity.CourseId == CourseId && entity.IsEnrolled == false
                                  select entity.Student,
            };
            if((from entity in db.Students where entity.Id == NewStudentId select entity).Any())
            {
                var enrollment = from entity in db.CourseStudents
                                 where entity.StudentId == NewStudentId && entity.CourseId == CourseId
                                 select entity;
                if (enrollment.Any())
                {
                    if(enrollment.Single().IsEnrolled)
                    {
                        res.LastError = $"Specified student already registered";
                    }
                    else
                    {
                        enrollment.Single().IsEnrolled = true;
                        db.SaveChanges();
                    }
                }
                else
                {
                    db.CourseStudents.Add(new CourseStudent
                    {
                        CourseId = CourseId,
                        StudentId = NewStudentId,
                        IsEnrolled = true
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
            if (User.IsInRole("student"))
            {
                if(!ValidateAccess<Student>(Id))
                {
                    return RedirectToAction("Index");
                }
                int studentId = GetStudentId();
                return View(new CourseDetailViewModel
                {
                    Course = (from entity in db.Courses
                              where entity.Id == Id
                              select entity).Single(),
                    Assignments = from entity in db.Assignments
                                  where entity.CourseId == Id
                                  orderby entity.Id ascending
                                  select new CourseDetailViewModel.Detail
                                  {
                                      Assignment = entity,
                                      Submitted = (from e in db.Submissions
                                                   where e.AssignmentId == entity.Id && e.StudentId == studentId
                                                   select e).Any(),
                                      Grade = (from e in db.Submissions
                                               where e.AssignmentId == entity.Id && e.StudentId == studentId
                                               select e.Grade).FirstOrDefault()
                                  }
                });
            }
            else
            {
                if(!ValidateAccess<Professor>(Id))
                {
                    return RedirectToAction("Index");
                }
                int professorId = GetProfessorId();
                return View(new CourseDetailViewModel
                {
                    Course = (from entity in db.Courses
                              where entity.Id == Id
                              select entity).Single(),
                    Assignments = from entity in db.Assignments
                                  where entity.CourseId == Id
                                  orderby entity.Id ascending
                                  select new CourseDetailViewModel.Detail
                                  {
                                      Assignment = entity,
                                      TotalSubmissions = (from e in db.Submissions
                                                          where e.AssignmentId == entity.Id
                                                          select e).Count(),
                                      GradedSubmissions = (from e in db.Submissions
                                                           where e.AssignmentId == entity.Id && e.Grade != "" && e.Grade != null
                                                           select e).Count(),

                                  }
                });
            }
        }
        [Authorize(Roles="student")]
        public ActionResult Enroll(string Id)
        {
            string userId = User.Identity.GetUserId();
            int studentId = (from e in db.Students
                             where e.ApplicationUserId == userId
                             select e.Id).Single();
            string res = "";
            if (!string.IsNullOrWhiteSpace(Id))
            {
                db.CourseStudents.Add(new CourseStudent
                {
                    StudentId = studentId,
                    CourseId = Id,
                    IsEnrolled = false
                });
                db.SaveChanges();
                string courseName = (from e in db.Courses
                                     where e.Id == Id
                                     select e.Name).Single();
                res = $"Succefully enrolled in {courseName}, please ask your professor to accept your enrollment";
            }
            return View(new Pair<string, IQueryable<Course>>(res, from e in db.Courses
                                                                  where !(from e2 in db.CourseStudents
                                                                          where e2.StudentId == studentId && e2.CourseId == e.Id
                                                                          select e2).Any()
                                                                  select e));
        }
    }
}