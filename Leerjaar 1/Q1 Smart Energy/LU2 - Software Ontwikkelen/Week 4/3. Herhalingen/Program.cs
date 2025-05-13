Console.Write("Voer een hele mooie zin in: ");
string ingevuldeZin = Console.ReadLine();
string loweredZin = ingevuldeZin.ToLower();
int aantalKlinkers = 0;

foreach (char letter in loweredZin)
{
  switch (letter)
  {
    case 'a':
    case 'e':
    case 'i':
    case 'o':
    case 'u':
      aantalKlinkers++;
      break;
  }
}

Console.WriteLine($"""De zin "{loweredZin}" bevat {aantalKlinkers} klinkers!""");