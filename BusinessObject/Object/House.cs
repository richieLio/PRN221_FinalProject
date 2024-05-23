using System;
using System.Collections.Generic;

namespace BusinessObject.Object;

public partial class House
{
    public Guid Id { get; set; }

    public Guid? OwnerId { get; set; }

    public Guid? StaffId { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public int? RoomQuantity { get; set; }

    public int? AvailableRoom { get; set; }

    public string? Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual User? Owner { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual User? Staff { get; set; }
}
