using System;
using System.Collections.Generic;

namespace BusinessObject.Object;

public partial class TransactionHistory
{
    public Guid Id { get; set; }

    public string? PartnerCode { get; set; }

    public string? MerchantRefId { get; set; }

    public decimal? Amount { get; set; }

    public string? PaymentCode { get; set; }

    public string? OrderId { get; set; }

    public string? RequestId { get; set; }

    public string? OrderInfo { get; set; }

    public string? RedirectUrl { get; set; }

    public string? IpnUrl { get; set; }

    public string? RequestType { get; set; }

    public string? Signature { get; set; }

    public string? Response { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid? UserId { get; set; }

    public virtual User? User { get; set; }
}
