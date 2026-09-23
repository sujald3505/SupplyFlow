using Microsoft.EntityFrameworkCore;
using SupplyFlow.Domain.Common;
using SupplyFlow.Domain.Entities;

namespace SupplyFlow.Infrastructure.Persistence;

public class SupplyFlowDbContext : DbContext
{
    public SupplyFlowDbContext(
        DbContextOptions<SupplyFlowDbContext> options)
        : base(options)
    {
    }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();

    public DbSet<Sale> Sales { get; set; } = null!;

    public DbSet<SaleItem> SaleItems { get; set; } = null!;

    public DbSet<WarehouseStock> WarehouseStocks => Set<WarehouseStock>();
    public DbSet<InventoryTransaction> InventoryTransactions
        => Set<InventoryTransaction>();

    public DbSet<PurchaseRequest> PurchaseRequests
        => Set<PurchaseRequest>();

    public DbSet<PurchaseRequestItem> PurchaseRequestItems
        => Set<PurchaseRequestItem>();

    public DbSet<PurchaseOrder> PurchaseOrders
        => Set<PurchaseOrder>();

    public DbSet<SalesReturn> SalesReturns => Set<SalesReturn>();

    public DbSet<SalesReturnItem> SalesReturnItems
        => Set<SalesReturnItem>();

    public DbSet<PurchaseOrderItem> PurchaseOrderItems
        => Set<PurchaseOrderItem>();

    public DbSet<GoodsReceipt> GoodsReceipts
        => Set<GoodsReceipt>();

    public DbSet<GoodsReceiptItem> GoodsReceiptItems
        => Set<GoodsReceiptItem>();

    public DbSet<Customer> Customers => Set<Customer>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Sale>()
           .HasMany(x => x.Items)
           .WithOne(x => x.Sale)
           .HasForeignKey(x => x.SaleId)
           .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SaleItem>()
            .HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Sale>()
    .HasOne(x => x.Customer)
    .WithMany(x => x.Sales)
    .HasForeignKey(x => x.CustomerId)
    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(SupplyFlowDbContext).Assembly);

        modelBuilder.Entity<SalesReturn>()
    .HasMany(x => x.Items)
    .WithOne(x => x.SalesReturn)
    .HasForeignKey(x => x.SalesReturnId)
    .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<SalesReturn>()
            .HasOne(x => x.Sale)
            .WithMany()
            .HasForeignKey(x => x.SaleId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<SalesReturn>()
            .HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<SalesReturn>()
            .HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<SalesReturnItem>()
            .HasOne(x => x.SaleItem)
            .WithMany()
            .HasForeignKey(x => x.SaleItemId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<SalesReturnItem>()
            .HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Global Soft Delete Filter
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(SupplyFlowDbContext)
                    .GetMethod(
                        nameof(SetSoftDeleteFilter),
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Static)
                    ?.MakeGenericMethod(entityType.ClrType);

                method?.Invoke(null, new object[] { modelBuilder });
            }
        }
    }




    private static void SetSoftDeleteFilter<TEntity>(
        ModelBuilder modelBuilder)
        where TEntity : BaseEntity
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(x => !x.IsDeleted);
    }


    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:

                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.IsDeleted = false;

                    break;

                case EntityState.Modified:

                    entry.Entity.UpdatedAt = DateTime.UtcNow;

                    break;

                case EntityState.Deleted:

                    entry.State = EntityState.Modified;

                    entry.Entity.IsDeleted = true;

                    entry.Entity.UpdatedAt = DateTime.UtcNow;

                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }



}