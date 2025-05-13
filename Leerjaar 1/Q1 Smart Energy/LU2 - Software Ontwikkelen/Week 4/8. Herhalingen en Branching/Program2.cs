Console.WriteLine("Gemiddelde van niet-negatieve getallen uit ingevoerde reeks");
Console.WriteLine("Hoeveel getallen wil je invoeren?");
int aantalGetallen = int.Parse(Console.ReadLine());
decimal som = default;
decimal bovenNul = default;

for (int i = 1; i <= aantalGetallen; i++)
{
  Console.WriteLine($"Vul getal {i} in");
  int nieuwGetal = Int32.Parse(Console.ReadLine());
  if (nieuwGetal < 0) { continue; }
  som += nieuwGetal;
  bovenNul++;
}

decimal gemiddelde = som / bovenNul;
Console.WriteLine($"Het gemiddelde van de ingevoerde nummers (die boven de 0 lagen!) is: {gemiddelde}");