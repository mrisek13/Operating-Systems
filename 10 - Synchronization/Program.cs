using System;
using System.Threading;

namespace OS_Synchronization
{
    class Program
    {
        // broj dretvi
        static int brojDretvi = 4;
        // broj iteracija svake dretve
        static int brojIteracija = 5;
        // max. pauza [ms]
        // eksprimentirati sa nezaključanim brojačem i max. pauzom
        const int maxPauza = 10;

        static string odabranaOpcija = "";

        // generator slučajnih brojeva
        static Random rnd = new Random();
        static int sirinaKolona = 18;

        // glavni brojac
        static int glavniBrojac = 0;

        // varijable za zaključavanje kritičnog koda
        // za lock i monitor
        static Object lockBrojac = new object();
        // mutex objekt
        static Mutex mutexBrojac = new Mutex();
        // semaphore objekt
        static Semaphore semBrojac = new Semaphore(1, 1);

        static void Main(string[] args)
        {
            Console.WriteLine("Odaberite opciju:");
            Console.WriteLine("1) Kreiranje N dretvi");
            Console.WriteLine("2) Kritični odsječak bez mehanizma");
            Console.WriteLine("3) Kritični odsječak Monitor");
            Console.WriteLine("4) Kritični odsječak lock");
            Console.WriteLine("5) Kritični odsječak Monitor s čekanjem");
            Console.WriteLine("6) Kritični odsječak Mutex");
            Console.WriteLine("7) Kritični odsječak Semaphore");
            odabranaOpcija = Console.ReadLine();
            if (odabranaOpcija == "1")
                KreiranjeDretvi();
            else
                KreiranjeKODretvi();
            Console.ReadLine();
        }

        // Jednostavni primjer sa pokretanjem 5 dretvi gdje svaka dretva pokreće metodu Radi
        public static void KreiranjeDretvi()
        {
            Console.WriteLine($"Pokrećem {brojDretvi} dretvi...\n");

            // definiramo polje za spremanje N dretvi
            Thread[] dretve = new Thread[brojDretvi];
            for (int i = 1; i <= brojDretvi; i++)
            {
                // Kroz konstruktor Thread definiramo metodu (Radi) koju će dretva izvršavati nakon što se pokrene naredbom Start()
                Thread t = new Thread(new ThreadStart(Radi));
                // definiramo naziv dretve
                t.Name = $"D{i}";
                // upisujemo dretvu u polje dretvi
                dretve[i - 1] = t;
                t.Start();

                // zaustavimo glavni program(glavnu dretvu) da se dretve ne kreiraju jedna za drugom
                Thread.Sleep(rnd.Next(10));
            }

            // imenujemo naziv glavne dretve pod kojom se izvršava ova aplikacija
            Thread.CurrentThread.Name = "GLAVNA";
            // i na glavnoj dretvi pokrećemo metodu Radi()
            Radi();

            // Čekamo da sve dretve završe sa svojim radom
            foreach (Thread t in dretve)
            {
                t.Join();
            }

            // nakon što su sve dretve završile sa svojim radom
            Console.WriteLine("Završile sve dretve");
            Console.ReadLine();
        }

        // Metoda koja ispisuje kad je pokrenuta nakon čega zaspi na određeno vrijeme (slučajni broj 0-4000) milisekundi
        // nakon čega ispisuje vrijeme kad je završila
        private static void Radi()
        {
            Console.WriteLine($"Dretva {Thread.CurrentThread.Name} pokrenuta u {DateTime.Now}");
            // pauza, ovdje bi radili vremenski intenzivnu operaciju, ažurirali bazu npr.
            Thread.Sleep(rnd.Next(0, 4000));
            Console.WriteLine($"Dretva {Thread.CurrentThread.Name} završila u {DateTime.Now}");
        }

        // Metoda u kojoj kreiramo 4 dodatne dretve a gdje svaka dretva izvršava metodu PozivanjeKO gdje u petlji poziva metodu KriticniKod
        private static void KreiranjeKODretvi()
        {
            Console.WriteLine("Pokrećem dretve...\n");

            // Definiramo zaglavlje
            Console.Write("GLAVNA".PadRight(sirinaKolona));
            for (int i = 1; i <= brojDretvi; i++)
            {
                Console.Write($"D{i}".PadRight(sirinaKolona));
            }
            Console.WriteLine("\n" + new string('-', (brojDretvi + 1) * sirinaKolona));

            // definiramo polje za spremanje N dretvi
            Thread[] dretve = new Thread[brojDretvi];
            for (int i = 1; i <= brojDretvi; i++)
            {
                // Kroz konstruktor Thread definiramo metodu (PozivanjeKO) koju će dretva izvršavati nakon što se pokrene naredbom Start(i)
                // Ovdje koristimo ParameterizedThreadStart jer moramo naredbom Start slati argument, u našem slučaju broj dretve i
                Thread t = new Thread(new ParameterizedThreadStart(PozivanjeKO));
                t.Name = $"D{i}";
                Thread.Sleep(rnd.Next(20));
                t.Start(i);
                dretve[i - 1] = t;
            }

            // imenujemo naziv glavne dretve pod kojom se izvršava ova aplikacija
            Thread.CurrentThread.Name = "Glavna";
            // i na glavnoj dretvi pokrećemo metodu RadiNesto() s argumentom 0 jer je ovo glavna dretva
            PozivanjeKO(0);

            // Čekamo da sve dretve završe sa svojim radom pa ispisujemo vrijednost globalnog brojača
            foreach (Thread t in dretve)
            {
                t.Join();
            }
            Console.WriteLine($"Završile sve dretve! Vrijednost brojača: {glavniBrojac}");
        }

