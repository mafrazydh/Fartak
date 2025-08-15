using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fartak.DbModels;

namespace Fartak.Models
{
    public class OrderOdd
    {
        public string Number { get; set; }
        public string Price { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
