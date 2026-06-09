using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class SessionRegistration
{
    public int SessionId { get; set; }

    public int UserId { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public virtual Session Session { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
