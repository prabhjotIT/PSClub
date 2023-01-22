using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PSClubs.Models
{
    public class GroupMemberWithNames
    {
        [Required]
        public int ArtistIdMember { get; set; }
        public string lastName = "";
        public string firstName = "";
       // public virtual NameAddress NameAddress { get; set; }
        private string fullName = "";
        public DateTime? DateJoined { get; set; }
        public DateTime? DateLeft { get; set; }
        public String FullName
        {
            get
            {
                fullName = "";
                if (!string.IsNullOrEmpty(lastName))
                {
                    fullName += lastName + " ";
                }
                if (!string.IsNullOrEmpty(firstName))
                {
                    fullName += firstName;
                }
                return fullName;
            }
        }

    }
}
