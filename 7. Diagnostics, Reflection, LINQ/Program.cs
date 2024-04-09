using System;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace OS_LINQ
{
    class Program
    {
        static void Main(string[] args)
        {
            string procesId = "";
            string akcija = "";

            // ako nema argumenata ispisujemo listu aktivnih procesa
            if (args.Length == 0)
            {
                IspisiListuProcesa();
            }

            // provjeravamo ako postoji barem 1 argument
            if (args.Length > 0)
            {
                procesId = args[0];
            }

            // provjeravamo ako postoje barem 2 argumenta
            if (args.Length > 1)
            {
                akcija = args[1];
            }

            // ako je definiran id procesa (prvi argument)
            // a varijabla akcija (vrsta akcije koju želimo izvršiti, šalje se kao drugi argument)
            // je prazna tada ispisujemo podatke o procesu
            if (!String.IsNullOrWhiteSpace(procesId) && String.IsNullOrWhiteSpace(akcija))
            {
                try
                {
                    // pretvaramo string tip podatka u int tip podatka
                    int pid = int.Parse(procesId);
                    IspisiPodatkeOProcesu2(pid);
                }
                catch (Exception e)
                {
                    // ispisujemo tekst iznimke ukoliko dođe do iznimke
                    Console.WriteLine(e.Message);
                }
            }

            // ako je definiran id procesa (prvi argument) a akcija = STOP tada zaustavljamo proces pod tim ID-em
            if (!String.IsNullOrWhiteSpace(procesId) && akcija.ToUpper() == "STOP")
            {
                try
                {
                    // pretvaramo string tip podatka u int tip podatka
                    int pid = int.Parse(procesId);
                    ZaustaviProces(pid);
                }
                catch (Exception e)
                {
                    // ispisujemo tekst iznimke ukoliko dođe do iznimke
                    Console.WriteLine(e.Message);
                }
            }

            Console.ReadLine();
        }

        static void IspisiListuProcesa()
        {
            Console.WriteLine($"{"Process name",-21} {"PID",9} {"#Threads",8}");
            // new String('=', 21) kreira string od 21 znaka =
            Console.WriteLine($"{new String('=', 21)} {new String('=', 9)} {new String('=', 8)}");

            // LINQ - Language-Integrated Query
            // Dohvaćamo listu procesa i sortiramo po nazivu procesa
            Process[] procesi = Process.GetProcesses().OrderBy(x => x.ProcessName).ToArray();

            foreach (Process p in procesi)
            {
                string naziv;
                // provjeravamo ako je naziv procesa veći od 21
                if (p.ProcessName.Length > 21)
                    // kratimo naziv procesa na 20 znakova
                    naziv = p.ProcessName.Substring(0, 21);
                else
                    naziv = p.ProcessName;

                Console.WriteLine($"{naziv,-21} {p.Id,9} {p.Threads.Count,8}");
            }

            Console.WriteLine($"{new String('=', 21)} {new String('=', 9)} {new String('=', 8)}");
            Console.WriteLine($"Broj procesa: {procesi.Length}");
        }

        static void IspisiPodatkeOProcesu(int pId)
        {
            // Dohvaćamo proces po ID-u i ispisujemo različite podatke o procesu
            Process p = Process.GetProcessById(pId);
            Console.WriteLine($"Process name: {p.ProcessName}");
            Console.WriteLine($"CPU time");
            Console.WriteLine($"\tUser: {p.UserProcessorTime}");
            Console.WriteLine($"\tPrivileged: {p.PrivilegedProcessorTime}");
            Console.WriteLine("Memory usage");
            Console.WriteLine($"\tCurrent: {p.WorkingSet64}");
            Console.WriteLine($"\tPeak: {p.PeakWorkingSet64}");
            Console.WriteLine($"Active threads: {p.Threads.Count}");
        }
        static void IspisiPodatkeOProcesu2(int pId)
        {
            // Dohvaćamo proces po ID-u
            Process p = Process.GetProcessById(pId);

            /* System.Reflection
            // Contains types that retrieve information about assemblies, modules, members, parameters,
            // and other entities in managed code by examining their metadata.
            // These types also can be used to manipulate instances of loaded types,
            // for example to hook up events or to invoke methods. */

            // Korištenjem refleksije dohvaćamo koja su sva svojstva, propertyi dostupni u klasi Process
            PropertyInfo[] props = typeof(Process).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                try
                {
                    // dohvaćamo vrijednost određenog svojstva korištenjem naredbe prop.GetValue(p)
                    Console.WriteLine($"{prop.Name} = {prop.GetValue(p)}");
                }
                catch { }
            }
        }

        static void ZaustaviProces(int pId)
        {
            // Dohvaćamo proces po ID-u
            Process p = Process.GetProcessById(pId);

            if (!p.HasExited)
            {
                Console.WriteLine($"Jeste li sigurni da želite zaustaviti proces {p.ProcessName}? D/N");
                string odgovor = Console.ReadLine();
                if (odgovor.ToUpper() == "D")
                {
                    // Zaustavljamo proces
                    // Iznimka se može desiti npr. ako želimo zaustaviti neki sistemski proces
                    p.Kill();
                    Console.WriteLine("Proces uspješno zaustavljen!");
                }
            }
        }
    }
}
