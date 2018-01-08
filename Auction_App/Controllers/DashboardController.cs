using System.Linq;
using Auctions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;

namespace Auctions.Controllers
{
    public class DashboardController : Controller
    {
        private AuctionsContext _context;

        public DashboardController(AuctionsContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("dashboard")]
        public IActionResult Dashboard()
        {
            int? LogId = HttpContext.Session.GetInt32("UserId");
            if(LogId == null)
            {
                return Redirect("/");
            }

            ViewBag.LoggedUserId = LogId;
            User user = _context.users.Where(u => u.UserId == LogId).FirstOrDefault();
            ViewBag.User = user;

            List<Auction> auctions = _context.auctions.Include(a => a.Seller).Include(a => a.Bids).ToList();
            foreach(Auction auction in auctions){
                auction.Bids.OrderBy(b => b.Amount).ToList();///doeesn't actually work
                if(auction.EndDate < DateTime.Now){
                    User seller = auction.Seller;
                    User winner = auction.Bids[auction.Bids.Count-1].Bidder;
                    double amount = auction.Bids[auction.Bids.Count-1].Amount;
                    winner.Wallet -= amount;
                    seller.Wallet += amount;
                    _context.auctions.Remove(auction);
                    _context.SaveChanges();
                }
            }
            _context.SaveChanges();
            auctions = _context.auctions.Include(a => a.Seller).Include(a => a.Bids).ToList();


            // ViewBag.Auctions = auctions;
            return View(auctions);
        }
        
        [HttpGet]
        [Route("NewAuction")]
        public IActionResult NewAuction()
        {
            int? LogId = HttpContext.Session.GetInt32("UserId");
            if(LogId == null)
            {
                return Redirect("/");
            }
            ViewBag.LoggedUserId = LogId;
            User user = _context.users.Where(u => u.UserId == LogId).FirstOrDefault();
            ViewBag.User = user;
            return View();
        }

        [HttpPost]
        [Route("CreateAuction")]
        public IActionResult CreateAuction(CreaAuction na)
        {
            int? LogId = HttpContext.Session.GetInt32("UserId");
            if(LogId == null)
            {
                return Redirect("/");
            }
            ViewBag.LoggedUserId = LogId;

            if(ModelState.IsValid)
            {
                Auction cna = new Auction{SellerId = (int)LogId, StartingBid = na.CreStartingBid, EndDate = na.CreEndDate, Product = na.CreProduct, Description = na.CreDescription };
                _context.auctions.Add(cna);
                _context.SaveChanges();

                return Redirect("/dashboard");
            }
            return View("NewAuction");
        }

        [HttpGet]
        [Route("delete/{AId}")]
        public IActionResult CreateAuction(int AId)
        {
            int? LogId = HttpContext.Session.GetInt32("UserId");
            if(LogId == null)
            {
                return Redirect("/");
            }
            ViewBag.LoggedUserId = LogId;

            Auction au = _context.auctions.Where(a => a.AuctionId == AId).Include(a => a.Seller).FirstOrDefault();

            if(au.SellerId == LogId){
                _context.auctions.Remove(au);
                _context.SaveChanges();
            }

            return Redirect("/dashboard");
        }

        [HttpGet]
        [Route("show/{AId}")]
        public IActionResult ShowAuction(int AId)
        {
            int? LogId = HttpContext.Session.GetInt32("UserId");
            if(LogId == null)
            {
                return Redirect("/");
            }
            ViewBag.LoggedUserId = LogId;

            Auction au = _context.auctions.Where(a => a.AuctionId == AId).Include(a => a.Bids).Include(a => a.Seller).FirstOrDefault();
            au.Bids.OrderBy(b => b.Amount).ToList();
            // foreach (Bid bid in au.Bids)
            // {
            //     bid.Bidder
            // }
            TempData["CurrentAuction"] = au;
            if (au.Bids.Count > 0){
                User bidder = _context.users.Where(u => u.UserId == au.Bids[au.Bids.Count-1].BidderId).SingleOrDefault();
            ViewBag.TopBidder = bidder.Username;
            }
            
            ViewBag.CurrentAuction = TempData["CurrentAuction"];
            ViewBag.InvalidBid = TempData["InvalidBid"];
            TempData["InvalidBid"] = false;

            ViewBag.LowerThanHighest = TempData["LowerThanHighest"];
            TempData["LowerThanHighest"] = false;
            return View();
        }

        [HttpPost]
        [Route("Bid")]
        public IActionResult Bid(ViewBid bid)
        {
            int? LogId = HttpContext.Session.GetInt32("UserId");
            if(LogId == null)
            {
               
                return Redirect("/");
            }
            
            User bidder = _context.users.Where(u => u.UserId == LogId).FirstOrDefault();
            Auction au = _context.auctions.Where(a => a.AuctionId == bid.AuctionId).Include(a => a.Bids).FirstOrDefault();
            au.Bids.OrderByDescending(b => b.Amount).ToList();

            if(bid.Amount > bidder.Wallet){
                TempData["InvalidBid"]= true;
                return Redirect($"/show/{bid.AuctionId}");
            } else if (au.Bids.Count > 0  && bid.Amount< au.Bids[au.Bids.Count-1].Amount){
                TempData["LowerThanHighest"]= true;
                return Redirect($"/show/{bid.AuctionId}");
            }

            
            ViewBag.LoggedUserId = LogId;
            Bid nb = new Bid();
            nb.Amount = bid.Amount;
            nb.AuctionId = bid.AuctionId;
            nb.BidderId = (int)LogId;
            nb.Bidder = bidder;
            

            _context.bids.Add(nb);
            _context.SaveChanges();

            return Redirect("/dashboard");
        }
    }
}
