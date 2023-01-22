using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSClubs.Models;

namespace PSClubs.Controllers
{//This is the Style controller which is used to show details of style table from Clubs database by defualt index view is called which show all the styles in the file it also have all CRUD operation like other two controllers(instrumet and country) it uses the clubs connection string
    public class PSStyleController : Controller
    {//this is a constructor used to create a context whenever style controller is intialized 
        private readonly PSClubsContext _context;

        public PSStyleController(PSClubsContext context)
        {
            _context = context;
        }
        //this is a get method use to show all the styles in the database this also called by defualt when no action is mentioned.
        // GET: PSStyle
        public async Task<IActionResult> Index()
        {
            return View(await _context.Style.ToListAsync());
        }
        //This is a get method use to show details of a specfic style user clicked on here id is mandatory as it take the primary key for the row to show details this method is called when detail view is clicked
        // GET: PSStyle/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var style = await _context.Style
                .FirstOrDefaultAsync(m => m.StyleName == id);
            if (style == null)
            {
                return NotFound();
            }

            return View(style);
        }
        //this is a get method simply called when create view is called by user.
        // GET: PSStyle/Create
        public IActionResult Create()
        {
            return View();
        }
        //This is a post method to save user entered details when the submit button was clicked it takes the information and create a new row if everythings gets validated
        // POST: PSStyle/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StyleName,Description")] Style style)
        {
            if (ModelState.IsValid)
            {
                _context.Add(style);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(style);
        }
        //this is a get method which gets the details of the row taht need to be edited and fills it with last save information for user to change it. id here is mandatory as it contains the primary key for the row that need to be changed.
        // GET: PSStyle/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var style = await _context.Style.FindAsync(id);
            if (style == null)
            {
                return NotFound();
            }
            return View(style);
        }
        //This is post method its called when user is in edit view and tries to submit the form, it gets validated and changes are reflected in the database on success, and index is called so user can see changes
        // POST: PSStyle/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("StyleName,Description")] Style style)
        {
            if (id != style.StyleName)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(style);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StyleExists(style.StyleName))
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
            return View(style);
        }
        //this is get method to show details of the row that will be deleted as user clicked on the delete link corresponding to a specific row having its primary key stored in ID for this view
        // GET: PSStyle/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var style = await _context.Style
                .FirstOrDefaultAsync(m => m.StyleName == id);
            if (style == null)
            {
                return NotFound();
            }

            return View(style);
        }
        //this is a post method which is used when user confirms the to be deleted row and changes are reflected in the database.
        // POST: PSStyle/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var style = await _context.Style.FindAsync(id);
            _context.Style.Remove(style);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //this method is used when user tries to put same primary key style name in two rows/ entries
        private bool StyleExists(string id)
        {
            return _context.Style.Any(e => e.StyleName == id);
        }
    }
}
