using System;
using System.Collections.Generic;

namespace BusinessObject.Object;

public partial class Licence
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public bool? IsLicence { get; set; }

    public DateOnly? IssueDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public virtual User? User { get; set; }
}
