using System;

class Program
{
  enum weekDagen
  {
    Maandag = 1,
    Dinsdag,
    Woensdag,
    Donderdag,
    Vrijdag,
    Zaterdag,
    Zondag
  }

  enum maaltijden
  {
    Ontbijt = 1,
    Lunch,
    Diner
  }

  static void Main(string[] args)
  {
    Console.WriteLine("Welke maaltijd ga je eten? (1 = Ontbijt, 2 = Lunch, 3 = Diner)");
    var volgendeMaaltijd = Convert.ToInt32(Console.ReadLine().ToLower());

    Console.WriteLine("Wanneer ga je deze maaltijd eten? (1 = Maandag, 2 = Dinsdag, 3 = Woensdag, 4 = Donderdag, 5 = Vrijdag, 6 = Zaterdag, 7 = Zondag)");
    var dagVolgendeMaaltijd = Convert.ToInt32(Console.ReadLine().ToLower());

    // Casting integers naar enums
    maaltijden gekozenMaaltijd = (maaltijden)volgendeMaaltijd;
    weekDagen gekozenDag = (weekDagen)dagVolgendeMaaltijd;

    // Output van de gekozen maaltijd en dag
    Console.WriteLine($"Je gaat {gekozenMaaltijd} eten op {gekozenDag}.");
  }
}
