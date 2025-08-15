using Fartak.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fartak.Models
{
    public class OrderModel
    {
        public Fartak.DbModels.User user { set; get; }
        public List<Order> orders { get; set; }
    }
}
