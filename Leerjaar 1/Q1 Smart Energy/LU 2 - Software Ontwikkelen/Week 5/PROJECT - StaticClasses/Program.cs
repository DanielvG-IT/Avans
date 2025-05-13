
decimal uitkomst;
Console.Write("Enter the first number: ");
int input1 = int.Parse(Console.ReadLine());

Console.Write("Enter the operator: ");
string @operator = Console.ReadLine();

Console.Write("Enter the second number: ");
int input2 = int.Parse(Console.ReadLine());

switch (@operator)
{
  case "+":
    uitkomst = Rekenmachine.Optellen(input1, input2);
    Console.WriteLine($"Het antwoord is: {uitkomst}");
    break;
  case "-":
    uitkomst = Rekenmachine.Aftrekken(input1, input2);
    Console.WriteLine($"Het antwoord is: {uitkomst}");
    break;
  case "*":
    uitkomst = Rekenmachine.Vermenigvuldigen(input1, input2);
    Console.WriteLine($"Het antwoord is: {uitkomst}");
    break;
  case "/":
    uitkomst = Rekenmachine.Delen(input1, input2);
    Console.WriteLine($"Het antwoord is: {uitkomst}");
    break;
  default:
    Console.WriteLine("HELEMAAL FOUT!"); break;
}
