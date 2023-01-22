using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSClubs.Models;

namespace PSClubs.Controllers
{//This Controller is use to display the country table of the Clubs database with this controller as we are using it with CRUD operation we can manupulate data in the database,it has all four CRUD operation Create delelte update and read
    [Authorize (Roles = "members")]
    public class PSCountryController : Controller
    {
        private readonly PSClubsContext _context;
        // this is the constructer of the controller which creates an instance of the controller whenever this controller is called 
        public PSCountryController(PSClubsContext context)
        {
            _context = context;
        }
        //This method is called whenever no action is defined in the search tab of browser it shows list of all countries no inpout value(id) is required for this method to return view
        // GET: PSCountry
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            
            return View(await _context.Country.OrderBy(n => n.Name).ToListAsync());
        }
        //This method is used to show details of the specific country i.e. row from the country table. the id store the unique row number to show, finnaly returns the list of where province is equals to the id that was sent in Url  
        // GET: PSCountry/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Country
                .FirstOrDefaultAsync(m => m.CountryCode == id);
            if (country == null)
            {
                return NotFound();
            }
            List<Province> province=new List<Province>();
            province = _context.Province.Where(c => c.CountryCode.Equals(id)).ToList();
            ViewBag.province = province;
            return View(country);
        }
        //This method is use to show create page in website where user can add new entries in the database by creating new row by adding the details in the page called by this method,simpky returns the create view as no other view is mentioned in the return statement
        // GET: PSCountry/Create
        public IActionResult Create()
        {
            return View();
        }
        //this is a post method which is used to take all the details user added in the create view and store them to database and redirect user to index page where he can see new entries
        // POST: PSCountry/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CountryCode,Name,PostalPattern,PhonePattern,FederalSalesTax,ProvinceTerminology")] Country country)
        {
            if (ModelState.IsValid)
            {
                _context.Add(country);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }
        //this is a get method which is callee with specific id which identify which row to edit in the database user can use this method which returns a view from where rows can be edited
        // GET: PSCountry/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Country.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }
        // this is simply a post method to post all the values user entered in order to manupulate the data in the Clubs database country table. it always have an id
        // POST: PSCountry/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CountryCode,Name,PostalPattern,PhonePattern,FederalSalesTax,ProvinceTerminology")] Country country)
        {
            if (id != country.CountryCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryExists(country.CountryCode))
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
            return View(country);
        }
        //This is also a get method which is used to delete a row from the database in country table. id is used heree to identify the row that need to be deleted.
        // GET: PSCountry/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Country
                .FirstOrDefaultAsync(m => m.CountryCode == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }
        //This is a post method mean the data filled in a form will be posted by this method to server where in delete view it take parameter in id to delete the specific row from country table.
        // POST: PSCountry/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var country = await _context.Country.FindAsync(id);
            _context.Country.Remove(country);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //this method is called when user tries to enter the country code, as its a prirmary key user cant use more than one country.
        private bool CountryExists(string id)
        {
            return _context.Country.Any(e => e.CountryCode == id);
        }
    }
}
