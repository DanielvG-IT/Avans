bool doorgaankeuze = true;
int userKeuze = default;
List<string> danielsnicelijst = [];

Console.WriteLine("Welkom bij: Daniel's Nicè Lijst");

while (doorgaankeuze == true)
{
  Console.WriteLine("");
  Console.WriteLine("1 = Toevoegen");
  Console.WriteLine("2 = Verwijderen");
  Console.WriteLine("3 = Toon alle items");
  Console.WriteLine("4 = Zoeken");
  Console.WriteLine("9 = Stop applicatie");
  Console.WriteLine("");
  Console.Write("Maak uw keuze: ");
  userKeuze = Int32.Parse(Console.ReadLine());

  switch (userKeuze)
  {
    case 1:
      {
        string toevoegen;
        Console.Write("Wat wil je toevoegen?: ");
        toevoegen = Console.ReadLine();

        danielsnicelijst.Add(toevoegen);

        break;
      }
    case 2:
      {
        string verwijderen;
        Console.WriteLine("Wat wil je verwijderen?: ");
        verwijderen = Console.ReadLine();

        danielsnicelijst.Remove(verwijderen);

        break;
      }
    case 3:
      {
        Console.WriteLine("De onderstaande items zitten in de lijst!:");
        foreach (var item in danielsnicelijst)
        {
          Console.WriteLine($"\t{item}");
        }
        break;
      }
    case 4:
      {
        string zoeken;
        Console.Write("");
        Console.Write("Wat wil je zoeken?: ");
        zoeken = Console.ReadLine();

        if (danielsnicelijst.Contains(zoeken) == true)
        {
          Console.WriteLine($"Het item {zoeken} komt voor in de lijst!");
        }
        else
        {
          Console.WriteLine($"Het item {zoeken} komt NIET voor in de lijst!");
        }
        break;
      }
    case 9:
      {
        Console.WriteLine("De applicatie wordt afgesloten!");
        doorgaankeuze = false;
        break;
      }
    default:
      {
        Console.WriteLine("Dit was geen geldige keuze!");
        break;
      }
  }
}