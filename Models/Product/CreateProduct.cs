using Fartak.DbModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fartak.Models.Product
{
    public class CreateProduct
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string NameForView { get; set; }
        public string Model { get; set; }
        public string Colors { get; set; }
        public int Price { get; set; }
        public string AboutThisProduct { get; set; }
        public IFormCollection Pictures { get; set; }
        

    }
}
