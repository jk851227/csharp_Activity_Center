using System;
using System.Linq;
using ActivityCenter.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ActivityCenter.Controllers
{
    public class ActivitiesController : Controller
    {
        private ActivityCenterContext db;
        public ActivitiesController(ActivityCenterContext context)
        {
            db = context;
        }

        private int? uid
        {
            get
            {
                return HttpContext.Session.GetInt32("UserId");
            }
        }

        private bool isLoggedIn
        {
            get
            {
                return uid != null;
            }
        }
        
        [HttpGet("/activity/new")]
        public IActionResult New()
        {
            if (!isLoggedIn)
                return RedirectToAction("Index", "Home");
            
            return View("New");
        }

        [HttpGet("/activity/details/{activityId}")]
        public IActionResult Details(int activityId)
        {
            if (!isLoggedIn)
                return RedirectToAction("Index", "Home");

            Activity currentActivity = db.Activities
                .Include(a => a.Coordinator)
                .Include(a => a.Participants)
                .ThenInclude(p => p.Participant)
                .FirstOrDefault(a => a.ActivityId == activityId);
            
            return View("Details", currentActivity);
        }


        [HttpPost("/activity/create")]
        public IActionResult Create(Activity newActivity)
        {
            if (ModelState.IsValid == false)
                return View("New");

            if (newActivity.Date < DateTime.Today)
            {
                ModelState.AddModelError("Date", "Date cannot be past");
                return View("New");
            }

            newActivity.UserId = (int)uid;
            db.Activities.Add(newActivity);
            db.SaveChanges();
            return RedirectToAction("Dashboard", "Home");
        }

        [HttpPost("/activity/{activityId}/participate")]
        public IActionResult Participate(Participate newParticipant)
        {
            Participate alreadyParticipating = db.Participates.FirstOrDefault(u => u.UserId == uid && u.ActivityId == newParticipant.ActivityId);

            if (alreadyParticipating != null)
                return RedirectToAction("Dashboard", "Home");
            
            newParticipant.UserId = (int)uid;
            db.Participates.Add(newParticipant);
            db.SaveChanges();

            return RedirectToAction("Dashboard", "Home");
        }

        [HttpPost("/activity/{activityId}/unparticipate")]
        public IActionResult UnParticipate(int activityId)
        {
            Participate participant = db.Participates.FirstOrDefault(a => a.ActivityId == activityId && a.UserId == uid);

            if(participant == null)
                return RedirectToAction("Dashboard", "Home");

            db.Participates.Remove(participant);
            db.SaveChanges();

            return RedirectToAction("Dashboard", "Home");
        }

        [HttpPost("/activity/{activityId}/delete")]
        public IActionResult Delete(int activityId)
        {
            Activity currentActivity = db.Activities.FirstOrDefault(a => a.ActivityId == activityId);

            if(currentActivity == null || currentActivity.ActivityId != activityId)
                return RedirectToAction("Dashboard", "Home");

            db.Activities.Remove(currentActivity);
            db.SaveChanges();

            return RedirectToAction("Dashboard", "Home");
        }
    }
}