using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MevDir
{
    // podaci o direktoriju
    class DirInfo
    {
        public string Name { get; set; }
        public int FilesCount { get; set; }
        public long FilesLength { get; set; }
        public int DirectoriesCount { get; set; }
    }

    class Program
    {
        // parametri -> direktorij patern
        static void Main(string[] args)
        {
            string rootDirektorij;
            string filter = "";

            // tekući folder
            if (args.Count() == 0)
            {
                rootDirektorij = Directory.GetCurrentDirectory();
            }
            else
            {
                rootDirektorij = args[0];
            }

            // pattern za filtriranje
            if (args.Count() > 1)
            {
                filter = args[1];
            }
            else
            {
                filter = "*";
            }

            if (Directory.Exists(rootDirektorij))
                ispisiDirektorij(rootDirektorij, filter);
            else
                Console.WriteLine("Greška ulaznog argumenta - pogrešno zadani direktorij!");

            #if DEBUG
            Console.ReadLine();
            #endif
        }

        // metoda za ispis sadržaja direktorija
        private static void ispisiDirektorij(string path, string pattern)
        {
            // targetDir
            DirectoryInfo targetDir = new DirectoryInfo(path);
            long velicina = 0, dirVelicina = 0;
            int koliko = 0, dirKoliko = 0, filesKoliko = 0;

            // naziv direktorija i prazan red
            Console.WriteLine("MEV directory info plus 1.0");
            Console.WriteLine("\nDirectory " + path);
            Console.WriteLine();

            // popis datoteka u folderu
            try
            {
                FileInfo[] files = targetDir.GetFiles();
                foreach (FileInfo file in files)
                {
                    velicina += file.Length;
                    koliko++;
                    Console.WriteLine("{0,-20}{1,18:N0} bytes {2}", file.LastWriteTime.ToString("dd.MM.yyyy hh:mm:ss"), file.Length, Path.GetFileName(file.Name));
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Greška: nedozvoljen pristup...");
            }
            catch (PathTooLongException)
            {
                Console.WriteLine("Greška: predugački naziv foldera...");
            }

            // popis poddirektorija u direktoriju
            DirectoryInfo[] direktoriji = targetDir.GetDirectories();
            foreach (DirectoryInfo direktorij in direktoriji)
            {
                DirInfo fi = racunajDirektorij(direktorij, pattern);
                dirVelicina += fi.FilesLength;
                dirKoliko += fi.DirectoriesCount;
                filesKoliko += fi.FilesCount;
                Console.WriteLine("{0,13} files {1,18:N0} bytes <DIR> {2}", fi.FilesCount, fi.FilesLength, Path.GetFileName(fi.Name));
            }
            // slobodno mjesta na disku
            DriveInfo disc = new DriveInfo(Path.GetPathRoot(path));

            // rekapitulacija
            Console.WriteLine("{0,11} file(s)       {1,18:N0} bytes", koliko, velicina);
            Console.WriteLine("{0,11} total file(s) {1,18:N0} bytes", filesKoliko, dirVelicina);
            Console.WriteLine("{0,11} dirs(s)       {1,18:N0} bytes", dirKoliko, dirVelicina);
            Console.WriteLine("   total free space {0,18:N0} bytes", disc.TotalFreeSpace);
        }

        // rekurzivna metoda za izračun broja datoteka i veličine direktorija
        private static DirInfo racunajDirektorij(DirectoryInfo di, string pattern)
        {
            long velicina = 0;   // velicina svih datoteka zajedno
            int koliko = 0;  // broj datoteka u direktoriju
            int kolikoPoddirektorija = 1;
            try
            {
                // prvo prođemo sve fileove i zbojimo veličine
                FileInfo[] files = di.GetFiles(pattern);
                foreach (FileInfo file in files)
                {
                    koliko++;
                    velicina += file.Length;
                }

                // nakon toga prođemo sve poddirektorije u direktoriju
                DirectoryInfo[] dirs = di.GetDirectories();
                foreach (DirectoryInfo direktorij in dirs)
                {
                    // rekurzivni poziv
                    DirInfo poddir = racunajDirektorij(direktorij, pattern);
                    velicina += poddir.FilesLength;
                    koliko += poddir.FilesCount;
                    kolikoPoddirektorija += poddir.DirectoriesCount;
                }
            }
            catch
            { }

            return new DirInfo { Name = di.Name, FilesCount = koliko, FilesLength = velicina, DirectoriesCount = kolikoPoddirektorija };
        }
    }
}
