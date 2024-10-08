﻿using System;
using System.Collections.Generic;

namespace BusinessObject.Object;

public partial class Otpverify
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? OtpCode { get; set; }

    public bool IsUsed { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ExpiredAt { get; set; }

    public virtual User User { get; set; } = null!;
}
