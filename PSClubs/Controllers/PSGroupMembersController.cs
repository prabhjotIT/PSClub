using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSClubs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSClubs.Controllers
{
    public class PSGroupMembersController : Controller
    {
        private readonly PSClubsContext _context;

        public PSGroupMembersController(PSClubsContext context)
        {
            _context = context;
        }

        // GET: PSGroupMembers
        public async Task<IActionResult> Index(string? id)
        {

            if (id != null)
            {
                int ids = int.Parse(id);
                var pSClubsContext = _context.GroupMember.Where(g => g.ArtistIdMember.Equals(ids)).Include(g => g.ArtistIdGroupNavigation).Include(g => g.ArtistIdMemberNavigation).ToListAsync();
                return View(await pSClubsContext);
            }
            else {             
            var pSClubsContext = _context.GroupMember.Include(g => g.ArtistIdGroupNavigation).Include(g => g.ArtistIdMemberNavigation).ToListAsync();
                return View(await pSClubsContext);
            }
            
        }
        //Hint : to makes thing clear i created two views both attached to this one action, and used according to the situation
        public async Task<IActionResult> GroupMemberWithNamesIndex(string? id, string? name)
        {
            if (id == null)
            {
                //check if session exist
                if (HttpContext.Session.GetString(nameof(id)) != null)
                {
                    id = HttpContext.Session.GetString(nameof(id));

                }
                if (HttpContext.Session.GetString(nameof(name)) != null)
                {
                    name = HttpContext.Session.GetString(nameof(name));

                }

            }
            if (id != null)
            {
                
                //var list = _context.Province.Where(cc => cc.CountryCode.Equals(id)).OrderBy(c => c.Name).ToList();

                //save it to the session
                HttpContext.Session.SetString(nameof(id), id.ToString());

                //set both item to session
                HttpContext.Session.SetString(nameof(name), name.ToString());
                HttpContext.Session.SetString(nameof(id), id.ToString());
                //List<GroupMember>artistInTheGroup = _context.GroupMember.Where(a => a.ArtistIdGroup.Equals(id)).ToList();
                //this is to show members of a group
                int id1 = Int32.Parse(id);
                if (_context.GroupMember.Any(a => a.ArtistIdGroup.Equals(id1)))
                {

                    IEnumerable<GroupMemberWithNames> model = null;
                    model = (from a in _context.NameAddress
                             join g in _context.GroupMember on a.NameAddressId equals g.ArtistIdMember
                             where g.ArtistIdGroup == Int32.Parse(id)
                             select new GroupMemberWithNames
                             {
                                 ArtistIdMember = g.ArtistIdGroup,
                                 lastName = a.LastName,
                                 firstName = a.FirstName,
                                 DateJoined = g.DateJoined,
                                 DateLeft = g.DateLeft

                             });
                    return View(model);
                }
                else if(_context.GroupMember.Any(a => a.ArtistIdMember.Equals(id1)))
                {
                    //add a temp data to describe that artist is an individual not a group so here is its history
                    TempData["artistIsNotAGroup"] = "the artist is an individual, not a group, so here’s their historic group memberships.";
                    
                    IEnumerable<GroupsForArtist> model = null;
                    model = (from a in _context.NameAddress
                             join g in _context.GroupMember on a.NameAddressId equals g.ArtistIdGroup
                             where g.ArtistIdMember == Int32.Parse(id)
                             select new GroupsForArtist
                             {
                                 GroupName = a.FirstName,                                 
                                 DateJoined = g.DateJoined,
                                 DateLeft = g.DateLeft

                             });
                    return View("GroupsForArtist", model.OrderBy(d=> d.DateLeft).ThenBy(d => d.DateJoined));
                }
                //since the contol has came here this means artist is neither a group member nor a group : take user to create action of PSGroupMemberController 
                else {
                    TempData["neitherMemberNorGroup"] = "the artist is neither a group nor a group member, but they can become a group.";
                    return RedirectToAction("Create");
                }

            }
            else
            {
                TempData["NoSelectedCountryError"] = "Please select an Artist first.";
                //var pSClubsContext = _context.Province.Include(p => p.CountryCodeNavigation);
                //return View();//display index of PSCountrycontroller with Eroror
                return RedirectToAction("Index", "PSAtrtists");
            }

            //this need to go up in if case when iknow what list members to show i will trim the list accordingly with relevent functions like where and include etc.
            //var pSClubsContext = _context.GroupMember.Include(g => g.ArtistIdGroupNavigation).Include(g => g.ArtistIdMemberNavigation);
            //return View(await pSClubsContext.ToListAsync());
        }


        // GET: PSGroupMembers/Details/5
        public async Task<IActionResult> Details(string ArtistIdGroup, string ArtistIdMember)
        {
            if (ArtistIdGroup == null || ArtistIdMember == null)
            {
                return NotFound();
            }

            var groupMember = await _context.GroupMember
                .Include(g => g.ArtistIdGroupNavigation)
                .Include(g => g.ArtistIdMemberNavigation)
                .Where(a=> a.ArtistIdGroup.Equals(Int32.Parse(ArtistIdGroup)) && a.ArtistIdMember.Equals(Int32.Parse(ArtistIdMember))).ToListAsync();
            if (groupMember == null)
            {
                return NotFound();
            }

            return View(groupMember[0]);
        }

        // GET: PSGroupMembers/Create
        public IActionResult Create()
        {
            TempData["NoSelectedCountryError"] = "Please select an Artist first.";
            filteredList();
            GroupMember gp = new GroupMember();
            gp.DateJoined = DateTime.Now;
            gp.DateLeft = null;
            return View(gp);
        }

        // POST: PSGroupMembers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArtistIdGroup,ArtistIdMember,DateJoined,DateLeft")] GroupMember groupMember)
        {
            if (ModelState.IsValid)
            {
                _context.Add(groupMember);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArtistIdGroup"] = new SelectList(_context.Artist, "ArtistId", "ArtistId", groupMember.ArtistIdGroup);
            ViewData["ArtistIdMember"] = new SelectList(_context.Artist, "ArtistId", "ArtistId", groupMember.ArtistIdMember);
            return View(groupMember);
        }

        // GET: PSGroupMembers/Edit/5
        public async Task<IActionResult> Edit(string ArtistIdGroup, string ArtistIdMember)
        {
            if (ArtistIdGroup == null || ArtistIdMember == null)
            {
                return NotFound();
            }

            var groupMember = await _context.GroupMember
                .Include(g => g.ArtistIdGroupNavigation)
                .Include(g=> g.ArtistIdGroupNavigation.NameAddress)
                .Include(g => g.ArtistIdMemberNavigation)
                .Include(g => g.ArtistIdMemberNavigation.NameAddress)
                .Where(a => a.ArtistIdGroup.Equals(Int32.Parse(ArtistIdGroup)) && a.ArtistIdMember.Equals(Int32.Parse(ArtistIdMember))).ToListAsync();
            if (groupMember == null)
            {
                return NotFound();
            }
            filteredList();
            return View(groupMember[0]);
        }
        private void filteredList()
        {
            //********************----Cover evertyhing in a function have to use it again in create controller as well----*****************
            //var allMembers = _context.Artist.Select(g => g.ArtistId).ToList();//for practice and clearing out things unrealted to current logic
            var onlygroups = _context.GroupMember.Select(g => g.ArtistIdGroup).ToList();//artist who are groups

            var onlygroupmembers = _context.GroupMember.Select(g => g.ArtistIdMember).ToList();//artists who does belong to any group(s)
            //var soloartist = _context.Artist.Where(a => !onlygroupmembers.Contains(a.ArtistId)).ToList();//artist who doesnt belong to any group(s)
            //filter onlyartist with onlygroupmemebrs as we dont need artists which already belong to any group
            var onlyartist = _context.Artist.Include(a => a.NameAddress).Where(a => !onlygroups.Contains(a.ArtistId) && !onlygroupmembers.Contains(a.ArtistId)).ToList();//artist who are not groups
            //ViewData["ArtistIdGroup"] = new SelectList(_context.Artist, "ArtistId", "ArtistId", groupMember[0].ArtistIdGroup);
            //Edit this list so only selected member id is shown
            ViewData["ArtistIdMember"] = new SelectList(_context.Artist, "ArtistId", "FullName", onlyartist);
        }
        // POST: PSGroupMembers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string ArtistIdGroup, string ArtistIdMember, [Bind("ArtistIdGroup,ArtistIdMember,DateJoined,DateLeft")] GroupMember groupMember)
        {
            if (ArtistIdGroup == null || ArtistIdMember == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(groupMember);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupMemberExists(groupMember.ArtistIdGroup))
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
            ViewData["ArtistIdGroup"] = new SelectList(_context.Artist, "ArtistId", "ArtistId", groupMember.ArtistIdGroup);
            ViewData["ArtistIdMember"] = new SelectList(_context.Artist, "ArtistId", "ArtistId", groupMember.ArtistIdMember);
            return View(groupMember);
        }

        // GET: PSGroupMembers/Delete/5
        public async Task<IActionResult> Delete(string ArtistIdGroup, string ArtistIdMember)
        {
            if (ArtistIdGroup == null || ArtistIdMember == null)
            {
                return NotFound();
            }

            var groupMember = await _context.GroupMember
                .Include(g => g.ArtistIdGroupNavigation)
                .Include(g => g.ArtistIdMemberNavigation)
                .Where(a => a.ArtistIdGroup.Equals(Int32.Parse(ArtistIdGroup)) && a.ArtistIdMember.Equals(Int32.Parse(ArtistIdMember))).ToListAsync();
            if (groupMember == null)
            {
                return NotFound();
            }

            return View(groupMember[0]);
        }

        // POST: PSGroupMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string ArtistIdGroup, string ArtistIdMember)
        {
            var groupMember = await _context.GroupMember
                 .Include(g => g.ArtistIdGroupNavigation)
                 .Include(g => g.ArtistIdMemberNavigation)
                 .Where(a => a.ArtistIdGroup.Equals(Int32.Parse(ArtistIdGroup)) && a.ArtistIdMember.Equals(Int32.Parse(ArtistIdMember))).ToListAsync();

            _context.GroupMember.Remove(groupMember[0]);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupMemberExists(int id)
        {
            return _context.GroupMember.Any(e => e.ArtistIdGroup == id);
        }
    }
}
