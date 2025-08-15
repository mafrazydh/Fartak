using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fartak.DbModels
{
    public class Order
    {
        public int Id { get; set; } // Primary key
        public int UserId { get; set; } // Primary key
        public string Orders { get; set; }
        public string State { get; set; }
        public string CreateDate { get; set; }
        public string LastChangeDate { get; set; }


    }
}
