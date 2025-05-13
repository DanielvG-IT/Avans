for (int i = 0; i < 66; i++)
{
  int teRaden = new Random().Next(1, 101);
  int geraden = default;
  int pogingen = default;
  int laagste = 1;
  int hoogste = 100;

  geraden = (laagste + hoogste) / 2;
  Console.WriteLine("Frameworky raad het getal 50");

  while (geraden != teRaden)
  {
    if (geraden < teRaden)
    {
      laagste = geraden;
      geraden = (laagste + hoogste) / 2;
      Console.WriteLine($"Frameworky zat eronder. Zij raad nu: {geraden}");
      pogingen++;
    }
    else if (geraden > teRaden)
    {
      hoogste = geraden;
      geraden = (laagste + hoogste) / 2;
      Console.WriteLine($"Frameworky zat erboven. Zij raad nu: {geraden}");
      pogingen++;
    }
  }
  Console.WriteLine($"Frameworky heeft gewonnen! Het getal was {teRaden}. Het duurde {pogingen} aantal pogingen.");
  Console.WriteLine("");
}