using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_Vjezba01
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Aplikacija pokrenuta...");
            Console.WriteLine("Primljenih argumenata: " + args.Length);

            if (args.Length > 0)
            {
                Console.WriteLine("---------ISPIS FOR PETLJOM----------");
                for (int i=0; i < args.Length; i++)
                {
                    //Ispis pomoću String Format načina
                    Console.WriteLine("Argument [{0}] = {1}", i, args[i]);
                }

                Console.WriteLine();
                Console.WriteLine("---------ISPIS FOREACH PETLJOM----------");
                int brojac = 1;
                foreach(string arg in args)
                {
                    //Ispis spajanjem stringa i vrijednosti varijabli
                    //Console.WriteLine("Argument " + brojac + ". = "+ arg +"");

                    //Ispis String Format načinom
                    //Console.WriteLine("Argument {0}. = {1}", brojac, arg);

                    //string interpolation
                    Console.WriteLine($"Argument {brojac}. = {arg}");
                    brojac++;
                }

                Console.WriteLine();
                Console.WriteLine("---------ISPIS kroz ENVIRONMENT klasu----------");
                string[] parametri = Environment.GetCommandLineArgs();
                foreach(string arg in parametri)
                {
                    Console.WriteLine(arg);
                }
            }
            else
            {
                Console.WriteLine("Nema ulaznih argumenata");
            }

            Console.WriteLine();
            Console.WriteLine("------ISPIS DISKOVA------");
            string[] drives = Environment.GetLogicalDrives();
            foreach(string d in drives)
            {
                Console.WriteLine(d);
            }

            Console.WriteLine();
            Console.WriteLine("------ISPIS PODATAKA O RAČUNALU------");
            Console.WriteLine($"OS verzija:           {Environment.OSVersion}");
            Console.WriteLine($"Naziv:                {Environment.MachineName}");
            Console.WriteLine($"Korisnik:             {Environment.UserName}");
            Console.WriteLine($"Broj procesora:       {Environment.ProcessorCount}");
            Console.WriteLine($"Sistemski direktorij: {Environment.SystemDirectory}");
            Console.WriteLine($"App fizička mem:      {Environment.WorkingSet/(1024*1024)} MB");
            Console.WriteLine($"App virtualna mem:    {Environment.SystemPageSize/(1024*1024)} MB");

            Console.WriteLine("\n------ISPIS PODATAKA O DISKOVIMA------");
            DriveInfo[] diskovi = DriveInfo.GetDrives();
            foreach(DriveInfo d in diskovi)
            {
                Console.WriteLine($"Naziv: {d.Name}");
                //DriveType: Fixed-disk, Cdrom, Network...
                Console.WriteLine($"    Tip: {d.DriveType}");


                /*
                 * IsReady daje informaciju ako je jedinica spremna. Npr, vraća true ako je CD u CD-Rom jedinici ili
                 * ako je memorijska jedinica spremna za operacije čitanja / pisanja. Ako se ne provjeri ako je jedinica
                 * spremna, a nije spremna, pozivanje narednih metoda bi izbacilo iznimku tipa IOException.
                 * */
                if (d.IsReady)
                {
                    Console.WriteLine($"    Labela: {d.VolumeLabel}");
                    Console.WriteLine($"    Datotečni sustav: {d.DriveFormat}");
                    Console.WriteLine($"    Kapacitet: {d.TotalSize / (1024*1024)} MB");

                    //Preostali kapacitet za korisnika (uzima u obzir ako su definirane kvote za korisnika - disk quotas)
                    Console.WriteLine($"    Preostali kapacitet (za korisnika): {d.AvailableFreeSpace / (1024*1024)} MB");

                    //Preostali kapacitet
                    Console.WriteLine($"    Preostali kapacitet: {d.TotalFreeSpace / (1024*1024)} MB");
                }
            }

            Console.ReadKey();

        }
    }
}
