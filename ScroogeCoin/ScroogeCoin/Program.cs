using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;


namespace ScroogeCoin
{
    class Program
    {
        static void Main(string[] args)
        {
            long from = -1; 
            long to = -1;

            //Take years from arguments
            if (args.Length >= 2)
            {
                from = ParseDateToTimestamp(args[0]);
                to = ParseDateToTimestamp(args[1]);
            }
            else //Ask valid years from user
            {
                bool invalidInput = true;
                while (invalidInput)
                {
                    Console.WriteLine("Enter Start Date");
                    string from_str = Console.ReadLine();
                    from = ParseDateToTimestamp(from_str);
                    if (from >= 0)
                    {
                        invalidInput = false;
                    }
                    else
                    {
                        Console.WriteLine("Invalid Date");
                    }
                }
                bool invalidInput2 = true;
                while (invalidInput2)
                {
                    Console.WriteLine("Enter End Date");
                    string to_str = Console.ReadLine();
                    to = ParseDateToTimestamp(to_str);
                    if (to >= 0)
                    {
                        invalidInput2 = false;
                    }
                    else
                    {
                        Console.WriteLine("Invalid Date");
                    }
                }
            }

            to += 3600;     //To include end date into date range, add one hour to end date; 3600 = 1 hour

            //switch years based on lenght
            if (from >= to)
            {
                Console.WriteLine("Dates switched");
                long temp = from;
                from = to;
                to = temp;
            }

            string json = DateRangeJSON(from, to);  //get json string

            //Data from JSON
            Root root = JsonSerializer.Deserialize<Root>(json);

            //Assignment A
            Console.WriteLine("Longest bearing within daterange: " + A(root) + " day(s)");

            //Assignment B
            Tuple<DateTimeOffset, double> trading_volume = B(root);
            Console.WriteLine("Date with highest trading volume: " + trading_volume.Item1.Date);
            Console.WriteLine("Volume in Euros on " + trading_volume.Item1.Date + ": " + trading_volume.Item2 + " euro");

            //Assignment C
            Tuple<DateTimeOffset, DateTimeOffset> buysell = C(root);
            if (buysell != null)
                Console.WriteLine("Buy on " + buysell.Item1.Date + ", and sell on " + buysell.Item2.Date);
            else Console.WriteLine("Should not buy nor sell; Price only decreases on given date range.");

            Console.ReadKey();
        }

        /// <summary>
        /// Gets JSON string from CoinGecko from given date range
        /// </summary>
        /// <param name="from">Starting date timestamp</param>
        /// <param name="to">End date timestamp</param>
        /// <returns>json</returns>
        static string DateRangeJSON(long from, long to)
        {
            string url = "https://api.coingecko.com/api/v3/coins/bitcoin/market_chart/range?vs_currency=eur&from=" + from + "&to=" + to;
            Console.WriteLine(url);
            string json = new WebClient().DownloadString(url);
            return json;
        }

        /// <summary>
        /// Converts date string into Unix Timestamp
        /// </summary>
        /// <param name="date">date as a string</param>
        /// <returns>Unix Timestamp</returns>
        static long ParseDateToTimestamp(string date)
        {
            if (!DateTime.TryParse(date, out DateTime dt))
            {
                return -1;
            }
            DateTimeOffset dto = new DateTimeOffset(dt);
            long timestamp = dto.ToUnixTimeSeconds();
            return timestamp;
        }

        //A: How many days is the longest bearish (downward) trend within a given date range?
        static int A(Root root)
        {
            if (root.prices.Count <= 1) return 0;           //excemption for empty range
            int max = 0;                                    //maximum bearing
            int counter = 0;                                //counting length of the bearing
            double prev_price = (double)root.prices[0][1];  //price of the previous date

            DateTimeOffset new_day = DateTimeOffset.FromUnixTimeMilliseconds((long)root.prices[0][0]);  //Date of the list item
            new_day = new_day.AddHours(2);  //Finnish Timezone

            for (int i = 1; i < root.prices.Count; i++)
            {
                DateTimeOffset this_day = DateTimeOffset.FromUnixTimeMilliseconds((long)root.prices[i][0]);  //Current list item date
                this_day = this_day.AddHours(2); //Finnish Timezone

                if (new_day.Day != this_day.Day)    //Checks whether day has changed
                {
                    new_day = this_day;

                    if ((double)root.prices[i][1] < prev_price) {
                        counter++;
                        if (counter >= max) max = counter;
                    }
                    else {
                        if (counter >= max) max = counter;
                        counter = 0;
                    }
                    prev_price = (double)root.prices[i][1];
                }
                else if (new_day > this_day)
                {
                    new_day = this_day;
                }
            }
            return max;
        }

