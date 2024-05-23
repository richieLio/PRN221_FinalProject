using System;
using System.Collections.Generic;

namespace BusinessObject.Object;

public partial class Book
{
    public int BookId { get; set; }

    public string BookName { get; set; } = null!;

    public string Authors { get; set; } = null!;

    public decimal Year { get; set; }

    public string? Url { get; set; }
}
