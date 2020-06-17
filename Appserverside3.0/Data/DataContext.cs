using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppServerSide.Models;
using Microsoft.EntityFrameworkCore;

namespace AppServerSide.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> dbContextOptions):base(dbContextOptions)
        {

        }

        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
