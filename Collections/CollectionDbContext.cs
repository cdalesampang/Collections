using Collection.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection.Data
{
    public class CollectionDbContext : DbContext
    {
        public CollectionDbContext()
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Models.Collection> Collections { get; set; }
        public DbSet<CollectionType> CollectionTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            Database.SetInitializer<CollectionDbContext>(null);
            base.OnModelCreating(modelBuilder);
        }
    }
}
