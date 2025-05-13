Console.WriteLine("Hello, what is your name?");
string naam = Console.ReadLine();
Console.WriteLine("I now know you name!");

int hour = DateTime.Now.Hour;

if (hour < 12)
{
    Console.WriteLine("Good morning, " + naam);
}
else if (hour > 12 && hour < 18)
{
    Console.WriteLine("Good afternoon, " + naam);
}
else
{
    Console.WriteLine("Good evening, " + naam);
}
