using System;

namespace TralaAI.Website.Services
{
  public interface IExampleService
  {
    string GetGreeting(string name);
  }

  public class ExampleService : IExampleService
  {
    public string GetGreeting(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Name cannot be null or empty.", nameof(name));

      return $"Hello, {name}!";
    }
  }
}