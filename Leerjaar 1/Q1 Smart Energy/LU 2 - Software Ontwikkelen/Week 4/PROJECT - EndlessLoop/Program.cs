var geraden = false;
var minGetal = 10;
var maxGetal = 20;

while (true)
{
  Console.Write("Vul een getal in: ");
  var ingevuld = Int32.Parse(Console.ReadLine());

  if (ingevuld <= maxGetal && ingevuld >= minGetal)
  {
    Console.WriteLine("JAJA, DAT IS HEM!!!");
    break;
  }
  else { Console.WriteLine("Nenenene, niet goed!"); }
}