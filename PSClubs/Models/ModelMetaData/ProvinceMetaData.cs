using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace PSClubs.Models
{
    /// <summary>
    /// meta data for province class in case we need to change something to our views, although we can change these things in province class but when we will do regenration it will remove everything 
    /// </summary>
    //[ModelMetaDataAttribute(typeof(ProvinceMetaData))]
    [ModelMetadataTypeAttribute(typeof(ProvinceMetaData))]
    public partial class Province
    {
        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(Name))
                {
                    return $"({ProvinceCode}) {Name}";
                }
                return $"{Name}";
            }
        }
    }


    public class ProvinceMetaData
    {
        [Display(Name = "Province Code")]
        [Required]
        public string ProvinceCode { get; set; }    
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public string SalesTaxCode { get; set; }
        public double SalesTax { get; set; }
        public bool IncludesFederalTax { get; set; }
        public string FirstPostalLetter { get; set; }

    }
}
