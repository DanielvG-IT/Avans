Console.WriteLine("Wat is uw naam?");
string Naam = Console.ReadLine();

Console.WriteLine("Wat is uw leeftijd?");
string InputLeeftijd = Console.ReadLine();
int Leeftijd = int.Parse(InputLeeftijd);
int VolgendJaar = Leeftijd + 1;

Console.WriteLine($"De {Leeftijd} jarige {Naam} programmeert in C#");
Console.WriteLine($"Als je {VolgendJaar} bent, ken je meer informatie");