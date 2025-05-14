using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProbabilityTrades.Data.SqlServer.DataModels.ApplicationDataModels;

namespace ProbabilityTrades.Data.SqlServer.DataAccess;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<BlogImage> BlogImages { get; set; }

    public virtual DbSet<CalculateHourlyConfiguration> CalculateHourlyConfigurations { get; set; }

    public virtual DbSet<CalculatePumpConfiguration> CalculatePumpConfigurations { get; set; }

    public virtual DbSet<CalculatePumpOrder> CalculatePumpOrders { get; set; }

    public virtual DbSet<CalculatePumpStatus> CalculatePumpStatuses { get; set; }

    public virtual DbSet<CurrencyHistoryProcess> CurrencyHistoryProcesses { get; set; }

    public virtual DbSet<DiscordNotification> DiscordNotifications { get; set; }

    public virtual DbSet<Log_DiscordBot> Log_DiscordBots { get; set; }

    public virtual DbSet<Log_WorkerService_CalculatePump> Log_WorkerService_CalculatePumps { get; set; }

    public virtual DbSet<Log_WorkerService_Kucoin> Log_WorkerService_Kucoins { get; set; }

    public virtual DbSet<Market> Markets { get; set; }

    public virtual DbSet<MovingAverageConfiguration> MovingAverageConfigurations { get; set; }

    public virtual DbSet<MovingAverageStatus> MovingAverageStatuses { get; set; }

    public virtual DbSet<StripeCustomer> StripeCustomers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserExchange> UserExchanges { get; set; }

    public virtual DbSet<UserPasswordReset> UserPasswordResets { get; set; }

    public virtual DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserSetting> UserSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:ApplicationDatabaseSqlServer");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>(entity =>
        {
            entity.ToTable("Blog");

            entity.HasIndex(e => e.CreatedByUserId, "IX_Blog_CreatedByUserId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.ShortDescription)
                .HasMaxLength(200)
                .HasDefaultValue("");
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Blogs).HasForeignKey(d => d.CreatedByUserId);
        });

        modelBuilder.Entity<BlogImage>(entity =>
        {
            entity.ToTable("BlogImage");

            entity.HasIndex(e => e.BlogId, "IX_BlogImage_BlogId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Url).HasMaxLength(255);

            entity.HasOne(d => d.Blog).WithMany(p => p.BlogImages).HasForeignKey(d => d.BlogId);
        });

        modelBuilder.Entity<CalculateHourlyConfiguration>(entity =>
        {
            entity.HasKey(e => e.Id).IsClustered(false);

            entity.ToTable("CalculateHourlyConfiguration");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.DateLastChanged).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastChangedBy)
                .HasMaxLength(101)
                .HasDefaultValue("");
        });

        modelBuilder.Entity<CalculatePumpConfiguration>(entity =>
        {
            entity.ToTable("CalculatePumpConfiguration");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ATRMultiplier).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BaseCurrency).HasMaxLength(50);
            entity.Property(e => e.CandlestickPattern).HasMaxLength(50);
            entity.Property(e => e.DataSource).HasMaxLength(50);
            entity.Property(e => e.LastChangedBy).HasMaxLength(101);
            entity.Property(e => e.QuoteCurrency).HasMaxLength(50);
            entity.Property(e => e.VolumeMultiplier).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<CalculatePumpOrder>(entity =>
        {
            entity.ToTable("CalculatePumpOrder");

            entity.HasIndex(e => e.UserId, "IX_CalculatePumpOrder_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BaseCurrency).HasMaxLength(50);
            entity.Property(e => e.ClosedAmount).HasColumnType("decimal(24, 4)");
            entity.Property(e => e.ClosedMarketPrice).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.ClosedOrderId).HasMaxLength(50);
            entity.Property(e => e.DataSource).HasMaxLength(50);
            entity.Property(e => e.LastChangedBy)
                .HasMaxLength(101)
                .HasDefaultValue("");
            entity.Property(e => e.OpenedAmount).HasColumnType("decimal(24, 4)");
            entity.Property(e => e.OpenedMarketPrice).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.OpenedOrderId).HasMaxLength(50);
            entity.Property(e => e.OrderQuantity).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.QuoteCurrency).HasMaxLength(50);
            entity.Property(e => e.StopOrderId)
                .HasMaxLength(50)
                .HasDefaultValue("");
            entity.Property(e => e.StopPrice).HasColumnType("decimal(24, 4)");

            entity.HasOne(d => d.User).WithMany(p => p.CalculatePumpOrders).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<CalculatePumpStatus>(entity =>
        {
            entity.ToTable("CalculatePumpStatus");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ATR).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AverageVolume).HasColumnType("decimal(30, 12)");
            entity.Property(e => e.BaseCurrency).HasMaxLength(50);
            entity.Property(e => e.CandlestickPattern).HasMaxLength(50);
            entity.Property(e => e.CurrentCandlePrice).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.CurrentCandleVolume).HasColumnType("decimal(30, 12)");
            entity.Property(e => e.DataSource).HasMaxLength(50);
            entity.Property(e => e.PriceTarget).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.QuoteCurrency).HasMaxLength(50);
            entity.Property(e => e.VolumeTarget).HasColumnType("decimal(30, 12)");
        });

        modelBuilder.Entity<CurrencyHistoryProcess>(entity =>
        {
            entity.ToTable("CurrencyHistoryProcess");

            entity.HasIndex(e => new { e.DataSource, e.BaseCurrency, e.CandlePattern }, "UC_CurrencyHistoryProcess_DataSource_BaseCurrency_CandlePattern").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BaseCurrency).HasMaxLength(50);
            entity.Property(e => e.CandlePattern).HasMaxLength(50);
            entity.Property(e => e.DataSource).HasMaxLength(50);
            entity.Property(e => e.LastChangedBy).HasMaxLength(101);
            entity.Property(e => e.QuoteCurrency).HasMaxLength(50);
        });

        modelBuilder.Entity<DiscordNotification>(entity =>
        {
            entity.HasKey(e => e.Id).IsClustered(false);

            entity.ToTable("DiscordNotification");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Author)
                .HasMaxLength(50)
                .HasDefaultValue("");
            entity.Property(e => e.Channel)
                .HasMaxLength(100)
                .HasDefaultValue("");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Footer)
                .HasMaxLength(200)
                .HasDefaultValue("");
            entity.Property(e => e.Message)
                .HasMaxLength(2000)
                .HasDefaultValue("");
            entity.Property(e => e.NotificationColor)
                .HasMaxLength(7)
                .HasDefaultValue("");
            entity.Property(e => e.NotificationSentAt).HasColumnType("datetime");
            entity.Property(e => e.NotificationType)
                .HasMaxLength(100)
                .HasDefaultValue("");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasDefaultValue("");
        });

        modelBuilder.Entity<Log_DiscordBot>(entity =>
        {
            entity.ToTable("Log_DiscordBot");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<Log_WorkerService_CalculatePump>(entity =>
        {
            entity.ToTable("Log_WorkerService_CalculatePump");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<Log_WorkerService_Kucoin>(entity =>
        {
            entity.ToTable("Log_WorkerService_Kucoin");

            entity.HasIndex(e => e.TimeStamp, "nci_wi_Log_WorkerService_Kucoin_456946C3FA8D9712664A933A7C5AB5FE");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<Market>(entity =>
        {
            entity.ToTable("Market");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DataSource).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.DisplayName).HasMaxLength(50);
            entity.Property(e => e.LastChangedBy).HasMaxLength(101);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<MovingAverageConfiguration>(entity =>
        {
            entity.ToTable("MovingAverageConfiguration");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BaseCurrency).HasMaxLength(50);
            entity.Property(e => e.CandlestickPattern).HasMaxLength(50);
            entity.Property(e => e.DataSource).HasMaxLength(50);
            entity.Property(e => e.LastChangedBy).HasMaxLength(101);
            entity.Property(e => e.QuoteCurrency).HasMaxLength(50);
            entity.Property(e => e.StopLossPercentage).HasColumnType("decimal(8, 4)");
        });

        modelBuilder.Entity<MovingAverageStatus>(entity =>
        {
            entity.ToTable("MovingAverageStatus");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BaseCurrency).HasMaxLength(50);
            entity.Property(e => e.CandlestickPattern).HasMaxLength(50);
            entity.Property(e => e.ClosePrice).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.DataSource).HasMaxLength(50);
            entity.Property(e => e.LongMovingAverage).HasColumnType("decimal(24, 12)");
            entity.Property(e => e.MarketPosition)
                .HasMaxLength(50)
                .HasDefaultValue("");
            entity.Property(e => e.QuoteCurrency).HasMaxLength(50);
            entity.Property(e => e.ShortMovingAverage).HasColumnType("decimal(24, 12)");
        });

        modelBuilder.Entity<StripeCustomer>(entity =>
        {
            entity.HasKey(e => e.Id).IsClustered(false);

            entity.ToTable("StripeCustomer");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(50)
                .HasDefaultValue("");
            entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastSubscriptionDateChange).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.StripeCustomers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StripeCustomer_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.DiscordAccessToken)
                .HasMaxLength(255)
                .HasDefaultValue("");
            entity.Property(e => e.DiscordRefreshToken)
                .HasMaxLength(255)
                .HasDefaultValue("");
            entity.Property(e => e.DiscordUserId)
                .HasMaxLength(50)
                .HasDefaultValue("");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasDefaultValue("");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasDefaultValue("");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasDefaultValue("");
            entity.Property(e => e.PasswordHash).HasMaxLength(64);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(14)
                .HasDefaultValue("");
            entity.Property(e => e.Salt).HasMaxLength(128);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasDefaultValue("");
        });

        modelBuilder.Entity<UserExchange>(entity =>
        {
            entity.ToTable("UserExchange");

            entity.HasIndex(e => e.UserId, "IX_UserExchange_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ApiKey).HasMaxLength(255);
            entity.Property(e => e.ApiPassphrase).HasMaxLength(255);
            entity.Property(e => e.ApiSecret).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.UserExchanges).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserPasswordReset>(entity =>
        {
            entity.ToTable("UserPasswordReset");

            entity.HasIndex(e => e.UserId, "IX_UserPasswordReset_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.User).WithMany(p => p.UserPasswordResets).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserRefreshToken>(entity =>
        {
            entity.ToTable("UserRefreshToken");

            entity.HasIndex(e => e.UserId, "IX_UserRefreshToken_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.RefreshToken).HasMaxLength(256);

            entity.HasOne(d => d.User).WithMany(p => p.UserRefreshTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRole");

            entity.HasIndex(e => e.UserId, "IX_UserRole_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Role).HasMaxLength(20);

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserSetting>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_UserSettings_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.User).WithMany(p => p.UserSettings).HasForeignKey(d => d.UserId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
