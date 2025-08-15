using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fartak.DbModels
{
    public class User
    {
        public int Id { get; set; } // Primary key
        public string Name { get; set; }
        public string Family { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string MelliCode { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Cart { get; set; }
        public bool IsAdmin { get; set; } = false;

    }
}
