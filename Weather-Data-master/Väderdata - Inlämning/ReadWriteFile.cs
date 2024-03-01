using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Väderdata___Inlämning
{
    internal class ReadWriteFile
    {

        public static string path = "../../../TextFile/";
        public static void ReadAll(string fileName)
        {
            using (StreamReader reader = new StreamReader(path + fileName))
            {
                string line = reader.ReadLine();
                int rowCount = 0;
                while (line != null)
                {
                    Console.WriteLine(rowCount + " " + line);
                    rowCount++;
                    line = reader.ReadLine();
                }
            }
        }

        public static void WriteAll(string fileName)
        {
            using (StreamWriter streamWriter = new StreamWriter(fileName))
            {
                streamWriter.WriteLine("Medeltemperatur sorterad efter månad (Ute):");

                List<(DateTime, float, int)> monthlyMeanValues = TemperatureData.SortMeanValuesByMonth(TemperatureData.OutputData(1));

                foreach (var meanValues in monthlyMeanValues)
                {
                    streamWriter.WriteLine("Månad: " + meanValues.Item1.Month);   
                    streamWriter.WriteLine("Temperatur: " + Math.Round(meanValues.Item2).ToString().DegreesC());
                    streamWriter.WriteLine("Fuktighet " + meanValues.Item3 + "%");
                    streamWriter.WriteLine("Mögelindex: " + TemperatureData.MoldIndexPercentage(meanValues.Item2, meanValues.Item3));
                }

                streamWriter.WriteLine("Medelfuktighet sorterad efter månad (Inne):");

                List<(DateTime, float, int)> monthlyMeanValues2 = TemperatureData.SortMeanValuesByMonth(TemperatureData.OutputData(2));

                foreach (var meanValues in monthlyMeanValues2)
                {
                    streamWriter.WriteLine("Månad: " + meanValues.Item1.Month);
                    streamWriter.WriteLine("Temperatur: " + Math.Round(meanValues.Item2).ToString().DegreesC());
                    streamWriter.WriteLine("Fuktighet " + meanValues.Item3 + "%");
                    streamWriter.WriteLine("Mögelindex: " + TemperatureData.MoldIndexPercentage(meanValues.Item2, meanValues.Item3));
                }

                streamWriter.WriteLine("Datum för meteorogiska årstider:");
                streamWriter.WriteLine(TemperatureData.AutumnChecker(1));
                streamWriter.WriteLine(TemperatureData.WinterChecker(1));
                streamWriter.WriteLine();
                streamWriter.WriteLine("Uträkningsformel för mögelindex: " + "((luftfuktighet -78) * (Temp/15))/0,22");
            }
        }


    }

}
