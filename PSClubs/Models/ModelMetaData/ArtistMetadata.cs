using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PSClubs.Models
{
    [ModelMetadataTypeAttribute(typeof(ArtistMetadata))]
    public partial class Artist
    {
        private string fullName = "";
        public string FullName
        {
            get
            {
                fullName = "";
                if (NameAddress != null)
                {
                    if (!string.IsNullOrEmpty(NameAddress.LastName))
                    {
                        fullName += NameAddress.LastName + " ";
                    }
                    if (!string.IsNullOrEmpty(NameAddress.FirstName))
                    {
                        fullName += NameAddress.FirstName;
                    }
                }
                return fullName;

            }
        }
    }
    public class ArtistMetadata
    {
        [Display(Name = "Full Name")]
        public string FullName { get; }
    }
}
