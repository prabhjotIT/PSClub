//using Microsoft.AspNetCore.Mvc;
//using System.ComponentModel.DataAnnotations;
////using PSClubs.Utilities.Validations;
//namespace PSClubs.Models
//{
//   // [ModelMetadataTypeAttribute(typeof(NameAddressMetadata))]
//    //public partial class NameAddress
//    //{
//    //    [Display(Name = "Name")]
//    //    public string FullName
//    //    {
//    //        get
//    //        {
//    //            if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
//    //            {
//    //                return $"{LastName},{FirstName}";//lastname and firstname
//    //            }
//    //            if(!string.IsNullOrEmpty(FirstName) )
//    //            {
//    //                return $"{FirstName}";
//    //            }
//    //            else if(!string.IsNullOrEmpty(LastName))
//    //            {
//    //                return $"{LastName}";
//    //            }
//    //            return "";//empty if nothing is present

//    //        }
//    //    }
//    }
//    public class NameAddressMetadata
//    {

//        public int NameAddressId { get; set; }
//        public string FirstName { get; set; }
//        public string LastName { get; set; }
//        public string CompanyName { get; set; }
//        public string StreetAddress { get; set; }
//        public string City { get; set; }
//        public string PostalCode { get; set; }
//        public string ProvinceCode { get; set; }
//        //[PSEmailAnnotation()]
//        public string Email { get; set; }
//        public string Phone { get; set; }

//    }
//}
