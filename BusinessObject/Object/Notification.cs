using System;
using System.Collections.Generic;

namespace BusinessObject.Object;

public partial class Notification
{
    public Guid Id { get; set; }

    public string? Content { get; set; }

    public Guid? HouseId { get; set; }

    public virtual House? House { get; set; }
}
