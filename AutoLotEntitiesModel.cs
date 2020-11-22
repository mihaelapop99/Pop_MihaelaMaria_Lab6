namespace Pop_MihaelaMaria_Lab6
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AutoLotEntitiesModel : DbContext
    {
        public AutoLotEntitiesModel()
            : base("name=AutoLotEntitiesModel")
        {
        }

        public virtual DbSet<Customer> Customers { get; set; } //proprietati virtuale
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .HasMany(e => e.Orders)
                .WithOptional(e => e.Customer)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Inventory>()
                .Property(e => e.Make)
                .IsUnicode(false);

            modelBuilder.Entity<Inventory>()
                .Property(e => e.Color)
                .IsUnicode(false);

            modelBuilder.Entity<Inventory>()
                .HasMany(e => e.Orders)
                .WithOptional(e => e.Inventory)
                .WillCascadeOnDelete();
        }
    }
}
