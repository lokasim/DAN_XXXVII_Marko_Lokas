using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trucks
{
    class Program
    {
        public static List<int> TopRoutesList = new List<int>();
        public static List<int> TopTenRoutesList = new List<int>();

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to aplication.");

            Console.WriteLine("Press any key to start script...");
            Console.ReadKey();


            CrateRoutes();
            TopTenRoutes();

            PrintTruckDelivered();
            PrintTruckCanceled();
            Console.ReadKey();
        }


        public static void CrateRoutes()
        {
            Random randomRoutes = new Random();
            using (TextWriter tw = new StreamWriter(@"..\..\Routes.txt"))
            {
                for (int i = 0; i < 1000; i++)
                {
                    tw.WriteLine(randomRoutes.Next(1, 5001));
                }
            }
        }

        public static void TopTenRoutes()
        {
            string[] routes = File.ReadAllLines(@"..\..\Routes.txt");
            foreach (var route in routes)
            {
                TopRoutesList.Add(Convert.ToInt32(route));
            }
            //because of its uniqueness
            TopRoutesList.Distinct();
            //Sorted A to Z
            TopRoutesList.Sort();
            foreach (var item in TopRoutesList)
            {
                //Draw 10 routes, because there are 10 trucks
                if (TopTenRoutesList.Count() == 10)
                {
                    break;
                }
                else
                {
                    //Minimum number divisible by 3
                    if (item % 3 == 0)
                    {
                        Console.WriteLine(item);
                        TopTenRoutesList.Add(item);
                    }
                }
                
            }
        }
























        public static void PrintTruckDelivered()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" ___________  |__\\");
            Console.WriteLine("|Lokas-trans| |__|");
            Console.WriteLine("|___________|_|__|");
            Console.WriteLine("  * * *     *   *");
            Console.WriteLine("Delivery status: Delivered");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void PrintTruckCanceled()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" ___________  |__\\");
            Console.WriteLine("|Lokas-trans| |__|");
            Console.WriteLine("|___________|_|__|");
            Console.WriteLine("  * * *     *   *");
            Console.WriteLine("Delivery status: Canceled");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
