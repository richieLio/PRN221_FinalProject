﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BusinessObject.Object;

public partial class RmsContext : DbContext
{
    public RmsContext()
    {
    }

    public RmsContext(DbContextOptions<RmsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<BillService> BillServices { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<House> Houses { get; set; }

    public virtual DbSet<Licence> Licences { get; set; }

    public virtual DbSet<LocalNotification> LocalNotifications { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Otpverify> Otpverifies { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<TransactionHistory> TransactionHistories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server =rms.clm6s6e4yupq.ap-southeast-1.rds.amazonaws.com,1433; database = RMS;uid=admin;pwd=12345678;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bill__3214EC07568D6E98");

            entity.ToTable("Bill");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasPrecision(6);
            entity.Property(e => e.Month).HasPrecision(6);
            entity.Property(e => e.PaymentDate).HasPrecision(6);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(19, 4)");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.Bills)
                .HasForeignKey(d => d.CreateBy)
                .HasConstraintName("FK_Bill_User");

            entity.HasOne(d => d.Room).WithMany(p => p.Bills)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_Bill_Room");
        });

        modelBuilder.Entity<BillService>(entity =>
        {
            entity.HasKey(e => new { e.BillId, e.ServiceId }).HasName("PK__BillServ__ADA3476A06665DDD");

            entity.ToTable("BillService");

            entity.Property(e => e.Quantity).HasColumnType("decimal(10, 0)");

            entity.HasOne(d => d.Bill).WithMany(p => p.BillServices)
                .HasForeignKey(d => d.BillId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BillService_ibfk_1");

            entity.HasOne(d => d.Service).WithMany(p => p.BillServices)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("BillService_ibfk_2");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Contract__3214EC07EB46BAC5");

            entity.ToTable("Contract");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasPrecision(6);
            entity.Property(e => e.EndDate).HasPrecision(6);
            entity.Property(e => e.FileUrl)
                .HasMaxLength(250)
                .HasColumnName("FileURL");
            entity.Property(e => e.ImagesUrl)
                .HasMaxLength(250)
                .HasColumnName("ImagesURL");
            entity.Property(e => e.StartDate).HasPrecision(6);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Room).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Contract_Room");
        });

        modelBuilder.Entity<House>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__House__3214EC07DBF815D7");

            entity.ToTable("House");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasPrecision(6);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasMany(d => d.Staff).WithMany(p => p.Houses)
                .UsingEntity<Dictionary<string, object>>(
                    "HouseStaff",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("StaffId")
                        .HasConstraintName("FK_HouseStaff_User"),
                    l => l.HasOne<House>().WithMany()
                        .HasForeignKey("HouseId")
                        .HasConstraintName("FK_HouseStaff_House"),
                    j =>
                    {
                        j.HasKey("HouseId", "StaffId").HasName("PK_HouseStaff_1");
                        j.ToTable("HouseStaff");
                    });
        });

        modelBuilder.Entity<Licence>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ExpiryDate)
                .HasColumnType("datetime")
                .HasColumnName("expiryDate");
            entity.Property(e => e.IsLicence).HasColumnName("isLicence");
            entity.Property(e => e.IssueDate)
                .HasColumnType("datetime")
                .HasColumnName("issueDate");

            entity.HasOne(d => d.User).WithMany(p => p.Licences)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Licences_User");
        });

        modelBuilder.Entity<LocalNotification>(entity =>
        {
            entity.ToTable("LocalNotification");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Subject).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.LocalNotifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_LocalNotification_User");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC071651BB24");

            entity.ToTable("Notification");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasPrecision(6);
            entity.Property(e => e.Subject).HasMaxLength(100);

            entity.HasOne(d => d.House).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.HouseId)
                .HasConstraintName("FK_Notification_House");
        });

        modelBuilder.Entity<Otpverify>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OTPVerif__3214EC07B7CE81D4");

            entity.ToTable("OTPVerify");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasPrecision(6);
            entity.Property(e => e.ExpiredAt).HasPrecision(6);
            entity.Property(e => e.OtpCode).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.Otpverifies)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OTPVerify_User");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Room__3214EC075100575F");

            entity.ToTable("Room");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasPrecision(6);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(19, 4)");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.House).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.HouseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Room_House");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Service__3214EC07F32676CA");

            entity.ToTable("Service");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasPrecision(6);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Price).HasColumnType("decimal(19, 4)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Services)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Service_User");

            entity.HasOne(d => d.House).WithMany(p => p.Services)
                .HasForeignKey(d => d.HouseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Service_House");
        });

        modelBuilder.Entity<TransactionHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transact__3214EC075E7F477B");

            entity.ToTable("TransactionHistory");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IpnUrl).HasMaxLength(255);
            entity.Property(e => e.MerchantRefId).HasMaxLength(50);
            entity.Property(e => e.OrderId).HasMaxLength(50);
            entity.Property(e => e.OrderInfo).HasMaxLength(255);
            entity.Property(e => e.PartnerCode).HasMaxLength(50);
            entity.Property(e => e.PaymentCode).HasMaxLength(50);
            entity.Property(e => e.RedirectUrl).HasMaxLength(255);
            entity.Property(e => e.RequestId).HasMaxLength(50);
            entity.Property(e => e.RequestType).HasMaxLength(50);
            entity.Property(e => e.Signature).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.TransactionHistories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_TransactionHistory_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC0742FDA466");

            entity.ToTable("User");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.Avatar).HasMaxLength(512);
            entity.Property(e => e.CitizenIdNumber).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasPrecision(6);
            entity.Property(e => e.Dob)
                .HasPrecision(6)
                .HasColumnName("DOB");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Gender).HasMaxLength(50);
            entity.Property(e => e.LastLoggedIn).HasPrecision(6);
            entity.Property(e => e.LicensePlates).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(512);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Salt).HasMaxLength(512);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasMany(d => d.Rooms).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRoom",
                    r => r.HasOne<Room>().WithMany()
                        .HasForeignKey("RoomId")
                        .HasConstraintName("FK_UserRoom_Room"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_UserRoom_User"),
                    j =>
                    {
                        j.HasKey("UserId", "RoomId").HasName("PK__UserRoom__94A0AFDFEBD2BC0F");
                        j.ToTable("UserRoom");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
