using System;
using System.Text.RegularExpressions;
using Väderdata___Inlämning;
using Meny;
using System.Text;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Meny.Meny meny = new Meny.Meny();
        meny.ShowMenu();

    }
}
