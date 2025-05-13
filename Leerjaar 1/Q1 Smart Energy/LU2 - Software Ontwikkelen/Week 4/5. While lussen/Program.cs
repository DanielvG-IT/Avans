int hoogsteGetal = 0;
int hoogsteItteratie = 0;

for (var range = 1; range < 101; range++)
{
  int beginGetal = range;
  int aantalPogingen = 0;

  while (beginGetal > 1)
  {
    if (beginGetal % 2 == 0)
    {
      beginGetal = beginGetal / 2;
      aantalPogingen++;
    }
    else if (beginGetal % 2 != 0)
    {
      beginGetal = (beginGetal * 3) + 1;
      aantalPogingen++;
    }
    if (aantalPogingen > hoogsteGetal)
    {
      hoogsteGetal = range;
      hoogsteItteratie = aantalPogingen;
    }
  }
  Console.WriteLine($"Het getal {range} heeft {aantalPogingen} aantal iteraties nodig!");
}
Console.WriteLine("");
Console.WriteLine($"Het hoogste getal: {hoogsteGetal} heeft {hoogsteItteratie} aantal iteraties nodig!");