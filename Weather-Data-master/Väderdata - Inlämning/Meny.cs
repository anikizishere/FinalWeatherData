using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Väderdata___Inlämning;

namespace Meny
{
    public class Meny
    {

        public void ShowMenu()
        {
            Regex dateRegex = new Regex(@"(?<year>\d{4})-(?<month>0[1-9]|1[0-2])-(?<day>0[1-9]|[12]\d|3[01])");
            int indicator;
            bool exit = false;

            string outputPath = "../../../Files/TextFile.txt";
            while (!exit)
            {
                Console.WriteLine("Välkommen till väderapplikationen!");
                Console.WriteLine("Välj en funktion:");
                Console.WriteLine("1. Medeltemperatur och medelluftfuktighet per dag, för valt datum");
                Console.WriteLine("2. Sortering av varmast till kallaste dagen enligt medeltemperatur per dag");
                Console.WriteLine("3. Sortering av torrast till fuktigaste dagen enligt medelluftfuktighet per dag");
                Console.WriteLine("4. Sortering av minst till störst risk av mögel");
                Console.WriteLine("5. Datum för meteorologisk höst");
                Console.WriteLine("6. Datum för meteorologisk vinter (OBS Mild vinter!)");
                Console.WriteLine("7. Skriv resultat till fil");
                Console.WriteLine("8. Avsluta programmet");


                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        indicator = ChoiceChecker();
                        Console.WriteLine("Ange ett datum: (YYYY-MM-DD)");
                        string input = Console.ReadLine();
                        Match match = dateRegex.Match(input);
                        if (match.Success)
                        {
                            DateTime date = DateTime.ParseExact(input, "yyyy-MM-dd", null);
                            Tuple<float, int> values = TemperatureData.AverageValuesOfDay(date, TemperatureData.OutputData(indicator));
                            Console.WriteLine("Medeltemperaturen för dagen är: " + Math.Round(values.Item1, 2));
                            Console.WriteLine("Medelluftfuktigheten är: " + values.Item2);
                            Console.WriteLine("Mögelindex är: " + TemperatureData.MoldIndexPercentage(values.Item1, values.Item2) + "%");
                        }
                        else
                        {
                            Console.WriteLine("Fel inmatning.");
                        }
                        Console.WriteLine();
                        Console.WriteLine("Tryck på valfri knapp");
                        Console.ReadKey(true);
                        Console.Clear();
                        break;
                    case ConsoleKey.D2:
                        indicator = ChoiceChecker();
                        var list = TemperatureData.SortMeanValuesByDay(TemperatureData.OutputData(indicator)).OrderByDescending(x => x.MeanTemperature).ToList();
                        foreach (var item in list)
                        {
                            Console.WriteLine("Datum: " + item.DateStamp.Date.ToString("yyyy/MM/dd") + " | Medeltemperatur: " + Math.Round(item.MeanTemperature, 2).ToString().DegreesC() + " | Medelfuktighet " + item.MeanHumidity + "%" + " | Mögelchans: " + Math.Round(TemperatureData.MoldIndexPercentage(item.MeanTemperature, item.MeanHumidity), 2) + "%");
                        }
                        Console.ReadKey(true);
                        Console.Clear();
                        break;
                    case ConsoleKey.D3:
                        indicator = ChoiceChecker();
                        var list2 = TemperatureData.SortMeanValuesByDay(TemperatureData.OutputData(indicator)).OrderBy (x => x.MeanHumidity).ToList();
                        foreach (var item in list2)
                        {
                            Console.WriteLine("Datum: " + item.DateStamp.Date.ToString("yyyy/MM/dd") + " | Medeltemperatur: " + Math.Round(item.MeanTemperature, 2).ToString().DegreesC() + " | Medelfuktighet " + item.MeanHumidity + "%" + " | Mögelchans: " + Math.Round(TemperatureData.MoldIndexPercentage(item.MeanTemperature, item.MeanHumidity), 2) + "%");
                        }
                        Console.ReadKey(true);
                        Console.Clear();
                        break;
                    case ConsoleKey.D4:
                        indicator = ChoiceChecker();
                        var list3 = TemperatureData.SortMeanValuesByDay(TemperatureData.OutputData(indicator)).OrderByDescending(x => TemperatureData.MoldIndexPercentage(x.MeanTemperature, x.MeanHumidity)).ToList();
                        foreach (var item in list3)
                        {
                            Console.WriteLine("Datum: " + item.DateStamp.Date.ToString("yyyy/MM/dd") + " | Medeltemperatur: " + Math.Round(item.MeanTemperature, 2).ToString().DegreesC() + " | Medelfuktighet " + item.MeanHumidity + "%" + " | Medelfuktighet " + item.MeanHumidity + "%" + " | Mögelchans: " + Math.Round(TemperatureData.MoldIndexPercentage(item.MeanTemperature, item.MeanHumidity), 2) + "%");
                        }
                        Console.ReadKey(true);
                        Console.Clear();
                        break;
                    case ConsoleKey.D5:
                        indicator = ChoiceChecker();
                        if (indicator == 2)
                        {
                            Console.WriteLine("Denna funktion är tyvärr inte tillgänglig för det valda alternativet, tryck valfri knapp för att gå vidare");
                            Console.ReadKey(true);
                            Console.Clear();
                            break;
                        }
                        Console.WriteLine(TemperatureData.AutumnChecker(indicator));
                        Console.ReadKey(true);
                        break;
                    case ConsoleKey.D6:
                        indicator = ChoiceChecker();
                        if (indicator == 2)
                        {
                            Console.WriteLine("Denna funktion är tyvärr inte tillgänglig för det valda alternativet, tryck valfri knapp för att gå vidare");
                            Console.ReadKey(true);
                            Console.Clear();
                            break;
                        }
                        Console.WriteLine(TemperatureData.WinterChecker(indicator));
                        Console.ReadKey(true);
                        break;
                    case ConsoleKey.D7:
                        ReadWriteFile.WriteAll(outputPath);
                        Console.WriteLine("Fil uppdaterad, tryck valfri knapp för att gå vidare");
                        Console.ReadKey(true);
                        Console.Clear();
                        break;
                    case ConsoleKey.D8:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Ogiltigt val. Vänligen välj ett giltigt alternativ, tryck för att gå tillbaka till huvudmenyn");
                        Console.ReadKey(true);
                        Console.Clear();
                        break;
                }

            }

        }

        public static int ChoiceChecker()
        {
            int indicator;
            Console.WriteLine("Önskar du se ute eller inne-temperatur?");
            string choice = Console.ReadLine().ToUpper();
            Console.Clear();
            if (choice == "UTE")
            {
                Console.WriteLine("Visar mätningar för utemiljö: ");
                indicator = 1;
            }
            else
            {
                Console.WriteLine("Visar mätningar för innemiljö: ");
                indicator = 2;
            }

            return indicator;
        }
    }
}
