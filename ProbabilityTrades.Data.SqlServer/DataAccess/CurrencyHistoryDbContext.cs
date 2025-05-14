using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProbabilityTrades.Data.SqlServer.DataModels.CurrencyHistoryDataModels;

namespace ProbabilityTrades.Data.SqlServer.DataAccess;

public partial class CurrencyHistoryDbContext : DbContext
{
    public CurrencyHistoryDbContext()
    {
    }

    public CurrencyHistoryDbContext(DbContextOptions<CurrencyHistoryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Kucoin> Kucoins { get; set; }

    public virtual DbSet<KucoinArchive> KucoinArchives { get; set; }

    public virtual DbSet<KucoinMovingAverage> KucoinMovingAverages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:CurrencyHistoryDatabaseSqlServer");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Kucoin>(entity =>
        {
            entity.ToTable("Kucoin");

            entity.HasIndex(e => new { e.BaseCurrency, e.QuoteCurrency, e.CandlestickPattern, e.ChartTimeEpoch }, "UC_Kucoin_BaseCurrency_QuoteCurrency_CandlestickPattern_ChartTimeEpoch").IsUnique();

            entity.HasIndex(e => new { e.BaseCurrency, e.CandlestickPattern, e.QuoteCurrency, e.ChartTimeEpoch }, "nci_wi_Kucoin_BE0FE3E9CE0559AB7C21F3D60158065E");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BaseCurrency).HasMaxLength(50);
            entity.Property(e => e.CandlestickPattern).HasMaxLength(50);
            entity.Property(e => e.ChartTimeCST).HasComputedColumnSql("(([ChartTimeUTC] AT TIME ZONE 'Central Standard Time'))", false);
            entity.Property(e => e.ClosingPrice).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.HighestPrice).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.LastChangedBy).HasMaxLength(101);
            entity.Property(e => e.LowestPrice).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.OpeningPrice).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.QuoteCurrency).HasMaxLength(50);
            entity.Property(e => e.Turnover).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.Volume).HasColumnType("decimal(30, 12)");
        });

        modelBuilder.Entity<KucoinArchive>(entity =>
        {
            entity.ToTable("KucoinArchive");

            entity.HasIndex(e => new { e.BaseCurrency, e.QuoteCurrency, e.CandlestickPattern, e.ChartTimeEpoch }, "UC_KucoinArchive_BaseCurrency_QuoteCurrency_CandlestickPattern_ChartTimeEpoch").IsUnique();

            entity.HasIndex(e => new { e.BaseCurrency, e.CandlestickPattern, e.QuoteCurrency, e.ChartTimeEpoch }, "nci_wi_KucoinArchive_BE0FE3E9CE0559AB7C21F3D60158065E");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BaseCurrency).HasMaxLength(50);
            entity.Property(e => e.CandlestickPattern).HasMaxLength(50);
            entity.Property(e => e.ChartTimeCST).HasComputedColumnSql("(([ChartTimeUTC] AT TIME ZONE 'Central Standard Time'))", false);
            entity.Property(e => e.ClosingPrice).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.HighestPrice).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.LastChangedBy).HasMaxLength(101);
            entity.Property(e => e.LowestPrice).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.OpeningPrice).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.QuoteCurrency).HasMaxLength(50);
            entity.Property(e => e.Turnover).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.Volume).HasColumnType("decimal(30, 12)");
        });

        modelBuilder.Entity<KucoinMovingAverage>(entity =>
        {
            entity.ToTable("KucoinMovingAverage");

            entity.HasIndex(e => e.KucoinId, "IX_KucoinMovingAverage_KucoinId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.LastChangedBy).HasMaxLength(101);
            entity.Property(e => e.MovingAverage13).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.MovingAverage144).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.MovingAverage21).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.MovingAverage233).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.MovingAverage3).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.MovingAverage34).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.MovingAverage5).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.MovingAverage50).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.MovingAverage55).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.MovingAverage8).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.MovingAverage89).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.MovingAverage9).HasColumnType("decimal(24, 12)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
