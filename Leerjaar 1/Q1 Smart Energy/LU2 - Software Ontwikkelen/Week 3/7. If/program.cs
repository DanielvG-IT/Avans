Console.WriteLine("Vul getal in!");
int getal = Convert.ToInt32(Console.ReadLine());

if (getal % 3 == 0 && getal % 5 == 0)
{
  Console.WriteLine("Fizz Buzz");
}
else if (getal % 3 == 0)
{
  Console.WriteLine("Fizz");
}

else if (getal % 5 == 0)
{
  Console.WriteLine("Buzz");
}

else
{
  Console.WriteLine("""Oops, geen gekke woorden :/""");
}


int x = 5;
int y = 10;
int z = 15;

if ((x + y) > z)
{
  if ((y - x) < (z - 14))
  {
    x = x + y + z;
  }
  else
  {
    y = x * z;
  }
}
else
{
  z = x * y;
}

Console.WriteLine($"x: {x}, y: {y}, z: {z}");