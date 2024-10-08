﻿using System;
using System.Collections.Generic;

namespace BusinessObject.Object;

public partial class User
{
    public Guid Id { get; set; }

    public string? Email { get; set; }

    public byte[]? Password { get; set; }

    public byte[]? Salt { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? Gender { get; set; }

    public byte[]? Avatar { get; set; }

    public DateTime? Dob { get; set; }

    public string? FullName { get; set; }

    public string? LicensePlates { get; set; }

    public string? Role { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? LastLoggedIn { get; set; }

    public string? CitizenIdNumber { get; set; }

    public Guid? OwnerId { get; set; }

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual ICollection<Licence> Licences { get; set; } = new List<Licence>();

    public virtual ICollection<LocalNotification> LocalNotifications { get; set; } = new List<LocalNotification>();

    public virtual ICollection<Otpverify> Otpverifies { get; set; } = new List<Otpverify>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();

    public virtual ICollection<TransactionHistory> TransactionHistories { get; set; } = new List<TransactionHistory>();

    public virtual ICollection<House> Houses { get; set; } = new List<House>();

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
