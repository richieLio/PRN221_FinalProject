using System;
using System.Collections.Generic;

namespace BusinessObject.Object;

public partial class HouseStaff
{
    public Guid HouseId { get; set; }

    public Guid StaffId { get; set; }

    public virtual House House { get; set; } = null!;

    public virtual User Staff { get; set; } = null!;
}
