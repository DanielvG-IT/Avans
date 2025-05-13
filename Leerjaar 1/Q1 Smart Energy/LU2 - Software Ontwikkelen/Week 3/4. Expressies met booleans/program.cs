//Opdracht 3
Console.WriteLine("Wat is uw geboortejaar?");

string inputBirthYear = Console.ReadLine();
int birthYear = int.Parse(inputBirthYear);
int currentYear = DateTime.Now.Year;
int age = currentYear - birthYear;

if (age >= 18)
{
  Console.WriteLine("Welkom in de bar. Neem gerust een drankje!");
}
else if (age < -100)
{
  Console.WriteLine("U bent al erg oud, of u heeft een verkeerd geboortejaar ingevuld!");
}
else
{
  Console.WriteLine("Wij schenken geen alcohol aan mensen onder de 18!");
  Console.WriteLine(age);
}




// Opdracht 2
Console.WriteLine("Vul een nummer in");

var inputNumber = Console.ReadLine();
var number = int.Parse(inputNumber);

if (number % 2 == 0)
{
  Console.WriteLine("Het ingevulde nummer is een even getal");
}
else
{
  Console.WriteLine("Het ingevulde nummer is een oneven getal");
}