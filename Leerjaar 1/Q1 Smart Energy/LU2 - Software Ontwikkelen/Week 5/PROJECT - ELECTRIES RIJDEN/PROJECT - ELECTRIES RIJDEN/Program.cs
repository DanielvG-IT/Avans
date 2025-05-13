using System;

internal class Program
{
  private static void Main(string[] args)
  {
    Console.WriteLine("Vul het verbruik in (x/100km)");
    float verbruik = float.Parse(Console.ReadLine());
    Console.WriteLine("Wat is de accucapaciteit van de auto?");
    float accucapaciteit = float.Parse(Console.ReadLine());
    Console.WriteLine("Vul de gewenste afstand!");
    float afstand = float.Parse(Console.ReadLine());

    bool afstandsberekening(float verbruik, float accucapaciteit, float afstand)
    {
      float maxRange = accucapaciteit / verbruik * 100;
      if (maxRange < afstand)
      { return false; }
      else
      { return true; }
    }

    Console.WriteLine($"Kom ik aan: {afstandsberekening(verbruik, accucapaciteit, afstand)}");
  }
}