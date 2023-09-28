using System;
using System.Collections.Generic;

namespace Store.Api.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public string? UserName { get; set; }

    public Guid RoleId { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual Role Role { get; set; } = null!;
}
