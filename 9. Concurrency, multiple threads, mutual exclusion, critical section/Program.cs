using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VJ09_Ponavljanje
{
    class Program
    {
        static void Main(string[] args)
        {
            // provjeravamo ako nema ulaznih parametara
            if (args.Length == 0)
            {
                Console.WriteLine("Niste upisali naredbu!");
                
                // pozivamo metodu koja ispisuje sve podržane naredbe ove aplikacije
                Pomoc();                
            }
            else
            {
                // u varijablu naredba spremamo vrijednost prvog argumenta - na indexu 0 u polju args
                string naredba = args[0];

                // Provjeravamo ako je vrijednost varijable naredba(prvi argument poziva) naredba broj_procesa
                if (naredba == "broj_procesa")
                {
                    int brojProcesa = Process.GetProcesses().Length;
                    Console.WriteLine($"Broj procesa: {brojProcesa}");
                }
                else if(naredba == "broj_dretvi")
                {
                    int brojDretvi = 0;
                    // Dohvaćamo sve pokrenute procese u polje procesi
                    Process[] procesi = Process.GetProcesses();
                    
                    // Petljom prolazimo kroz procese
                    foreach (Process p in procesi)
                    {
                        // u varijablu brojDretvi dodajemo broj dretvi trenutačnog procesa koji je u iteraciji
                        brojDretvi += p.Threads.Count;
                    }
                    Console.WriteLine($"Broj dretvi: {brojDretvi}");
                }
                else if(naredba == "ispisi_sve")
                {
                    // Dohvaćamo listu procesa i sortiramo po nazivu procesa
                    Process[] procesi = Process.GetProcesses().OrderBy(x => x.ProcessName).ToArray();

                    // ispisujemo zaglavlje našeg tabličnog prikaza, drugim argumentom unutar {} definiramo širinu kolone
                    Console.WriteLine("{0,-21} {1,9} {2,10} {3,8}", "Process name", "PID", "Memory", "#Threads");
                    Console.WriteLine(new String('=', 51));
                    
                    foreach(Process p in procesi)
                    {
                        // provjeravamo ako je naziv procesa veći od 20 i kratimo ga
                        string naziv = p.ProcessName.Length > 20 ? p.ProcessName.Substring(0, 20)  : p.ProcessName;
                        Console.WriteLine($"{naziv,-21} {p.Id, 9} {PretvoriVelicinu(p.WorkingSet64), 10} {p.Threads.Count, 8}");
                    }
                    
                    Console.WriteLine(new String('=', 51));
                    Console.WriteLine($"Ukupno procesa: {procesi.Length}");
                }
                else if (naredba == "detalji")
                {
                    // provjeravamo ako nisu definirana 3 argumenta kod ispisa detalja procesa
                    if (args.Length != 3)
                    {
                        Console.WriteLine("Definirajte ID procesa i putanju!");
                    }
                    else
                    {
                        // pretvaramo drugi argument(id procesa) u int tip podatka
                        int pid = int.Parse(args[1]);

                        // dohvaćamo vrijednost trećeg argumenta(putanj datoteke) u varijablu putanja
                        string putanja = args[2];

                        // pozivamo metodu za ispis podataka o procesu u datoteku s argumentom id procesa i putanjom do datoteke
                        IspisiPodatkeOProcesu(pid, putanja);
                    }
                }
                else if (naredba == "pokreni")
                {
                    // provjeravamo ako nisu definirana 2 argumenta
                    if (args.Length != 2)
                        Console.WriteLine("Definirajte naziv procesa");
                    else
                    {
                        try
                        {
                            string naziv = args[1];
                            Process p = new Process();
                            p.StartInfo.FileName = naziv;
                            p.Start();
                        }
                        // ukoliko dođe do iznimke, tj. greške ispisujemo tekst greške, npr. ne postoji traženi program
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                else if(naredba == "zaustavi")
                {
                    // provjeravamo ako nisu definirana 2 argumenta
                    if (args.Length != 2)
                        Console.WriteLine("Definirajte naziv procesa");
                    else
                    {
                        // u varijablu naziv spremamo vrijednost na indexu 1 polja args (drugi argument poziva)
                        string naziv = args[1];
                        
                        int brojZaustavljenihProcesa = 0;
                        Process[] procesi = Process.GetProcesses();
                        
                        foreach(Process p in procesi)
                        {
                            // provjeravamo ako proces u nazivu sadrži vrijednost varijable naziv i još je aktivan
                            if (p.ProcessName.Contains(naziv) && !p.HasExited)
                            {
                                try
                                {
                                    // prekidamo izvršavanje procesa
                                    p.Kill();
                                    brojZaustavljenihProcesa++;
                                }
                                // ukoliko dođe do iznimke, tj. greške ispisujemo tekst greške
                                catch(Exception e)
                                {
                                    Console.WriteLine($"{p.ProcessName}: {e.Message}");
                                }
                            }
                        }
                        Console.WriteLine($"Zaustavljeno procesa: {brojZaustavljenihProcesa}");
                    }
                }
                else
                {
                    Console.WriteLine($"Nepodržana naredba: {naredba}");
                }
            }
            Console.ReadLine();
        }

        private static void IspisiPodatkeOProcesu(int pid, string putanja)
        {
            try
            {
                // Dohvaćamo proces po ID-u i ispisujemo različite podatke o procesu u datoteku
                Process proces = Process.GetProcessById(pid);
                using(StreamWriter sWriter = new StreamWriter(putanja))
                {
                    sWriter.WriteLine("Podaci o procesu");
                    sWriter.WriteLine($"Datum i vrijeme: {DateTime.Now}");
                    sWriter.WriteLine($"Naziv procesa: {proces.ProcessName}");
                    sWriter.WriteLine($"Broj dretvi: {proces.Threads.Count}");
                    sWriter.WriteLine("CPU");
                    sWriter.WriteLine($"\tUser: {proces.UserProcessorTime}");
                    sWriter.WriteLine($"\tPrivileged: {proces.PrivilegedProcessorTime}"); 
                    sWriter.WriteLine("Memorija");
                    sWriter.WriteLine($"\tCurrent: {PretvoriVelicinu(proces.WorkingSet64)}");
                    sWriter.WriteLine($"\tPeak: {PretvoriVelicinu(proces.PeakWorkingSet64)}");
                }
                Console.WriteLine($"Generirana datoteka: {putanja}");
            }
            // ukoliko dođe do iznimke, tj. greške ispisujemo tekst greške, npr. ne postoji proces pod traženim ID-em
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void Pomoc()
        {
            Console.WriteLine("Podržane naredbe:\n");
            Console.WriteLine("broj_procesa \n\t -naredba ispisuje ukupni broj aktivnih procesa na računalu");
            Console.WriteLine("broj_dretvi \n\t -naredba ispisuje ukupni broj dretvi na računalu");
            Console.WriteLine("ispisi_sve \n\t -naredba ispisuje sve procese");
            Console.WriteLine("detalji <PID> <PUTANJA DATOTEKE> \n\t -naredba ispisuje detalje o procesu u datoteku");
            Console.WriteLine("pokreni <NAZIV PROCESA> \n\t -naredba pokreće proces prema nazivu(npr. pokreni winword, pokreni calc, pokreni chrome itd.)");
            Console.WriteLine("zaustavi <FILTER> \n\t -naredba zaustavlja proces koji u svom nazivu sadrži FILTER");
        }

        private static string PretvoriVelicinu(double velicina)
        {
            string[] sufix = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            int s = 0;

            while (velicina >= 1024)
            {
                s++;
                velicina = velicina / 1024;
            }

            velicina = Math.Round(velicina, 2);
            return velicina.ToString() + " " + sufix[s];
        }
    }
}
