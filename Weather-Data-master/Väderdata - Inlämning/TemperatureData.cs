using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Väderdata___Inlämning
{
    internal class TemperatureData
    {
        public delegate string MeteorologicSeason(int indicator);
        public static MeteorologicSeason AutumnChecker = SeasonCheckAutumn;
        public static MeteorologicSeason WinterChecker = SeasonCheckWinter;
        public static List<Data> OutputData(int indicator)
        {
            string location = "";
            string filePath = "../../../Files/tempData.txt";

            List<Data> dataList = new List<Data>();
            List<string> tempData = new List<string>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    tempData.Add(line);
                }
            }

            Regex tempRegex = new Regex(@"(?<year>\d{4})-(?<month>0[1-9]|1[0-2])-(?<day>0[1-9]|[12]\d|3[01]) (?<hours>([01]\d|2[0-3])):(?<minutes>[0-5]\d):(?<seconds>[0-5]\d),(?<indicator>\w+),(?<temp>-?([0-9]\d*(\.\d+)?)),(?<humidity>(?:100|\d{1,2}))"); //Modify temp regex

            if (indicator == 1)
            {
                location = "Ute";
            }
            if (indicator == 2)
            {
                location = "Inne";
            }

            foreach (string data in tempData)
            {
                Match match = tempRegex.Match(data);

                if (match.Success)
                {
                    if (!((int.Parse(match.Groups["year"].Value) == 2016) && (int.Parse(match.Groups["month"].Value) == 5)) && !((int.Parse(match.Groups["year"].Value) == 2017) && (int.Parse(match.Groups["month"].Value) == 1)))
                    {
                        if (match.Groups["indicator"].Value == location)
                        {
                            int year = int.Parse(match.Groups["year"].Value);
                            int month = int.Parse(match.Groups["month"].Value);
                            int day = int.Parse(match.Groups["day"].Value);
                            int hours = int.Parse(match.Groups["hours"].Value);
                            int minutes = int.Parse(match.Groups["minutes"].Value);
                            int seconds = int.Parse(match.Groups["seconds"].Value);
                            float temp = float.Parse(match.Groups["temp"].Value, CultureInfo.InvariantCulture);
                            int humidity = int.Parse(match.Groups["humidity"].Value);

                            DateTime dateTime = new DateTime(year, month, day, hours, minutes, seconds);

                            dataList.Add(new Data(dateTime, temp, humidity));

                        }
                    }
                }

            }
            return dataList;
        }

        public static Tuple<float, int> AverageValuesOfDay(DateTime date, List<Data> tempList)
        {
            var groupedEntries = tempList.GroupBy(e => e.DateTime.Date);
            Tuple<float, int> valueTuple = new Tuple<float, int>(0, 0);
            var meanValues = groupedEntries.Select(group =>
            {
                float meanTemperature = group.Select(e => e.Temperature).Sum() / group.Count();
                int meanHumidity = group.Select(e => e.Humidity).Sum() / group.Count();
                return new
                {
                    DateStamp = group.Key,
                    MeanTemperature = meanTemperature,
                    MeanHumidity = meanHumidity,
                };
            });

            foreach (var item in meanValues)
            {
                if (item.DateStamp.Year == date.Year && item.DateStamp.Month == date.Month && item.DateStamp.Date == date.Date)
                {
                    Tuple<float, int> returnValue = new Tuple<float, int>(item.MeanTemperature, item.MeanHumidity);
                    return returnValue;
                }
            }
            Console.WriteLine("Varning, inget värde finns för angivet datum.");
            return valueTuple;
        }

        public static string SeasonCheckAutumn(int indicator)
        { 
            int consecutiveAutumnDays = 0;
            var list = TemperatureData.SortMeanValuesByDay(TemperatureData.OutputData(indicator));
            foreach (var entry in list)
            {
                if (entry.MeanTemperature < 10)
                {
                    consecutiveAutumnDays++;
                }
                else
                {
                    consecutiveAutumnDays = 0;
                }

                if (consecutiveAutumnDays == 5)
                {
                    return new string($"{entry.DateStamp.AddDays(-4):yyyy-MM-dd} var första meteorogiska höstdagen med medeltemperaturen {Math.Round(entry.MeanTemperature, 2).ToString().DegreesC()}.");
                } 
            }

            return new string("Inga matchningar kan hittas");
        }

        public static string SeasonCheckWinter(int indicator)
        {
            DateTime closestDate = DateTime.MinValue;
            int consecutiveWinterDays = 0;
            var list = TemperatureData.SortMeanValuesByDay(TemperatureData.OutputData(indicator));
            foreach (var entry in list)
            {
                if (entry.MeanTemperature < 0)
                {
                    consecutiveWinterDays++;
                }
                else
                {
                    consecutiveWinterDays = 0;
                }

                if (consecutiveWinterDays == 5)
                {
                    return new string($"{entry.DateStamp.AddDays(-4):yyyy-MM-dd} var första meteorogiska vinterdagen med medeltemperaturen {Math.Round(entry.MeanTemperature, 2).ToString().DegreesC()}.");

                }
                else if (consecutiveWinterDays == 4)
                {
                    closestDate = entry.DateStamp;
                }
            }
            if (consecutiveWinterDays < 5 && closestDate != DateTime.MinValue)
            {
                return new string($"Fullständiga villkor för meteorogisk vinter kunde inte hittas. Datumet som är närmast till att matcha är {closestDate.AddDays(1):yyyy-MM-dd}, med tidigare dagar i följd.");

            }
            else if (consecutiveWinterDays < 5 && closestDate == DateTime.MinValue)
            {
                return new string("Kan tyvärr inte finna information som matchas");
            }

            return new string("Kan tyvärr inte finna information som matchas");
        }

        public static double MoldIndexPercentage(float temperature, int humidity)
        {
            double moldIndex = ((humidity - 78) * (temperature / 15)) / 0.22;

            moldIndex = Math.Max(0, Math.Min(100, moldIndex));

            return moldIndex;
        }

        public static List<(DateTime DateStamp, float MeanTemperature, int MeanHumidity)> SortMeanValuesByDay(List<Data> tempList)
        {
            var groupedEntries = tempList.GroupBy(e => e.DateTime.Date);
            var meanValues = groupedEntries.Select(group =>
            {
                float meanTemperature = group.Select(e => e.Temperature).Sum() / group.Count();
                int meanHumidity = group.Select(e => e.Humidity).Sum() / group.Count();

                return (DateStamp: group.Key, MeanTemperature: meanTemperature, MeanHumidity: meanHumidity);
            });

            return meanValues.OrderBy(x => x.DateStamp.Date).ToList();
        }

        public static List<(DateTime DateStamp, float MeanTemperature, int MeanHumidity)> SortMeanValuesByMonth(List<Data> tempList)
        {
            var groupedEntries = tempList.GroupBy(e => new DateTime(e.DateTime.Year, e.DateTime.Month, 1));

            var meanValues = groupedEntries.Select(group =>
            {
                float meanTemperature = group.Select(e => e.Temperature).Sum() / group.Count();
                int meanHumidity = group.Select(e => e.Humidity).Sum() / group.Count();
                return (DateStamp: group.Key, MeanTemperature: meanTemperature, MeanHumidity: meanHumidity);
            });

            return meanValues.ToList();
        }
       
    }
}
