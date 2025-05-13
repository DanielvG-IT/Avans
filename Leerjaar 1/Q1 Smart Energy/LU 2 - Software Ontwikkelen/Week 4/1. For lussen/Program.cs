var aantalGeld = 0;

for (var i = 0; i < 12; i++)
{
  aantalGeld += 100;
}

Console.Write($"Je hebt {aantalGeld} gekregen. Ben je blij?: ");
var cool = Console.ReadLine();

if (cool = "Ja")
  Console.WriteLine("Ik zie dat je blij bent.");

if (cool = "Nee")
  Console.WriteLine("Ik zie dat je boos bent");