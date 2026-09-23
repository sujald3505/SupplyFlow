using SupplyFlow.Application.DTOs.Unit;

namespace SupplyFlow.Application.Interfaces.Services;

public interface IUnitService
{
    Task<List<UnitDto>> GetAllAsync();

    Task<UnitDto> GetByIdAsync(int id);

    Task<UnitDto> CreateAsync(
        CreateUnitDto request);

    Task<UnitDto> UpdateAsync(
        int id,
        UpdateUnitDto request);
}