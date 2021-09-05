using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace EndeavourDemo.Models
{
    public partial class EndeavourContext : DbContext
    {
        public EndeavourContext()
        {
        }

        public EndeavourContext(DbContextOptions<EndeavourContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<PromotionRule> PromotionRules { get; set; }
        public virtual DbSet<TrolleyItem> TrolleyItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("server=tiny-database-instance-1.ccqiuiplz4kp.ap-southeast-2.rds.amazonaws.com;user=admin;password=TINYDBqwaszx12;database=endeavour;treattinyasboolean=true", Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.12-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("product");

                entity.Property(e => e.ProductId)
                    .HasColumnType("int(11)")
                    .HasColumnName("product_id");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("timestamp")
                    .HasColumnName("date_created")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DateModified)
                    .HasColumnType("timestamp")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnName("date_modified")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("type");

                entity.Property(e => e.UnitPrice)
                    .HasPrecision(10, 2)
                    .HasColumnName("unit_price")
                    .HasDefaultValueSql("'99999.00'");
            });

            modelBuilder.Entity<PromotionRule>(entity =>
            {
                entity.ToTable("promotion_rule");

                entity.HasIndex(e => e.ProductId, "fk_product_id_promotion_rule_product");

                entity.Property(e => e.PromotionRuleId)
                    .HasColumnType("int(11)")
                    .HasColumnName("promotion_rule_id");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("timestamp")
                    .HasColumnName("date_created")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DateModified)
                    .HasColumnType("timestamp")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnName("date_modified")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Definition)
                    .HasMaxLength(100)
                    .HasColumnName("definition");

                entity.Property(e => e.EndDate)
                    .HasColumnType("datetime")
                    .HasColumnName("end_date");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("is_active")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Priority)
                    .HasColumnType("int(11)")
                    .HasColumnName("priority")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.ProductId)
                    .HasColumnType("int(11)")
                    .HasColumnName("product_id");

                entity.Property(e => e.Scope)
                    .HasColumnType("int(11)")
                    .HasColumnName("scope")
                    .HasDefaultValueSql("'1'")
                    .HasComment("1 product 2 cross-product 3 trolley");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("start_date");

                entity.Property(e => e.SubtotalExpression)
                    .HasMaxLength(100)
                    .HasColumnName("subtotal_expression");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("type");

                entity.Property(e => e.UnitPriceExpression)
                    .HasMaxLength(100)
                    .HasColumnName("unit_price_expression");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.PromotionRules)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_product_id_promotion_rule_product");
            });

            modelBuilder.Entity<TrolleyItem>(entity =>
            {
                entity.ToTable("trolley_item");

                entity.HasIndex(e => e.ProductId, "fk_product_id_trolley_product");

                entity.Property(e => e.TrolleyItemId)
                    .HasColumnType("int(11)")
                    .HasColumnName("trolley_item_id");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("timestamp")
                    .HasColumnName("date_created")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DateModified)
                    .HasColumnType("timestamp")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnName("date_modified")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ProductId)
                    .HasColumnType("int(11)")
                    .HasColumnName("product_id");

                entity.Property(e => e.Qty)
                    .HasColumnType("int(11)")
                    .HasColumnName("qty")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.UserId)
                    .HasColumnType("int(11)")
                    .HasColumnName("user_id");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.TrolleyItems)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("fk_product_id_trolley_product");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
