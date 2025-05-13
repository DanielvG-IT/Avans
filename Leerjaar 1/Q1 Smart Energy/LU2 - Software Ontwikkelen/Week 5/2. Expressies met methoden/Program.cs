using System;

class Program
{
  enum seizoen { Lente = 1, Zomer, Herfst, Winter }

  static void Main()
  {
    Console.Write("Hoeveel kilometers gaat u rijden?: ");
    int aantalKilometers = int.Parse(Console.ReadLine());

    Console.Write("In welk seizoen gaat u rijden? (1 = Lente, 2 = Zomer, 3 = Herfst, 4 = Winter): ");
    int welkSeizoen = int.Parse(Console.ReadLine());

    decimal batterijOpslag = 82;
    int actieradius = 663;

    decimal resterendeAccu = verbruikteOpslag(welkSeizoen, batterijOpslag, actieradius, aantalKilometers);

    if (resterendeAccu > 0)
    {
      Console.WriteLine($"Je wilt {aantalKilometers}km gaan rijden in het seizoen {((seizoen)welkSeizoen)}. Je gaat aankomen op je bestemming met {resterendeAccu} kWh accu over.");
    }
    else
    {
      Console.WriteLine($"Je wilt {aantalKilometers}km gaan rijden in het seizoen {((seizoen)welkSeizoen)}. Je gaat helaas NIET aankomen op je bestemming.");
    }

  }

  // Verbruikte opslag methode
  static decimal verbruikteOpslag(int welkSeizoen, decimal batterijOpslag, int actieradius, int aantalKilometers)
  {
    bool isWinter = isWinterMethod(welkSeizoen);

    if (isWinter)
    {
      actieradius = 500;
    }

    decimal verbruikPerKm = batterijOpslag / actieradius;
    decimal accuResterend = batterijOpslag - (verbruikPerKm * aantalKilometers);

    return accuResterend;
  }

  // IsWinter methode
  static bool isWinterMethod(int welkSeizoen)
  {
    return welkSeizoen == (int)seizoen.Winter;
  }
}