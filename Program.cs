using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace HoursNamespace
{
    class Program
    {
        static async Task Main(string[] args)
        {
            
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            string uri = configuration.GetSection("AppSettings:Uri").Value ?? "http://localhost:8384/ranges";

            if (args.Length == 3)
            {
                int start = int.Parse(args[0]);
                int lunchStart = int.Parse(args[1]);
                int minutesOfLunchBreak = int.Parse(args[2]);
                uri += $"With/{start}/{lunchStart}/{minutesOfLunchBreak}";
            }
            else if (args.Length == 1)
            {
                int minutesOfLunchBreak = int.Parse(args[0]);
                uri += $"/{minutesOfLunchBreak}";
            }
            else
            {
                uri += "/30";
            }

            Console.WriteLine(uri);
            var response = await new HttpClient().GetStringAsync(uri);
            var json = JObject.Parse(response);

            if (json is null)
            {
                Console.WriteLine("No data found");
                return;
            }

            var rangeDetails = json["rangeDetails"];

            if (rangeDetails is null)
            {
                Console.WriteLine("No data found");
                return;
            }

            Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10}", "start", "end", "duration", "durationInHours");
            Console.WriteLine(new string('-', 40));


            foreach (var rangeDetail in rangeDetails)
            {
                if (rangeDetail is not null && rangeDetail["range"] is not null)
                {
                    var startTime = DateTime.Parse(rangeDetail["range"]["start"].ToString()).TimeOfDay;
                    var endTime = DateTime.Parse(rangeDetail["range"]["end"].ToString()).TimeOfDay;
                    var durationInHours = double.Parse(rangeDetail["durationInHours"].ToString()).ToString("0.00");
                    Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10}", startTime, endTime, rangeDetail["duration"], durationInHours);
                }
            }
            Console.WriteLine();

            // Display the remaining data
            Console.WriteLine("{0,-20} {1,-20} {2,-20}", "totalHours", "totalHoursInHHMM", "expectedLunchTimeInHHMM");
            Console.WriteLine(new string('-', 60));
            var totalHours = double.Parse(json["totalHours"].ToString()).ToString("0.00");
            Console.WriteLine("{0,-20} {1,-20} {2,-20}", totalHours, json["totalHoursInHHMM"], json["expectedLunchTimeInHHMM"]);
        }

    }
}