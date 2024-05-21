using System;
using System.Collections.Generic;

namespace BusinessObject.Object;

public partial class Room
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public Guid? HouseId { get; set; }

    public string? Status { get; set; }

    public decimal? Price { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual House? House { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
