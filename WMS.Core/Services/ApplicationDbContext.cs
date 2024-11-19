using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using WMS.Core;
namespace WMS.Core
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var admin = new IdentityRole("Admin");
            admin.NormalizedName = "Admin";

            var warehouseManager = new IdentityRole("WarehouseManager");
            warehouseManager.NormalizedName = "WarehouseManager";

            var warehouseWorker = new IdentityRole("WarehouseWorker");
            warehouseWorker.NormalizedName = "WarehouseWorker";

            builder.Entity<IdentityRole>().HasData(admin, warehouseManager, warehouseWorker);

            // Relationships

            // Address
            // One-to-One
            builder.Entity<Warehouse>().HasOne(w => w.Address).WithOne(a => a.Warehouse).HasForeignKey<Warehouse>(w => w.AddressId);
            builder.Entity<Customer>().HasOne(c => c.Address).WithOne(a => a.Customer).HasForeignKey<Customer>(c => c.AddressId);
            builder.Entity<Order>().HasOne(c => c.Address).WithOne(a => a.Order).HasForeignKey<Order>(o => o.AddressId);
            // Contact Info
            // One-to-One
            builder.Entity<Customer>().HasOne(c => c.ContactInfo).WithOne(ci => ci.Customer).HasForeignKey<Customer>(c => c.ContactInfoId);
            builder.Entity<Supplier>().HasOne(s => s.ContactInfo).WithOne(ci => ci.Supplier).HasForeignKey<Supplier>(s => s.ContactInfoId);
            builder.Entity<Warehouse>().HasOne(w => w.ContactInfo).WithOne(ci => ci.Warehouse).HasForeignKey<Warehouse>(c => c.ContactInfoId);
            // Inventory
            builder.Entity<Inventory>().HasOne(i => i.Product).WithMany(p => p.Inventories).HasForeignKey(i => i.ProductId);
            builder.Entity<Inventory>().HasOne(i => i.Warehouse).WithMany(p => p.Inventories).HasForeignKey(i => i.WarehouseID);
            // Delivery
            builder.Entity<Delivery>().HasOne(d => d.Shipment).WithMany(sh => sh.Deliveries).HasForeignKey(d => d.ShipmentID);
            builder.Entity<Delivery>().HasOne(d => d.Order).WithOne(o => o.Delivery).HasForeignKey<Delivery>(d => d.OrderID);
            // Order
            builder.Entity<Order>().HasOne(o => o.Customer).WithMany(c => c.Orders).HasForeignKey(o => o.CustomerID);
            // Product
            builder.Entity<Product>().HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryID);
            builder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18, 2)");
            // Notification
            builder.Entity<Notification>().HasOne(n => n.ApplicationUser).WithMany(u => u.Notifications).HasForeignKey(n => n.UserId);
            // Tasks
            builder.Entity<WorkerTask>().HasOne(t => t.User).WithMany(u => u.Tasks).HasForeignKey(t => t.UserId);

            // Order Item
            builder.Entity<OrderItem>().HasOne(oi => oi.Order).WithMany(o => o.OrderItems).HasForeignKey(oi => oi.OrderId);
            builder.Entity<OrderItem>().HasOne(oi => oi.Product).WithMany(p => p.OrderItems).HasForeignKey(oi => oi.ProductId);
            builder.Entity<OrderItem>().Property(oi => oi.UnitPrice).HasColumnType("decimal(18, 2)");
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<WorkerTask> Tasks { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}
