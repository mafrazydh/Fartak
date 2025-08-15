using Fartak.DbModels;
using Fartak.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fartak.Services
{
    public class DashboardViewModel
    {
        public List<Product> Product { get; set; }
        public IGetAdmin IsAdmin { get; set; }
    }

}
