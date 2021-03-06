﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalProject.Models;
using System.Dynamic;
using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Routing;

namespace FinalProject.Controllers
{
    public class UsersController : Controller
    {
        private readonly FinalProjectContext _context;

        public UsersController(FinalProjectContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View();
        }


        // GET: Users/Details/5
        public async Task<IActionResult> Details(string Username, string Pword)
        {
            if (Username == null || Pword == null)
            {
                return Content("Text fields can not be blank and honestly if you made it this far bravo \n" +
                    "Here are some cookies for you" +
                    " https://www.reddit.com/r/food/comments/7prr42/prochef_chocolate_chip_cookies/");
            }

            var users = await _context.Users.Include(x => x.StockPurchaseEntry)
                .SingleOrDefaultAsync(m => m.Username == Username);

            if (users == null)
            {
                    return NotFound();
            }
            else if (users.Pword != Pword)
            {
                return Content("Invalid password");
            }

            ApiDataCalls.curId = users.Id;
            ApiDataCalls.curUser = users.Username;
            ApiDataCalls.itemsToPass = users.StockPurchaseEntry;
            
            return View(users);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Pword,Amount")] Users users) //need to insert Stock purchase entry for this user
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(users);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Create", new RouteValueDictionary(
                        new { controller = "StockPurchaseEntry", action = "Create", id = users.Id }));
                }
                catch (DbUpdateException)
                {
                    return StatusCode(409);
                }
            }

            
            return View();
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            if (users == null)
            {
                return NotFound();
            }
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Username,Pword,Amount")] Users users)
        {
            if (id != users.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(users);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExists(users.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(users);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .SingleOrDefaultAsync(m => m.Id == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var users = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            _context.Users.Remove(users);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }


    }
}
