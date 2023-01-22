using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSClubs.Models;

namespace PSClubs.Controllers
{
    public class PSNameAddressesController : Controller
    {
        private readonly PSClubsContext _context;

        public PSNameAddressesController(PSClubsContext context)
        {
            _context = context;
        }

        // GET: PSNameAddresses
        public async Task<IActionResult> Index()
        {
            var pSClubsContext = _context.NameAddress.Include(n => n.ProvinceCodeNavigation);
            return View(await pSClubsContext.ToListAsync());
        }

        // GET: PSNameAddresses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nameAddress = await _context.NameAddress
                .Include(n => n.ProvinceCodeNavigation)
                .FirstOrDefaultAsync(m => m.NameAddressId == id);
            if (nameAddress == null)
            {
                return NotFound();
            }

            return View(nameAddress);
        }

        // GET: PSNameAddresses/Create
        public IActionResult Create()
        {
            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode");
            return View();
        }

        // POST: PSNameAddresses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NameAddressId,FirstName,LastName,CompanyName,StreetAddress,City,PostalCode,ProvinceCode,Email,Phone")] NameAddress nameAddress)
        {
            try { 
            
            if (ModelState.IsValid)
            {
                _context.Add(nameAddress);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode", nameAddress.ProvinceCode);
            return View(nameAddress);
            }
            catch(Exception e)
            {
                ModelState.AddModelError("", $"error inserting new order: {e.GetBaseException().Message}");
            }
            Create();  // call setup action to initialise ViewBag for dropdowns
            return View(nameAddress);
        }

        // GET: PSNameAddresses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nameAddress = await _context.NameAddress.FindAsync(id);
            if (nameAddress == null)
            {
                return NotFound();
            }
            var provincename = _context.Province.Select(n => n.Name).ToList();


            ViewData["ProvinceName"] = new SelectList(_context.Province.OrderBy(n=>n.Name), "ProvinceCode", "Name", nameAddress.ProvinceCode);

            //ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode", nameAddress.ProvinceCode);
            return View(nameAddress);
        }

        // POST: PSNameAddresses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NameAddressId,FirstName,LastName,CompanyName,StreetAddress,City,PostalCode,ProvinceCode,Email,Phone")] NameAddress nameAddress)
        {
            if (id != nameAddress.NameAddressId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nameAddress);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NameAddressExists(nameAddress.NameAddressId))
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
            ViewData["ProvinceName"] = new SelectList(_context.Province.OrderBy(n => n.Name), "ProvinceCode", "Name", nameAddress.ProvinceCode);
            return View(nameAddress);
        }

        // GET: PSNameAddresses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nameAddress = await _context.NameAddress
                .Include(n => n.ProvinceCodeNavigation)
                .FirstOrDefaultAsync(m => m.NameAddressId == id);
            if (nameAddress == null)
            {
                return NotFound();
            }

            return View(nameAddress);
        }

        // POST: PSNameAddresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nameAddress = await _context.NameAddress.FindAsync(id);
            _context.NameAddress.Remove(nameAddress);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NameAddressExists(int id)
        {
            return _context.NameAddress.Any(e => e.NameAddressId == id);
        }
    }
}
