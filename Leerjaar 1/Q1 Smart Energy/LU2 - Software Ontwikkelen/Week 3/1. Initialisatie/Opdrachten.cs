// Opdracht 1
Console.WriteLine("What is your name?");
string name = Console.ReadLine();

Console.WriteLine("What is your age?");
string age = Console.ReadLine();

Console.WriteLine($"Your name is {name} and you are {age} years old!");


// Opdracht 2
string cpu = "i7";
int cores = 12;
double snelheid = 4.0;

Console.WriteLine("CPU:");
Console.WriteLine(cpu);
Console.WriteLine("Cores:");
Console.WriteLine(cores);
Console.WriteLine("Snelheid:");
Console.WriteLine(snelheid);

Console.WriteLine();
Console.WriteLine("samengevat");
Console.WriteLine(cpu);
Console.WriteLine(cores);
Console.WriteLine(snelheid);


// Opdracht 3
int totalCosts = 25000;
double averageTemps = 21.7;
double cookingTime = 1.5;

Console.WriteLine($"The total cost of running the heater at an average of {averageTemps} degrees is {totalCosts}");
Console.WriteLine($"The cooking instructions on the recipe said the cooking time is {cookingTime} hours");