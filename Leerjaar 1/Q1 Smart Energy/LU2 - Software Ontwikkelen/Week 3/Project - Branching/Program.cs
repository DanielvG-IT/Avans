decimal saldo = 20;
decimal transactieBedrag = 49.95M;
decimal kredietLimietP = 5000;

decimal kredietLimietN = -kredietLimietP;
decimal voldoendeSaldo = saldo - transactieBedrag;
if (voldoendeSaldo > 0)
{
  saldo = saldo - transactieBedrag;
}
else
{
  if (saldo - transactieBedrag > kredietLimietN)
  {
    saldo = saldo - transactieBedrag;
  }
  else
  {
    Console.WriteLine("Saldo te laag. Transactie niet mogelijk!");
  }
}

Console.WriteLine("Uw Saldo: " + saldo);