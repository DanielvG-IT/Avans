Console.Write("Voer een groot getal in: ");
string grootGetal = Console.ReadLine();

int som = default;
foreach (char cijfer in grootGetal)
{
  som += Convert.ToInt32(Convert.ToString(cijfer));
}

Console.WriteLine("");
Console.WriteLine($"De som van de cijfers in het getal is: {som}");