        /*
        * metoda koju izvršava svaka dretva
        * cilj je prikazati vrijednost globalne varijable kad nema mehanizma isključivanja i kad se uključi neki od mehanizama isključivanja
        * cilj mehanizama isključivanja je da samo jedna dretva može ući u kritični odsječak, u našem slučaju KriticniOdsjecak()
        * dok ostale dretve moraju čekati da prethodna dretva izađe iz tog kritičnog odsječka
        * */
        private static void PozivanjeKO(object dretva)
        {
            // broj dretve dobivamo kao object pa radimo casting u int tip podatka
            int idDretva = (int)dretva;

            // generiramo broj praznih mjesta kako bi simulirali kolone
            // kod glavne dretve je _dretva 0 pa je 0 * 18 = 0 što znači da odma na početku može pisati
            // kod dretve 3 je 3 * 18 = 54 što znači da mora ostaviti 54 znaka prazno nakon čega tek piše sadržaj
            string razmak = new string(' ', idDretva * sirinaKolona);
            Console.WriteLine($"{razmak}S {DateTime.Now:HH:mm:ss fff}");

            // N puta izvršavamo kritični kod
            for (int i = 1; i <= brojIteracija; i++)
            {
                if (odabranaOpcija == "2")
                {
                    /* BEZ MEHANIZMA */
                    KriticniOdsjecak();
                }
                else if (odabranaOpcija == "3")
                {
                    /* MONITOR */
                    Monitor.Enter(lockBrojac);
                    try
                    {
                        KriticniOdsjecak();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        Monitor.Exit(lockBrojac);
                    }
                }
                else if (odabranaOpcija == "4")
                {
                    /* LOCK */
                    // Exclusive locking is used to ensure that only one thread can enter particular sections of code at a time.
                    // The two main exclusive locking constructs are lock and Mutex. Of the two, the lock construct is faster
                    // and more convenient.Mutex, though, has a niche in that its lock can span applications in different
                    // processes on the computer.
                    lock (lockBrojac)
                    {
                        KriticniOdsjecak();
                    }
                }
                else if (odabranaOpcija == "5")
                {
                    /* MONITOR verzija 2*/
                    //Čeka N milisekundi i status ulaska u Monitor sprema u varijablu slobodno
                    int cekaj = maxPauza * 2;
                    bool slobodno = false;

                    Monitor.TryEnter(lockBrojac, cekaj, ref slobodno);
                    // uspio je uć u Monitor unutar određenog intervala
                    if (slobodno)
                    {
                        try
                        {
                            KriticniOdsjecak();
                        }
                        finally
                        {
                            Monitor.Exit(lockBrojac);
                        }
                    }
                    // nije uspio uć u Monitor unutar određenog intervala pa ispisujemo grešku
                    else
                    {
                        Console.WriteLine($"{razmak}Neuspj. zaklj.");
                    }
                }
                else if (odabranaOpcija == "6")
                {
                    /* MUTEX */
                    mutexBrojac.WaitOne();
                    KriticniOdsjecak();
                    mutexBrojac.ReleaseMutex();
                }
                else if (odabranaOpcija == "7")
                {
                    /* SEMAPHORE */
                    semBrojac.WaitOne();
                    KriticniOdsjecak();
                    semBrojac.Release();
                }

                // ispisujemo vrijednost: broj prolaza / vrijednost globalne varijable glavniBrojac
                Console.WriteLine($"{razmak}P:{i}/Br:{glavniBrojac}");
            }
            // na kraju ispisujemo vrijeme završetka metode
            Console.WriteLine($"{razmak}E {DateTime.Now:HH:mm:ss fff}");
        }

        // kritični dio koda nad kojim koristimo mehanizme isključivanja
        // a u kojem ažuriramo globalnu varijablu
        // bez mehanizma isključivanja dešava se da više dretvi istovremeno izvršava ovu metodu što želimo spriječiti
        private static void KriticniOdsjecak()
        {
            int g = glavniBrojac;
            Thread.Sleep(rnd.Next(0, 20));
            glavniBrojac = g + 1;
        }
    }
}
