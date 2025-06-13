namespace Api.Models.Enums
{
  public enum Category
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
    public static string ToFriendlyString(this Category category)
    {
      return category switch
      {
        Category.Organic => "Organic",
        Category.Plastic => "Plastic",
        Category.Paper => "Paper",
        Category.Metal => "Metal",
        Category.Glass => "Glass",
        _ => "Unknown"
      };
    }
  }
}