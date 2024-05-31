using System;
using System.Collections.Generic;

namespace BusinessObject.Object;

public partial class LocalNotification
{
    public Guid Id { get; set; }

    public string? Subject { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid? UserId { get; set; }

    public bool IsRead { get; set; }

    public virtual User? User { get; set; }
}
