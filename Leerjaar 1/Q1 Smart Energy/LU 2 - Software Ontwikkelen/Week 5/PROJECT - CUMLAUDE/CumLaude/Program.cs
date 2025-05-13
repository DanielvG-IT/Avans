decimal cijfers = 0;

Console.WriteLine("Hoeveel cijfers wil je invullen?");
int aantalCijfers = int.Parse(Console.ReadLine());

for (int i = 1; i <= aantalCijfers; i++)
{
  Console.Write($"Vul getal {i} in: ");
  cijfers += decimal.Parse(Console.ReadLine());
}

decimal uitkomst = cijfers / aantalCijfers;
if (uitkomst >= 8.0M)
{
  Console.WriteLine("De student heeft cumlaude behaald! Het gemiddelde is: " + uitkomst);
}
else
{
  Console.WriteLine("De student heeft GEEN cumlaude behaald! Het gemiddelde is: " + uitkomst);
}