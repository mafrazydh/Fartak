using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fartak.DbModels
{
    public class Categories
    {
        public int Id { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Model { get; set; }

    }
}
