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
        static Barrier barrier = new Barrier(2);
        public static int counter = 0;
        public static int counter2 = 0;
        public static bool threadOne = false;
        public static bool threadSecond = false;
        public static readonly object locker = new object();
        public static readonly object locker1 = new object();
        static bool lockTaken = false;
        static bool lockTaken1 = false;
        public static List<Thread> TruckThreadList = new List<Thread>();
        public static int waitingFirstLoading = 0;
        public static int waitingSecondLoading = 0;

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
            Thread managerThread = new Thread(new ThreadStart(ManagerMethod));

            managerThread.Start();
            managerThread.Join();
            
            //PrintTruckDelivered();
            //PrintTruckCanceled();
            //PrintForklift("Unloading");
            //PrintForklift("Loading");

            for (int i = 1; i < 11; i++)
            {
                if (i % 2 == 1)
                {
                    Thread thread = new Thread(new ThreadStart(FirstLoadingLine));
                    thread.Name = "Truck-" + i;
                    TruckThreadList.Add(thread);
                }
                else if (i % 2 == 0)
                {
                    Thread thread = new Thread(new ThreadStart(SecondLoadingLine));
                    thread.Name = "Truck-" + i;
                    TruckThreadList.Add(thread);
                }
            }

            lock (locker)
            {
                while (counter != 10)
                {
                    while (counter2 == 2)
                    {
                        while (counter2 == 1)
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
                            counter2++;
                        }
                        break;
                    }
                    for (int i = counter; i <= TruckThreadList.Count;)
                    {
                        if (i % 2 == 1)
                        {
                            counter++;
                            TruckThreadList[i].Start();
                            counter2++;
                        }
                        break;
                    }

                }
            }
            lock (locker1)
            {
                while (counter != 10)
                {
                    while (counter2 == 2)
                    {
                        while (counter2 == 1)
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
                            counter2++;
                        }
                        break;
                    }
                    for (int i = counter; i <= TruckThreadList.Count;)
                    {
                        if (i % 2 == 1)
                        {
                            counter++;
                            TruckThreadList[i].Start();
                            counter2++;
                        }
                        break;
                    }

                }
            }
            TruckThreadList[8].Join();
            TruckThreadList[9].Join();
            Thread.Sleep(100);
            Console.WriteLine("\nPress Enter to continue");
            Console.ReadKey();


            for (int i = 0; i < LoadingTimeListOdd.Count; i++)
            {
                
                for (int j = i; j < LoadingTimeListEven.Count; j++)
                {
                    Console.Write(LoadingTimeListOdd[i] + "\n");
                    Console.Write(LoadingTimeListEven[j] + "\n");
                    break;
                }
            }
            

            //CrateRoutes();
            //TopTenRoutes();

            Console.ReadKey();
        }

        private static void FirstLoadingLine()
        {
            lock (locker)
            {
                lockTaken = false;
                try
                {
                    Monitor.Enter(locker, ref lockTaken);
                    Thread thread = Thread.CurrentThread;
                    Console.WriteLine(thread.Name + " is being loaded");
                    Random randomLoadingTime = new Random();
                    int FirstTruckLoadingTime = randomLoadingTime.Next(500, 5001);
                    Thread.Sleep(FirstTruckLoadingTime);
                    Console.WriteLine(thread.Name + " is loaded " + FirstTruckLoadingTime + " sec");
                    LoadingTimeListOdd.Add(thread.Name + "," + FirstTruckLoadingTime);
                    barrier.SignalAndWait();

                }
                finally { if (lockTaken) Monitor.Exit(locker); counter2--; }
                waitingFirstLoading++;
                Monitor.PulseAll(locker);
            }
        }

        private static void SecondLoadingLine()
        {
            lock (locker1)
            {
                lockTaken1 = false;
                try
                {
                    Monitor.Enter(locker1, ref lockTaken1);
                    Thread thread = Thread.CurrentThread;
                    Console.WriteLine(thread.Name + " is being loaded");
                    Random randomLoadingTime = new Random();
                    int SecondTruckLoadingTime = randomLoadingTime.Next(500,5001);
                    Thread.Sleep(SecondTruckLoadingTime);
                    Console.WriteLine(thread.Name + " is loaded " + SecondTruckLoadingTime  + " sec");
                    LoadingTimeListEven.Add(thread.Name + "," + SecondTruckLoadingTime);
                    barrier.SignalAndWait();
                }
                finally { if (lockTaken1) Monitor.Exit(locker1); counter2--; }
                waitingSecondLoading++;
                Monitor.PulseAll(locker1);
            }
        }

        public static int RandomLoadingTime()
        {
            Random random = new Random();
            return random.Next(500, 15001);
        }

        public static int RandomLoadingTime2()
        {
            Random random = new Random();
            return random.Next(0, 5);
        }

        public static void CrateRoutes()
        {
            lock (locker)
            {
                Console.WriteLine("Random generate routes");
                Console.WriteLine("\nPlease wait...");
                int left = Console.CursorLeft;
                int top = Console.CursorTop;
                Random randomRoutes = new Random();
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

        public static void TopTenRoutes()
        {
            lock (locker)
            {
                //while (randomCounter != 1000)
                //{
                if(randomCounter == 1000)
                {
                    Monitor.Wait(locker,0);

                }
                else
                {
                    Monitor.Wait(locker, 3000);

                }
                //}

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
            }
            
        }

        public static void ManagerMethod()
        {
            TopTenRoutes();
        }
        public static Thread second = Thread.CurrentThread;
        public static Thread first = Thread.CurrentThread;

        public static void Loading()
        {

                
            
            


        }






















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
