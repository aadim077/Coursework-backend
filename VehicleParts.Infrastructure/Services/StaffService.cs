using Microsoft.AspNetCore.Identity;
using VehicleParts.Application.Common;
using VehicleParts.Application.DTOs.Staff;
using VehicleParts.Application.Interfaces;
using VehicleParts.Domain.Entities;
using VehicleParts.Domain.Enums;

namespace VehicleParts.Infrastructure.Services;

internal sealed class StaffService : IStaffService
{
    private readonly UserManager<AppUser> _userManager;

    private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        UserRoles.Admin,
        UserRoles.Staff
    };

    public StaffService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<List<StaffDto>>> GetAllStaffAsync(CancellationToken cancellationToken = default)
    {
        var adminUsers = await _userManager.GetUsersInRoleAsync(UserRoles.Admin);
        var staffUsers = await _userManager.GetUsersInRoleAsync(UserRoles.Staff);

        var allStaff = adminUsers
            .Union(staffUsers, new AppUserComparer())
            .OrderByDescending(u => u.CreatedAt)
            .ToList();

        var staffDtos = new List<StaffDto>();
        foreach (var user in allStaff)
        {
            var roles = await _userManager.GetRolesAsync(user);
            staffDtos.Add(MapToDto(user, roles.FirstOrDefault() ?? string.Empty));
        }

        return Result<List<StaffDto>>.Success(staffDtos);
    }

    public async Task<Result<StaffDto>> GetStaffByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return Result<StaffDto>.Failure("Staff member not found.");

        var roles = await _userManager.GetRolesAsync(user);
        var primaryRole = roles.FirstOrDefault() ?? string.Empty;

        if (!AllowedRoles.Contains(primaryRole))
            return Result<StaffDto>.Failure("User is not a staff member.");

        return Result<StaffDto>.Success(MapToDto(user, primaryRole));
    }

    public async Task<Result<StaffDto>> CreateStaffAsync(CreateStaffRequest request, CancellationToken cancellationToken = default)
    {
        var role = string.IsNullOrWhiteSpace(request.Role) ? UserRoles.Staff : request.Role;

        if (!AllowedRoles.Contains(role))
            return Result<StaffDto>.Failure("Role must be 'Admin' or 'Staff'.");

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            return Result<StaffDto>.Failure("A user with this email already exists.");

        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            return Result<StaffDto>.Failure($"Failed to create staff member: {errors}");
        }

        var roleResult = await _userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
            return Result<StaffDto>.Failure($"Failed to assign role: {errors}");
        }

        return Result<StaffDto>.Success(MapToDto(user, role), "Staff member created successfully.");
    }

    public async Task<Result<StaffDto>> UpdateStaffAsync(string id, UpdateStaffRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return Result<StaffDto>.Failure("Staff member not found.");

        var currentRoles = await _userManager.GetRolesAsync(user);
        var currentRole = currentRoles.FirstOrDefault() ?? string.Empty;

        if (!AllowedRoles.Contains(currentRole))
            return Result<StaffDto>.Failure("User is not a staff member.");


        if (!string.IsNullOrWhiteSpace(request.FullName))
            user.FullName = request.FullName;

        if (request.PhoneNumber != null)
            user.PhoneNumber = request.PhoneNumber;

        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;

        user.UpdatedAt = DateTime.UtcNow;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            return Result<StaffDto>.Failure($"Failed to update staff member: {errors}");
        }


        var newRole = currentRole;
        if (!string.IsNullOrWhiteSpace(request.Role) && !request.Role.Equals(currentRole, StringComparison.OrdinalIgnoreCase))
        {
            if (!AllowedRoles.Contains(request.Role))
                return Result<StaffDto>.Failure("Role must be 'Admin' or 'Staff'.");

            await _userManager.RemoveFromRoleAsync(user, currentRole);
            var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
            if (!roleResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, currentRole);
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                return Result<StaffDto>.Failure($"Failed to update role: {errors}");
            }

            newRole = request.Role;
        }

        return Result<StaffDto>.Success(MapToDto(user, newRole), "Staff member updated successfully.");
    }

    public async Task<Result> ResetStaffPasswordAsync(string id, UpdateStaffPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return Result.Failure("Staff member not found.");

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Any(r => AllowedRoles.Contains(r)))
            return Result.Failure("User is not a staff member.");

        var removeResult = await _userManager.RemovePasswordAsync(user);
        if (!removeResult.Succeeded)
        {
            var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
            return Result.Failure($"Failed to reset password: {errors}");
        }

        var addResult = await _userManager.AddPasswordAsync(user, request.NewPassword);
        if (!addResult.Succeeded)
        {
            var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
            return Result.Failure($"Failed to set new password: {errors}");
        }


        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return Result.Success("Password reset successfully.");
    }

    public async Task<Result> DeleteStaffAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return Result.Failure("Staff member not found.");

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Any(r => AllowedRoles.Contains(r)))
            return Result.Failure("User is not a staff member.");


        user.IsActive = false;
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure($"Failed to deactivate staff member: {errors}");
        }

        return Result.Success("Staff member deactivated successfully.");
    }

    private static StaffDto MapToDto(AppUser user, string role)
    {
        return new StaffDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Role = role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }


    private sealed class AppUserComparer : IEqualityComparer<AppUser>
    {
        public bool Equals(AppUser? x, AppUser? y) => x?.Id == y?.Id;
        public int GetHashCode(AppUser obj) => obj.Id.GetHashCode();
    }
}
