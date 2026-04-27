namespace VehicleParts.Domain.Entities;

public class AppUser : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Enums.UserRole Role { get; set; } = Enums.UserRole.Customer;
}
