using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fartak.DbModels;

namespace Fartak
{
    public class DataBase : DbContext
    {
        public DataBase(DbContextOptions<DataBase> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Specification> MainSpecifications { get; set; }
        public DbSet<Specification> AllSpecifications { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<PictureNumber> PictureNumbers { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Fartak;Integrated Security=true");
            base.OnConfiguring(optionsBuilder);
        }

         

    }
}