        //Which date within a given date range had the highest trading volume?
        static Tuple<DateTimeOffset, double> B(Root root)
        {
            if (root.prices.Count <= 0) return null;    //empty list exception
            if (root.prices.Count <= 1)                 //list with only one item exception
            {
                DateTimeOffset onlyday = DateTimeOffset.FromUnixTimeMilliseconds((long)root.total_volumes[0][0]);
                onlyday = onlyday.AddHours(2); //Finnish Timezone
                return new Tuple<DateTimeOffset, double>(onlyday, (double)root.total_volumes[0][1]);
            }

            int max_index = 0;      //index of the item with highest trading volume

            for (int i = 1; i < root.total_volumes.Count; i++)
            {
                if ((double)root.total_volumes[i][1] > (double)root.total_volumes[max_index][1])
                {
                    max_index = i;
                }
            }

            DateTimeOffset day = DateTimeOffset.FromUnixTimeMilliseconds((long)root.total_volumes[max_index][0]);
            day = day.AddHours(2); //Finnish Timezone

            return new Tuple<DateTimeOffset, double>(day, (double)root.total_volumes[max_index][1]);
        }

        /* The application should be able to tell
for a given date range, the best day for buying bitcoin, and the best day for selling the
bought bitcoin to maximize profits. If the price only decreases in the date range, your
output should indicate that one should not buy (nor sell) bitcoin on any of the days. */
        static Tuple<DateTimeOffset, DateTimeOffset> C(Root root)
        {
            if (root.prices.Count <= 1)     //Too small list exception
            {
                return null;
            }

            int min = 0;
            int min_index = 0;
            int max_index = 0;
            double max_profit = 0;

            DateTimeOffset new_day = DateTimeOffset.FromUnixTimeMilliseconds((long)root.prices[0][0]);  //Date of the list item
            new_day = new_day.AddHours(2); //Finnish Timezone

            for (int i = 1; i < root.prices.Count; i++)
            {
                DateTimeOffset this_day = DateTimeOffset.FromUnixTimeMilliseconds((long)root.prices[i][0]); //Current list item date
                this_day = this_day.AddHours(2); //Finnish Timezone

                if (new_day.Day != this_day.Day)    //Checks whether day has changed
                {
                    new_day = this_day;
                    if ((double)root.prices[i][1] < (double)root.prices[min][1])
                    {
                        min = i;
                    }
                    if ((double)root.prices[i][1] - (double)root.prices[min][1] > max_profit)
                    {
                        min_index = min;
                        max_index = i;
                        max_profit = (double)root.prices[i][1] - (double)root.prices[min][1];
                    }
                }
            }

            DateTimeOffset buy_date = DateTimeOffset.FromUnixTimeMilliseconds((long)root.prices[min_index][0]);
            DateTimeOffset sell_date = DateTimeOffset.FromUnixTimeMilliseconds((long)root.prices[max_index][0]);
            buy_date = buy_date.AddHours(2); //Finnish Timezone
            sell_date = sell_date.AddHours(2); //Finnish Timezone

            if (max_profit <= 0)    //Cannot profit exception
            {
                return null;
            }

            return new Tuple<DateTimeOffset, DateTimeOffset>(buy_date, sell_date);
        }


    }


    //JSON Deserialization Object
    public class Root
    {
        public List<List<Double>> prices { get; set; }
        public List<List<Double>> market_caps { get; set; }
        public List<List<Double>> total_volumes { get; set; }
    }
}
