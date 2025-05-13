// Opdracht 1
Console.WriteLine("""Welke dag van de week is het? (1 = Maandag, 2 = Dinsdag, 3 = Woensdag, 4 = Donderdag, 5 = Vrijdag, 6 = Zaterdag, 7 = Zondag)""");
var gekozenDag = Convert.ToInt32(Console.ReadLine());

switch (gekozenDag)
{
  case 1:
    Console.WriteLine("Het is Maandag!");
    break;
  case 2:
    Console.WriteLine("Het is Dinsdag!");
    break;
  case 3:
    Console.WriteLine("Het is Woensdag!");
    break;
  case 4:
    Console.WriteLine("Het is Donderdag!");
    break;
  case 5:
    Console.WriteLine("Het is Vrijdag!");
    break;
  case 6:
    Console.WriteLine("Het is Zaterdag!");
    Console.WriteLine("Het is WEEKEND!");
    break;
  case 7:
    Console.WriteLine("Het is Zondag!");
    Console.WriteLine("Het is WEEKEND!");
    break;
}


// Opdracht 2
Console.WriteLine("Vul getal 1 in:");
var getal1 = Convert.ToInt32(Console.ReadLine());

Console.WriteLine("Vul de operator in: (- , + , * , / , %)");
var operatorr = Convert.ToString(Console.ReadLine());

Console.WriteLine("Vul getal 2 in:");
var getal2 = Convert.ToInt32(Console.ReadLine());

switch (operatorr)
{
  case "-":
    Console.WriteLine(getal1 - getal2);
    break;
  case "+":
    Console.WriteLine(getal1 + getal2);
    break;
  case "/":
    Console.WriteLine(getal1 / getal2);
    break;
  case "*":
    Console.WriteLine(getal1 * getal2);
    break;
  case "%":
    Console.WriteLine(getal1 % getal2);
    break;
}