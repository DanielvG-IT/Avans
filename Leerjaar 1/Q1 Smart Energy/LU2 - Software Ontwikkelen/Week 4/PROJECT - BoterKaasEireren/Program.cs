using System;
using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;

namespace BoterKaasEireren
{
  class Program
  {
    static void Main()
    {
      char[,] bord = new char[3, 3];
      char huidigeSpeler = '\0';
      char gewonnenspeler = '\0';

      /// Initialize the playing board
      for (int i = 0; i < 3; i++)
      {
        for (int j = 0; j < 3; j++)
        {
          bord[i, j] = ' ';
        }
      }
      Console.WriteLine("Wie mag er eerst? (O of X)");
      huidigeSpeler = Convert.ToChar(Console.ReadLine());



      /// While loop used for every turn
      while (gewonnenspeler != 'X' && gewonnenspeler != 'O')
      {
        for (int i = 0; i < 3; i++)
        {
          for (int j = 0; j < 3; j++)
          {
            Console.Write(bord[i, j]);
            if (j < 2) { Console.Write("|"); }
          }
          Console.WriteLine();
          if (i < 2) { Console.WriteLine("-----"); }
        }

        /// Ask input for next move of current player
        Console.WriteLine($"Vul in waar je het {huidigeSpeler} wilt! (kolom,rij)");
        var inputRondje = Console.ReadLine();
        var parts = inputRondje.Split(',');
        int kolom = int.Parse(parts[0]);
        int rij = int.Parse(parts[1]);

        if (bord[kolom, rij] == ' ')
        {
          bord[kolom, rij] = huidigeSpeler;
        }
        else
        {
          Console.WriteLine("Helaas is die plaats al bezet probeer het opnieuw!");
          continue;
        }






        /// Check who won after every round!!
        // Check rows and columns
        // Check diagonals

        // Check if there is a winner
        if (gewonnenspeler == 'X' || gewonnenspeler == 'O')
        {
          Console.WriteLine($"Speler {gewonnenspeler} heeft gewonnen!");
          break;
        }


        /// Change current player after every turn
        if (huidigeSpeler == 'X') { huidigeSpeler = 'O'; }
        else if (huidigeSpeler == 'O') { huidigeSpeler = 'X'; }
        else { Console.Error.WriteLine("Er ging iets fout. Probeer het opnieuw!"); }
      }
    }
  }
}


