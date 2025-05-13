// Opdracht 1
using Microsoft.DotNet.Interactive;
string input = await Kernel.GetInputAsync("Type your name!");
Console.WriteLine("Hello " + input);


// Opdracht 2
using Microsoft.DotNet.Interactive;
string yearOfBirth = await Kernel.GetInputAsync("Type your year of birth!");

int age = DateTime.Now.Year - int.Parse(yearOfBirth);

Console.WriteLine("You are " + age + " years old!");