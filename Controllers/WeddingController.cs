using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers
{
    public class WeddingController : Controller
    {
        private int? uid
        {
            get
            {
                return HttpContext.Session.GetInt32("userId");
            }
        }

        private bool isLoggedIn
        {
            get{
                return uid != null;
            }
        }

        private WeddingPlannerContext db;

        public WeddingController(WeddingPlannerContext context)
        {
            db = context;
        }

        [HttpGet("/dashboard")]
        public IActionResult Dashboard()
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.UserId = uid;
            List<Wedding> weddings = db.Weddings
                .Include(wed => wed.WeddingAttenders)
                .ThenInclude(att => att.User)
                .ToList();
            return View("Dashboard", weddings);
        }

        [HttpGet("/new")]
        public IActionResult NewWedding()
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Login", "Home");
            }
            return View("CreateWedding");
        }

        [HttpGet("/{weddingId}/delete")]
        public IActionResult Delete(int weddingId)
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Login", "Home");
            }
            
            Wedding weddingToDelete = db.Weddings.FirstOrDefault(wed => wed.WeddingId == weddingId);
            if (weddingToDelete == null || weddingToDelete.UserId != uid)
            {
                return RedirectToAction("Details", new { weddingId = weddingId });
            }
            db.Weddings.Remove(weddingToDelete);
            db.SaveChanges();
            List <Wedding> weddings = db.Weddings
                .Include(wed => wed.WeddingAttenders)
                .ToList();
            return RedirectToAction("Dashboard", weddings);
        }

        [HttpGet("/{weddingId}/rsvp")]
        public IActionResult RSVP(int weddingId, Attend newAttend)
        {
            newAttend.WeddingId = weddingId;
            newAttend.UserId = (int)uid;
            db.Attendances.Add(newAttend);
            db.SaveChanges();
            return RedirectToAction("WeddingDetails", "Wedding", new { weddingId = weddingId });
        }

        [HttpGet("/{attendId}/unrsvp")]
        public IActionResult UnRSVP(int attendId)
        {
            Attend attendToRemove = db.Attendances.FirstOrDefault(att => att.AttendId == attendId);
            db.Attendances.Remove(attendToRemove);
            db.SaveChanges();
            return RedirectToAction("WeddingDetails", "Wedding", new { weddingId = attendToRemove.WeddingId });
        }

        [HttpPost("/createWedding")]
        public IActionResult CreateWedding(Wedding newWedding)
        {
            if (ModelState.IsValid)
            {
                if (newWedding.Date.Date <= DateTime.Now.Date)
                {
                    ModelState.AddModelError("Date", "You must enter a future date.");
                    return View("CreateWedding");
                }
                newWedding.UserId = (int)uid;
                db.Weddings.Add(newWedding);
                db.SaveChanges();

                List<Wedding> weddings = db.Weddings
                    .Include(wed => wed.WeddingAttenders)
                    .ToList();
                return RedirectToAction("Dashboard", "Wedding", weddings);
            }
            return View("CreateWedding");
        }

        [HttpGet("/{weddingId}/details")]
        public IActionResult WeddingDetails(int weddingId)
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Login", "Home");
            }
            Wedding wedding = db.Weddings
                .Include(wed => wed.WeddingAttenders)
                .ThenInclude(wedAtt => wedAtt.User)
                .FirstOrDefault(wed => wed.WeddingId == weddingId); 
            return View("WeddingDetails", wedding);
        }
    }
}