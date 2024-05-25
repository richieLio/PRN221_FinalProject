using System;
using System.Collections.Generic;

namespace BusinessObject.Object;

public partial class Service
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public Guid? HouseId { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<BillService> BillServices { get; set; } = new List<BillService>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual House? House { get; set; }
}
