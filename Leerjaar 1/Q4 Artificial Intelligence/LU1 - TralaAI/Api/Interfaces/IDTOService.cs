using Api.Models.Enums;
using Api.Models.Enums.DTO;

namespace Api.Interfaces
{
    public interface IDTOService
    {
        Category? GetCategory(LitterType? litter);
    }
}