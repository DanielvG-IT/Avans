// Werkt nog niet, geeft alleen terug dat het getal kleiner is wanneer dat niet zo is!

var rand = new Random();
int teRaden = rand.Next(5, 55);
int euross = rand.Next(100, 200);

Console.WriteLine($"Raad het getal en win {euross} euro!");
for (int i = 1; i < 6; i++)
{
  Console.WriteLine($"Dit is uw {i}de getal");
  int geradenGetal = Convert.ToInt32(Console.ReadLine());
  if (geradenGetal == teRaden)
  {
    Console.WriteLine($"Je hebt het juiste getal geraden! Het getal was {teRaden}");
    break;
  }
  else if (geradenGetal > teRaden)
  {
    Console.WriteLine("Helaas probeert het opnieuw. Het getal is groter");
  }
  else if (geradenGetal < teRaden)
  {
    Console.WriteLine("Helaas probeert het opnieuw. Het getal is kleiner");
  }
  else
  {
    Console.WriteLine("Helaas, er een fout opgetreden!");
  }
}

Console.WriteLine($"Het getal was {teRaden}");