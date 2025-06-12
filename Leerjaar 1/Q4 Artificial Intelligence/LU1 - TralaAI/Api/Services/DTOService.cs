using Api.Interfaces;
using Api.Models.Enums;
using Api.Models.Enums.DTO;

namespace Api.Services
{
  public class DTOService : IDTOService
  {
    // This service can be used to convert LitterType to Category
    // and potentially other DTO-related operations in the future.
    public Category? GetCategory(LitterType? litter)
    {
      if (litter == null)
        return null;

      return litter switch
      {
        LitterType.AluminiumFoil or LitterType.BottleCap or LitterType.Can or LitterType.PopTab => Category.Metal,
        LitterType.Bottle or LitterType.BrokenGlass => Category.Glass,
        LitterType.Carton or LitterType.Paper => Category.Paper,
        LitterType.Cigarette => Category.Organic,
        LitterType.Cup or LitterType.Lid or LitterType.OtherPlastic or LitterType.PlasticBagWrapper or LitterType.PlasticContainer or LitterType.Straw or LitterType.StyrofoamPiece => Category.Plastic,
        _ => Category.Unknown,
      };
    }
  }
}