using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSClubs.Models;

namespace PSClubs.Controllers
{//This controller is used to show the instrument table of the clubs database it uses Connection club as a connection string unlike home controller which use defualt connection.
    public class PSInstrumentController : Controller
    {
        private readonly PSClubsContext _context;
        //this is the constructor which is use to create an instance or context for the controller to be used by a class
        public PSInstrumentController(PSClubsContext context)
        {
            _context = context;
        }
        //this a et method to get detais of all instruments in the file, this fnction is also called when no action is mentioned becouse this default and parameter are optional here
        // GET: PSInstrument
        public async Task<IActionResult> Index()
        {
            return View(await _context.Instrument.ToListAsync());
        }
        //This is a Get method its use to get details of a specific row from the instrument table in Clubs database it shows all its values in that row id here is mendatory to work
        // GET: PSInstrument/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrument = await _context.Instrument
                .FirstOrDefaultAsync(m => m.InstrumentId == id);
            if (instrument == null)
            {
                return NotFound();
            }

            return View(instrument);
        }
        //This is also a get method which is used when user tries to create a new entry in database
        // GET: PSInstrument/Create
        public IActionResult Create()
        {
            return View();
        }
        //This is a post method which is called when a form is submitted where user have entered the values for new instrument in create view
        // POST: PSInstrument/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InstrumentId,Name")] Instrument instrument)
        {
            if (ModelState.IsValid)
            {
                _context.Add(instrument);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(instrument);
        }
        //This a get method whic is called when Edit view is used where user can change values (which are allowed) here input tabs are also filled automatically with previous data that was saved for the  specific entry named in ID
        // GET: PSInstrument/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrument = await _context.Instrument.FindAsync(id);
            if (instrument == null)
            {
                return NotFound();
            }
            return View(instrument);
        }
        //Edit here is a post method which take the user entered new values for a specific row in intrument table and change it into the database if all the validation are right otherwise shows error.
        // POST: PSInstrument/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InstrumentId,Name")] Instrument instrument)
        {
            if (id != instrument.InstrumentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instrument);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstrumentExists(instrument.InstrumentId))
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
            return View(instrument);
        }
        //This is  get method used to show delete view in instrument controller where if is used to show one specific row from table that have to be deleted
        // GET: PSInstrument/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrument = await _context.Instrument
                .FirstOrDefaultAsync(m => m.InstrumentId == id);
            if (instrument == null)
            {
                return NotFound();
            }

            return View(instrument);
        }
        //this is a post method where once the user press the confirm button it post the deleted entry to database where the same action is repeated to show user next time all the entries except the one deleted
        // POST: PSInstrument/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instrument = await _context.Instrument.FindAsync(id);
            _context.Instrument.Remove(instrument);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // this is called when user tries to use same primary  key twice for instrument table.
        private bool InstrumentExists(int id)
        {
            return _context.Instrument.Any(e => e.InstrumentId == id);
        }
    }
}
