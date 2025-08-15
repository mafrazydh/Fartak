using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fartak.Models.Category
{
    public class GetAllCategory
    {
        public List<string> Categories { set; get; }
        public List<NamesInCategory> NamesCategory { set; get; }
        public List<ModelsInCategory> ModelsCategory { set; get; }
    }
}
