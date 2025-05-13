List<int> nummers = [];
List<int> andersman = [];

for (int i = 0; i <= 200; i++)
{
  if (i % 5 == 0 && i % 10 == 0) { andersman.Add(i); }
  else if (i % 5 == 0 && i % 10 != 0) { nummers.Add(i); }
  else { continue; }
}

foreach (var nummer in nummers) { System.Console.WriteLine($"Het nummer {nummer} is een veelvoud van 5"); }
System.Console.WriteLine("");
foreach (var nummer in andersman) { System.Console.WriteLine($"Het nummer {nummer} is een veelvoud van 5"); }