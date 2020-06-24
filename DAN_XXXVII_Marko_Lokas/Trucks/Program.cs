using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trucks
{
    class Program
    {
        public static int counter = 0;
        public static int counterTwoByTwo = 0;
        public static int waitingFirstLoading = 0;
        public static int waitingSecondLoading = 0;
        public static bool threadOne = false;
        public static bool threadSecond = false;
        static bool lockTaken = false;
        static bool lockTaken1 = false;
        public static readonly object locker = new object();
        public static readonly object locker1 = new object();
        public static List<Thread> TruckThreadList = new List<Thread>();
        public static List<string> TruckTimeDestinationList = new List<string>();
        public static List<string> LoadingTimeListAllTrucks = new List<string>();
        public static List<string> LoadingTimeListAllTrucksSort = new List<string>();
        public static List<string> LoadingTimeListEven = new List<string>();
        public static List<string> LoadingTimeListOdd = new List<string>();
        public static List<int> TopRoutesList = new List<int>();
        public static List<int> TopTenRoutesList = new List<int>();
        public static int randomCounter = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to aplication.");

            Console.WriteLine("Press any key to start script...");
            Console.ReadKey();

            Console.Clear();
            CrateRoutes();
            Thread managerThread = new Thread(new ThreadStart(TopTenRoutes));

            managerThread.Start();
            managerThread.Join();

            for (int i = 1; i < 11; i++)
            {
                if (i % 2 == 1)
                {
                    Thread thread = new Thread(new ThreadStart(FirstLoadingLine))
                    {
                        Name = "Truck-" + i
                    };
                    TruckThreadList.Add(thread);
                }
                else if (i % 2 == 0)
                {
                    Thread thread = new Thread(new ThreadStart(SecondLoadingLine))
                    {
                        Name = "Truck-" + i
                    };
                    TruckThreadList.Add(thread);
                }
            }
            //create threads for destination
            Thread threadDestination = new Thread(new ThreadStart(DestinationWaiting));
            //Logic for loading trucks on the first line
            lock (locker)
            {
                while (counter != 10)
                {
                    //Waiting for the second truck to load
                    while (counterTwoByTwo == 2)
                    {
                        while (counterTwoByTwo == 1)
                        {
                            Monitor.Wait(locker);
                        }
                        Monitor.Wait(locker);
                    }
                    for (int i = counter; i <= TruckThreadList.Count;)
                    {
                        if (i % 2 == 0)
                        {
                            counter++;
                            TruckThreadList[i].Start();
                            counterTwoByTwo++;
                        }
                        break;
                    }
                    for (int i = counter; i <= TruckThreadList.Count;)
                    {
                        if (i % 2 == 1)
                        {
                            counter++;
                            TruckThreadList[i].Start();
                            counterTwoByTwo++;
                        }
                        break;
                    }
                }
            }
            //Logic for loading trucks on the second line
            lock (locker1)
            {
                while (counter != 10)
                {
                    //Waiting for the first truck to load
                    while (counterTwoByTwo == 2)
                    {
                        while (counterTwoByTwo == 1)
                        {
                            Monitor.Wait(locker1);
                        }
                        Monitor.Wait(locker1);
                    }
                    for (int i = counter; i <= TruckThreadList.Count;)
                    {
                        if (i % 2 == 0)
                        {
                            counter++;
                            TruckThreadList[i].Start();
                            counterTwoByTwo++;
                        }
                        break;
                    }
                    for (int i = counter; i <= TruckThreadList.Count;)
                    {
                        if (i % 2 == 1)
                        {
                            counter++;
                            TruckThreadList[i].Start();
                            counterTwoByTwo++;
                        }
                        break;
                    }

                }
            }
            //Waiting for the last two trucks to finish loading
            TruckThreadList[8].Join();
            TruckThreadList[9].Join();
            Thread.Sleep(100);

            //Creating a single truck list with loading times
            for (int i = 0; i < LoadingTimeListOdd.Count; i++)
            {

                for (int j = i; j < LoadingTimeListEven.Count;)
                {
                    LoadingTimeListAllTrucks.Add(LoadingTimeListOdd[i]);
                    LoadingTimeListAllTrucks.Add(LoadingTimeListEven[j]);
                    break;
                }
            }
            //Sorted list of trucks with loading times
            LoadingTimeListAllTrucks.Sort();
            //start destination thread
            threadDestination.Start();
            threadDestination.Join();

            Console.WriteLine("Press any key to exit app");
            Console.ReadKey();
        }
        
        /// <summary>
        /// Method for simulating loading on the first loading line
        /// </summary>
        private static void FirstLoadingLine()
        {
            lock (locker)
            {
                lockTaken = false;
                try
                {
                    //It locks when the truck arrives for loading
                    Monitor.Enter(locker, ref lockTaken);
                    Thread thread = Thread.CurrentThread;
                    Console.WriteLine(thread.Name + " is being loaded");
                    //Random loading time
                    Random randomLoadingTime = new Random();
                    int FirstTruckLoadingTime = randomLoadingTime.Next(500, 5001);
                    Thread.Sleep(FirstTruckLoadingTime);
                    //Loading duration message
                    Console.WriteLine(thread.Name + " is loaded " + FirstTruckLoadingTime + " ms");
                    LoadingTimeListOdd.Add(thread.Name + "," + FirstTruckLoadingTime);
                    barrier.SignalAndWait();

                }
                //Unlocks so the next truck can get in, and lowers the counter
                finally { if (lockTaken) Monitor.Exit(locker); counterTwoByTwo--; }
                waitingFirstLoading++;
                Monitor.PulseAll(locker);
            }
        }

        /// <summary>
        /// Method for simulating loading on another loading line
        /// </summary>
        private static void SecondLoadingLine()
        {
            lock (locker1)
            {
                lockTaken1 = false;
                try
                {
                    //It locks when the truck arrives for loading
                    Monitor.Enter(locker1, ref lockTaken1);
                    Thread thread = Thread.CurrentThread;
                    Console.WriteLine(thread.Name + " is being loaded");
                    //Random loading time
                    Random randomLoadingTime = new Random();
                    int SecondTruckLoadingTime = randomLoadingTime.Next(500, 5001);
                    Thread.Sleep(SecondTruckLoadingTime);
                    //Loading duration message
                    Console.WriteLine(thread.Name + " is loaded " + SecondTruckLoadingTime + " ms");
                    LoadingTimeListEven.Add(thread.Name + "," + SecondTruckLoadingTime);
                    barrier.SignalAndWait();
                }
                //Unlocks so the next truck can get in, and lowers the counter
                finally { if (lockTaken1) Monitor.Exit(locker1); counterTwoByTwo--; }
                waitingSecondLoading++;
                Monitor.PulseAll(locker1);
            }
        }
        static Barrier barrier = new Barrier(2);
        /// <summary>
        /// Method that determines the duration of transport to the destination
        /// </summary>
        /// <returns></returns>
        public static int RandomTransportTime()
        {
            Random random = new Random();
            return random.Next(500, 5001);
        }

        /// <summary>
        /// Route creation method
        /// </summary>
        public static void CrateRoutes()
        {
            lock (locker)
            {
                Console.WriteLine("Random generate routes");
                Console.WriteLine("\nPlease wait...");
                int left = Console.CursorLeft;
                int top = Console.CursorTop;
                Random randomRoutes = new Random();
                //Entering 1000 randomly generated numbers
                using (TextWriter tw = new StreamWriter(@"..\..\Routes.txt"))
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        if (i == 999)
                        {
                            tw.Write(randomRoutes.Next(1, 5001));
                            randomCounter++;
                        }
                        else
                        {
                            tw.WriteLine(randomRoutes.Next(1, 5001));
                            randomCounter++;
                        }
                    }
                }
                Console.SetCursorPosition(left, top);
                Console.WriteLine("Completed random generated routes");
                Monitor.Pulse(locker);
            }
        }
        /// <summary>
        /// Method for selecting the best 10 routes
        /// </summary>
        public static void TopTenRoutes()
        {
            lock (locker)
            {
                if (randomCounter == 1000)
                {
                    Monitor.Wait(locker, 0);
                }
                else
                {
                    Monitor.Wait(locker, 3000);
                }
                string[] routes = File.ReadAllLines(@"..\..\Routes.txt");
                foreach (var route in routes)
                {
                    TopRoutesList.Add(Convert.ToInt32(route));
                }

                //Sorted A to Z
                TopRoutesList.Sort();
                foreach (var item in TopRoutesList)
                {
                    //Draw 10 routes, because there are 10 trucks
                    if (TopTenRoutesList.Count() == 10)
                    {
                        break;
                    }
                    //Because of its uniqueness
                    else if (TopTenRoutesList.Contains(item))
                    {
                        continue;
                    }
                    else
                    {
                        //Minimum number divisible by 3
                        if (item % 3 == 0)
                        {
                            TopTenRoutesList.Add(item);
                        }
                    }
                }
                Console.WriteLine("Manager informs the drivers.");
                Console.WriteLine("Unloading routes are selected");
                Console.Write("\nRoutes:{\t");
                foreach (var TopRoute in TopTenRoutesList)
                {
                    Console.Write(TopRoute + "\t");
                }
                Console.Write("}");
                Console.WriteLine();
                Console.WriteLine("Everything is ready, loading can begin. :)");
                PrintForklift("Loading");
            }
        }

        /// <summary>
        /// Method that notifies the destination that the trucks 
        /// have started and prints important information
        /// </summary>
        public static void DestinationWaiting()
        {
            int count = 0;
            foreach (var item in LoadingTimeListAllTrucks)
            {
                for (int i = count; i < TopTenRoutesList.Count;)
                {
                    //The number of trucks and loading time are extracted
                    string[] trucks = item.Split(',');
                    Console.Write("\n" + new string('-', 50));
                    //To generate different delivery times
                    Thread.Sleep(10);
                    Console.WriteLine("\nYou can expect delivery between 500 and 5000ms...");
                    Console.WriteLine("\nTransport to the destination... RouteID: " + TopTenRoutesList[i]);

                    //Randomly generated delivery time
                    int randomDeliveryTime = RandomTransportTime();
                    //If the delivery time is longer than 3 seconds, delivery is canceled
                    if (randomDeliveryTime > 3000)
                    {
                        Thread.Sleep(3000);
                    }
                    //If the delivery time is less than 3 seconds
                    else
                    {
                        Thread.Sleep(randomDeliveryTime);
                    }
                    //Print a message about the truck, loading time, route ID and estimated delivery time
                    Console.WriteLine("\n" + trucks[0] + "\nTime Loading: " + trucks[1] + "\nRoutes ID: " + TopTenRoutesList[i] + "\nDelivery time: " + randomDeliveryTime);
                    TruckTimeDestinationList.Add(trucks[0] + "," + trucks[1] + "," + TopTenRoutesList[i] + "," + randomDeliveryTime);
                    //When the order is canceled, due to delivery delay
                    if (randomDeliveryTime > 3000)
                    {
                        PrintTruckCanceled();
                        Console.WriteLine("The truck returns to base");
                        Thread.Sleep(3000);
                        Console.WriteLine("The truck is back");
                    }
                    //When the delivery is made on time
                    else
                    {
                        PrintTruckDelivered();
                        int unloading = Convert.ToInt32(trucks[1]);
                        Console.WriteLine("Unloading in progress...");
                        Thread.Sleep(Convert.ToInt16(unloading / 1.5));
                        //Print duration of unloading
                        Console.WriteLine("Time Unloading: " + String.Format("{0:0.00}", unloading / 1.5));
                    }
                    Console.Write("\n" + new string('-', 50));
                    count++;
                    break;
                }
            }
        }
        /// <summary>
        /// Print that the delivery was successful
        /// </summary>
        public static void PrintTruckDelivered()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  _____________  |__\\");
            Console.WriteLine(" | Lokas-trans | |__|");
            Console.WriteLine(" |_____________|_|__|");
            Console.WriteLine("   * * *     * *   *");
            Console.WriteLine("Delivery status: Delivered");
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Print that the delivery failed
        /// </summary>
        public static void PrintTruckCanceled()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  _____________  |__\\");
            Console.WriteLine(" | Lokas-trans | |__|");
            Console.WriteLine(" |_____________|_|__|");
            Console.WriteLine("   * * *     * *   *");
            Console.WriteLine("Delivery status: Canceled");
            Console.ForegroundColor = ConsoleColor.White;
        }
        /// <summary>
        /// Print action: Loading/Unloading
        /// </summary>
        /// <param name="action"></param>
        public static void PrintForklift(string action)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("  __  |");
            Console.WriteLine(" |__\\ |__,");
            Console.WriteLine(" |___||");
            Console.WriteLine("  * *");
            Console.WriteLine("Action: " + action);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
