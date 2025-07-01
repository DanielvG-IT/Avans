namespace Blazor.Models.Enums
{
  public enum LitterCategory
  {
    Organic,
    Plastic,
    Paper,
    Metal,
    Glass,
    Unknown
  }

  public static partial class CategoryExtensions
  {
    public static string ToFriendlyString(this LitterCategory category)
    {
      return category switch
      {
        LitterCategory.Organic => "organic",
        LitterCategory.Plastic => "plastic",
        LitterCategory.Paper => "paper",
        LitterCategory.Metal => "metal",
        LitterCategory.Glass => "glass",
        _ => "unknown"
      };
    }
  }
}