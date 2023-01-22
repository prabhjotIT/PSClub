using Microsoft.AspNetCore.Mvc;
using PSClubsUtilities;
using PSClubsUtilities.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PSClubs.Models
{
    [ModelMetadataTypeAttribute(typeof(PSNameAddressMetadata))]
    public partial class NameAddress : IValidatableObject
    {
        private readonly PSClubsContext _context = new PSClubsContext();
        // this is the constructer of the controller which creates an instance of the controller whenever this controller is called 
        // public NameAddress(PSClubsContext context)
        // {
        //_context = context;
        //var _context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));
        // }
        [Display(Name = "Name")]
        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
                {
                    return $"{LastName},{FirstName}";//lastname and firstname
                }
                if (!string.IsNullOrEmpty(FirstName))
                {
                    return $"{FirstName}";
                }
                else if (!string.IsNullOrEmpty(LastName))
                {
                    return $"{LastName}";
                }
                return "";//empty if nothing is present

            }

        }
        public string TrimAndConvert(string input)
        {
            if (input == null)
                input = "";
            input = input.Trim();
            return input;
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<Province> pc;
            //manipulating strings by trimming extra spaces
            FirstName = TrimAndConvert(FirstName);
            LastName = TrimAndConvert(LastName);
            CompanyName = TrimAndConvert(CompanyName);
            StreetAddress = TrimAndConvert(StreetAddress);
            City = TrimAndConvert(City);
            PostalCode = TrimAndConvert(PostalCode);
            ProvinceCode = TrimAndConvert(ProvinceCode);
            Email = TrimAndConvert(Email);
            Phone = TrimAndConvert(Phone);

            //using methods to captilize the strings firstname, lastname, companyname, and city
            if (FirstName.Length != 0)
                FirstName = PSStringManipulation.PSCapitalize(FirstName);
            if (LastName.Length != 0)
                LastName = PSStringManipulation.PSCapitalize(LastName);
            if (CompanyName.Length != 0)
                CompanyName = PSStringManipulation.PSCapitalize(CompanyName);
            if (City.Length != 0)
                City = PSStringManipulation.PSCapitalize(City);



            //checking for At least one of FirstName, LastName or CompanyName must be specified.
            if (FirstName.Length == 0 && LastName.Length == 0 && CompanyName.Length == 0)
            {
                yield return new ValidationResult("At least one of First Name, Last Name, or Company Name should be present", new[] { nameof(FirstName), nameof(LastName), nameof(CompanyName) });
            }
            //Province code checking
            if (ProvinceCode != "")
            {
                string exception = "";
                try
                {
                    pc = _context.Province.Where(pc => pc.ProvinceCode.Equals(ProvinceCode)).ToList();
                }
                catch (Exception e)
                {
                    exception = e.GetBaseException().Message.ToString();
                    
                }
                if (exception != "")
                {
                    yield return new ValidationResult($"Cannot process ahead error is found :{exception}", new[] { nameof(ProvinceCode) } );

                }
                pc = _context.Province.Where(pc => pc.ProvinceCode.Equals(ProvinceCode)).ToList();
                if (!pc.Any())
                {
                    yield return new ValidationResult("Province Code, if provided. should be valid!", new[] { nameof(ProvinceCode) });
                }



            }
            //Checking postal code
            if (PostalCode != "")
            {
                pc = _context.Province.Where(pc => pc.ProvinceCode.Equals(ProvinceCode)).ToList();//im repeating my self here
                if (!pc.Any())
                {
                    yield return new ValidationResult("Province code is empty you cannot give postalcode without province code, either fill province code or remove postal code as well", new[] { nameof(PostalCode), nameof(ProvinceCode) });
                }
                //now extract the country record as both province code and postal code are present
                var selectedCountry = _context.Country.Where(c => c.CountryCode.Equals(pc[0].CountryCode)).ToList();

                if (!PSStringManipulation.PSPostalCodeIsValid(PostalCode, selectedCountry[0].PostalPattern))
                {
                    //if you are here this means postal code doesnt matched with the provided province codes-> country codes-> postal patten.
                    yield return new ValidationResult("postal code doesnt matched with the provided province postal pattern", new[] { nameof(PostalCode), nameof(ProvinceCode) });

                }
                //bool isPostacodeValid=PSStringManipulation.PSPostalCodeIsValid(PostalCode, selectedCountry[0].PostalPattern);
                if (selectedCountry[0].Name.Equals("Canada"))
                {

                    var postalCodeChar = PostalCode.ToUpper().ToCharArray();
                    var validpostalCodeForProvince = _context.Province.Where(p => p.FirstPostalLetter.Equals(postalCodeChar[0].ToString())).ToList();
                    if (!validpostalCodeForProvince.Any())
                    {
                        yield return new ValidationResult("first letter of the postal code is not valid for the given province", new[] { nameof(PostalCode) });

                    }

                }
                //checking if there is a space in middle in postal code, if not adding it
                if (PostalCode[3] != ' ')
                {
                    //var postalCodeCharacter = PostalCode.ToCharArray();
                    PostalCode = PostalCode.Substring(0, 3) + " " + PostalCode.Substring(3);

                }
                if (Email == "")
                {
                    if (City == "" || PostalCode == "" || ProvinceCode == "" || StreetAddress == "")
                    {
                        yield return new ValidationResult("All postal informartion is required if email is not provided", new[] { nameof(Email) });

                    }
                }


            }
            // as phone is required no need to check for its empty 

            //reducing phone number to just digits
            var phonestring = PSStringManipulation.PSExtractDigits(Phone);
            if (phonestring.Length != 10)
            {
                yield return new ValidationResult("phonenumber should be exactly 10 digits long", new[] { nameof(Phone) });

            }
            Phone = phonestring.Substring(0, 3) + "-" + phonestring.Substring(3, 3) + "-" + phonestring.Substring(6);
            yield return ValidationResult.Success;
        }
    }
    public class PSNameAddressMetadata
    {
        [Display(Name = "ID")]
        public int NameAddressId { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }
        public string City { get; set; }
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        [Display(Name = "Province Code")]
        public string ProvinceCode { get; set; }
        [PSEmailAnnotation]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Display(Name = "Province Code")]
        public virtual Province ProvinceCodeNavigation { get; set; }
    }

}
