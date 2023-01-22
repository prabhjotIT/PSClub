using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSClubs.Models;
using System.Linq;
using System.Threading.Tasks;
namespace PSClubs.Controllers
{
    public class PSProvincesController : Controller
    {
        private readonly PSClubsContext _context;
        //This is the constructer which gets the context andsave it in a variable which can be used later on for getting lists
        public PSProvincesController(PSClubsContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Index function is loaded whenever no specific action is mentioned i have modified this function so that first it checks if any session code is sent in the url if not found then it tries to find if any session is available and if a session is foudnd 
        /// then country code is loaded from it and coressponding to that country code all its province or state are displayed, however if no session is found and url code variable is also null then a redirectToAction is send to country province index.
        /// </summary>
        /// <param name="ccode"></param>
        /// <param name="cname"></param>
        /// <returns>if session is found a go index with list of corresponding states with coutnry code
        /// otherwise return to index of country code with a message sayingto select a country to see provinces </returns>
        // GET: PSProvinces
        public async Task<IActionResult> Index(string ccode, string cname)//I am using this variable to send data, other way around is using querry string
        {
            if (ccode == null)
            {
                //check if session exist
                if (HttpContext.Session.GetString(nameof(ccode)) != null)
                {
                    var code = HttpContext.Session.GetString(nameof(ccode));
                    ccode = code;
                }
                if (HttpContext.Session.GetString(nameof(cname)) != null)
                {
                    var name = HttpContext.Session.GetString(nameof(cname));
                    cname = name;
                }
                
            }
             if (ccode != null)
            {
                //get the province of country in ccode
                var list = _context.Province.Where(cc => cc.CountryCode.Equals(ccode)).OrderBy(c => c.Name).ToList();
                //save it to the session
                HttpContext.Session.SetString(nameof(ccode), ccode.ToString());
                #region Trying to get country name
                //I was trying to get country name but wasnt sure how to get context for getting all countries
                /*if (cname == null)
                {//this means no country name was sent in the url but yes there is country code so we can get country name
                    //List<Province> countryname = _context.Province.Where(p => p.CountryCode.Equals(ccode)).ToList();//or [0]//first way
                    Province countryname = _context.Province.FirstOrDefault(p => p.CountryCode.Equals(ccode));//better way
                    List < Country > countname= new List<Country>();
                    countname=Models.Country.
                }*/
                #endregion
                //set both item to session

                HttpContext.Session.SetString(nameof(cname), cname.ToString());
                HttpContext.Session.SetString(nameof(ccode), ccode.ToString());

                return View(list);
            }
            else
            {
                TempData["NoSelectedCountryError"] = "Please select a country to see country specific provinces/states";
                //var pSClubsContext = _context.Province.Include(p => p.CountryCodeNavigation);
                //return View();//display index of PSCountrycontroller with Eroror
                return RedirectToAction("Index", "PSCountry");
            }
        }
        /// <summary>
        /// Details is Get function which is use to find the details of a specific province that was requested by user by sending its id by the user from index view, however if the id couldnt be matched with the record a not found view will be called in return
        /// </summary>
        /// <param name="id"></param>
        /// <returns>details of a single province </returns>
        // GET: PSProvinces/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var province = await _context.Province
                .Include(p => p.CountryCodeNavigation)
                .FirstOrDefaultAsync(m => m.ProvinceCode == id);
            if (province == null)
            {
                return NotFound();
            }

            return View(province);
        }
        ///This is simply a get function used to call the create view when a new province has to be created.
        // GET: PSProvinces/Create
        public IActionResult Create()
        {
            ViewData["CountryCode"] = new SelectList(_context.Country, "CountryCode", "CountryCode");
            return View();
        }
        /// <summary>
        /// This function is Post function it is called when the user submit the create form and all the details are brought in here and if they gets validated a new record is entered in the database however, there are two more condition that 
        /// need to be satisfied first, the name should not match with any other province and province code should not match either 
        /// </summary>
        /// <param name="province"></param>
        /// <returns>if data didnt get validated it passed to create view again with province object and if the data gets validated
        /// than a redirectTAction is called which take the index view of province</returns>
        // POST: PSProvinces/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProvinceCode,Name,CountryCode,SalesTaxCode,SalesTax,IncludesFederalTax,FirstPostalLetter")] Province province)
        {
            if (_context.Province.Any(n => n.Name.Equals(province.Name)) )//check that no name should match
            {
                TempData["alreadyExist"] = $"The province code: {province.CountryCode} you entered already exist please try again with different value ";
                return View(province);
            }
            if( _context.Province.Any(c => c.ProvinceCode.Equals(province.ProvinceCode)))//no province should be same
            {
                TempData["alreadyExist"] = $"The name: {province.Name} you entered already exist please try again with different value ";
                return View(province);
            }
            if (ModelState.IsValid)
            {

                _context.Add(province);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryCode"] = new SelectList(_context.Country, "CountryCode", "CountryCode", province.CountryCode);
            return View(province);
        }
        /// <summary>
        /// Edit is a get function which is use to call the edit view when user clicks on the edit hyperlink from the index view 
        /// </summary>
        /// <param name="id">the id contains the unique id that is mandatory in order to find the relevent province to edit it </param>
        /// <returns>if id is not null the previous filled details are filled in relevent fields and edit view is called however if the id was found to be null than not found view is called</returns>
        // GET: PSProvinces/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var province = await _context.Province.FindAsync(id);
            if (province == null)
            {
                return NotFound();
            }
            ViewData["CountryCode"] = new SelectList(_context.Country, "CountryCode", "CountryCode", province.CountryCode);
            return View(province);
        }
        /// <summary>
        /// This is a POST function which is use to call when the user submit the form in edit view all the details ar send in here and after validating if successful the details are updated in the database 
        /// </summary>
        /// <param name="id">unique id to determine the entry in database</param>
        /// <param name="province"> an object of province which contain new entries than need to be validated before updating</param>
        /// <returns>on success index view is called of province</returns>
        // POST: PSProvinces/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ProvinceCode,Name,CountryCode,SalesTaxCode,SalesTax,IncludesFederalTax,FirstPostalLetter")] Province province)
        {
            //something required
            if(string.IsNullOrEmpty(province.Name))
            {
                ModelState.AddModelError("Name", "Name is required");

            }
            if (province.SalesTax < 0)
            {
                ModelState.AddModelError("SalesTax", "sales tax cant be less than zero ");
            }
            //now just add this model validation in a function and you can use it multiple place ie craete or edit 
            if (id != province.ProvinceCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(province);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProvinceExists(province.ProvinceCode))
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
            ViewData["CountryCode"] = new SelectList(_context.Country, "CountryCode", "CountryCode", province.CountryCode);
            return View(province);
        }
        /// <summary>
        /// Details is GET function it is called when the user click on the delete hyper link in index view it has a mandatory id which is used to find the exact entry that need to be deleted from database after confirming but in this funciton only details and confirm button is shown
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: PSProvinces/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var province = await _context.Province
                .Include(p => p.CountryCodeNavigation)
                .FirstOrDefaultAsync(m => m.ProvinceCode == id);
            if (province == null)
            {
                return NotFound();
            }

            return View(province);
        }
        /// <summary>
        /// This is a POST function which is used to delete the entry from the database by matchin the unique id with the entry that need to be deleted.
        /// </summary>
        /// <param name="id">unique id to figure out which vaulue need to be deleted</param>
        /// <returns>return with the province that is deleted back to action view</returns>
        // POST: PSProvinces/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var province = await _context.Province.FindAsync(id);
            _context.Province.Remove(province);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProvinceExists(string id)
        {
            return _context.Province.Any(e => e.ProvinceCode == id);
        }
    }
}
