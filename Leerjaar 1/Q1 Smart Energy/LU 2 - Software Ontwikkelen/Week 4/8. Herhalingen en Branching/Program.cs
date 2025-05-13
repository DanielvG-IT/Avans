Console.WriteLine("Vul getal 1 in");
var input1 = Int32.Parse(Console.ReadLine());
Console.WriteLine("Vul getal 2 in");
var input2 = Int32.Parse(Console.ReadLine());
Console.WriteLine("Vul getal 3 in");
var input3 = Int32.Parse(Console.ReadLine());

int kleinste = default;

if (input1 < input2 && input1 < input3) { kleinste = input1; }
else if (input2 < input1 && input2 < input3) { kleinste = input2; }
else if (input3 < input1 && input3 < input2) { kleinste = input3; }
else { Console.WriteLine("Het gaat helemaal fout!"); }

Console.WriteLine($"Het kleinste getal is: {kleinste}");