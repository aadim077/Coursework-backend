using System;
using System.Collections.Generic;
using System.Text;

using VehicleParts.Application.Common;
using VehicleParts.Application.DTOs.Parts;

namespace VehicleParts.Application.Interfaces;

public interface IPartService
{
    Task<Result<IEnumerable<PartResponseDto>>> GetAllAsync();
    Task<Result<PartResponseDto>> GetByIdAsync(int id);
    Task<Result<PartResponseDto>> CreateAsync(CreatePartDto dto);
    Task<Result<PartResponseDto>> UpdateAsync(int id, UpdatePartDto dto);
    Task<Result<bool>> DeleteAsync(int id);
}