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

  public static class CategoryExtensions
  {
    public static string ToFriendlyString(this LitterCategory category)
    {
      return category switch
      {
        LitterCategory.Organic => "Organic",
        LitterCategory.Plastic => "Plastic",
        LitterCategory.Paper => "Paper",
        LitterCategory.Metal => "Metal",
        LitterCategory.Glass => "Glass",
        _ => "Unknown"
      };
    }
  }
}