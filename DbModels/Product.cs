using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fartak.DbModels
{
    public class Product
    {
        public int Id { get; set; } // Primary key
        public string Category { get; set; }
        public string Name { get; set; }
        public string NameForView { get; set; }
        public string Model { get; set; }
        public string Colors { get; set; }
        public string Price { get; set; }
        public string AboutThisProduct { get; set; }
        public string PictureNumbers { get; set; }
        public bool State { get; set; } = true;

    }
    public class Specification
    {
        public int Id { get; set; }
        public string Key { get; set; } // عنوان مشخصه (مثل "RAM", "Storage")
        public string Value { get; set; } // مقدار مشخصه (مثل "8GB", "128GB")

        public int ProductId { get; set; } // کلید خارجی برای ارتباط با گوشی
    }

    public class Comments
    {
        public int Id { get; set; }
        public string Key { get; set; } // عنوان مشخصه (مثل "RAM", "Storage")
        public string Value { get; set; } // مقدار مشخصه (مثل "8GB", "128GB")

        public int ProductId { get; set; } // کلید خارجی برای ارتباط با گوشی
    }

    public class PictureNumber
    {
        public int Id { get; set; }
        public string Number { get; set; }
    }
}
