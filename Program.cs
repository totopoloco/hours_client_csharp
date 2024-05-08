using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace HoursNameSpace
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Create a new instance of the Hours class
            int start = 0;
            int lunchStart = 0;
            int minutesOfLunchBreak = 30;

            string uri = "http://localhost:8384/ranges";
            if (args.Length >= 3)
            {
                start = int.Parse(args[0]);
                lunchStart = int.Parse(args[1]);
                minutesOfLunchBreak = int.Parse(args[2]);
                uri += $"WithStartLunchAndMinutesOfLunchBreak/{start}/{lunchStart}/{minutesOfLunchBreak}";
            }
            else if (args.Length >= 1)
            {
                minutesOfLunchBreak = int.Parse(args[0]);
                uri += $"/{minutesOfLunchBreak}";
            }
            else
            {
                uri += "/30";
            }

            Console.WriteLine(uri);
            var response = await new HttpClient().GetStringAsync(uri);
            var json = JObject.Parse(response);

            if (json == null)
            {
                Console.WriteLine("No data found");
                return;
            }

            var rangeDetails = json["rangeDetails"];

            if (rangeDetails == null)
            {
                Console.WriteLine("No data found");
                return;
            }

            // Display the rangeDetails array in a table format with separate columns for start and end times
            Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10}", "start", "end", "duration", "durationInHours");
            Console.WriteLine(new string('-', 40));


            foreach (var rangeDetail in rangeDetails)
            {
                if (rangeDetail != null && rangeDetail["range"] != null && rangeDetail["range"]["start"] != null && rangeDetail["range"]["end"] != null && rangeDetail["duration"] != null && rangeDetail["durationInHours"] != null)
                {
                    Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10}", rangeDetail["range"]["start"], rangeDetail["range"]["end"], rangeDetail["duration"], rangeDetail["durationInHours"]);
                }
            }
            Console.WriteLine();

            // Display the remaining data
            Console.WriteLine("{0,-20} {1,-20} {2,-20}", "totalHours", "totalHoursInHHMM", "expectedLunchTimeInHHMM");
            Console.WriteLine(new string('-', 60));
            Console.WriteLine("{0,-20} {1,-20} {2,-20}", json["totalHours"], json["totalHoursInHHMM"], json["expectedLunchTimeInHHMM"]);

        }

    }
}