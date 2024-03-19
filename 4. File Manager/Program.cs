using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_Rekap_01_04
{
    class Program
    {
        static void Main(string[] args)
        {
            string prviParametar="";
            string drugiParametar="";

            /* OBRADA ULAZNIH ARGUMENATA, Command line arguments */
            // ako postoji barem jedan argument
            if (args.Length > 0)
            {
                prviParametar = args[0];
            }
            // ako postoji više od 1 argumenta
            if (args.Length > 1)
            {
                drugiParametar = args[1];
            }

            string folder;
            if (!String.IsNullOrWhiteSpace(prviParametar))
            {
                folder = prviParametar;
            }
            // ako nije definiran prvi parametar tada uzimamo trenutačni direktorij
            else {
                folder = Directory.GetCurrentDirectory();
            }

            Console.WriteLine($"Odabran folder: {folder}");

            if (!Directory.Exists(folder))
            {
                Console.WriteLine("Odabran folder ne postoji!");
                Console.ReadKey();
                return;
            }

            /* ISPIS PODFOLDERA: Directory.GetDirectories */
            string[] podfolderi = Directory.GetDirectories(folder);
            Console.WriteLine("Podfolderi:");
            foreach(string f in podfolderi)
            {
                //Console.WriteLine($"\t{f}");
                DirectoryInfo dInfo = new DirectoryInfo(f);
                Console.WriteLine($"\t{dInfo.Name, -30} kreiran {dInfo.CreationTime}");
            }

            Console.WriteLine("\nPRITISNITE ENTER");
            Console.ReadLine();

            /* KREIRANJE FOLDERA: Directory.CreateDirectory */
            Console.WriteLine($"Kreiranje foldera");
            string noviFolder = Path.Combine(folder, "MEV");
            if (Directory.Exists(noviFolder))
            {
                Console.WriteLine($"Folder {noviFolder} već postoji!");
            }
            else
            {
                Directory.CreateDirectory(noviFolder);
                Console.WriteLine($"Kreiran folder: {noviFolder}");
            }

            Console.WriteLine("\nPRITISNITE ENTER");
            Console.ReadLine();

            /* PREIMENOVANJE FOLDERA: Directory.Move */
            string noviFolderBackup = noviFolder + "_backup";
            Console.WriteLine($"Preimenovanje foldera {noviFolder}");
            Console.WriteLine($"--> {noviFolderBackup}");
            try
            {
                Directory.Move(noviFolder, noviFolderBackup);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Greška preimenovanja: " + ex.Message);
            }

            Console.WriteLine("\nPRITISNITE ENTER");
            Console.ReadLine();

            /* ISPIS DETALJA O DATOTEKAMA DirectoryInfo - GetFiles */
            DirectoryInfo fInfo = new DirectoryInfo(folder);
            FileInfo[] datoteke = fInfo.GetFiles();
            Console.WriteLine($"Folder:{folder}");
            Console.WriteLine("Popis datoteka:");
            foreach(FileInfo dat in datoteke)
            {
                Console.WriteLine($"\t{dat.Name,-40} {VratiVelicinu(dat.Length),10}" +
                    $" Kreirana: {dat.CreationTime}");

                // dohvaćamo naziv datoteke bez ekstenzije
                string naziv = Path.GetFileNameWithoutExtension(dat.Name);

                // kreiramo naziv nove datoteke s ekstenzijom tmp
                string novaDat = Path.Combine(noviFolderBackup, naziv + ".tmp");

                if (!File.Exists(novaDat))
                {
                    //Kopiranje datoteke u podfolder MEV_Backup sa ekstenzijom .tmp
                    File.Copy(dat.FullName, novaDat);
                    Console.WriteLine($"\tKopirano --> {novaDat}");
                }
            }

            Console.WriteLine("\nPRITISNITE ENTER");
            Console.ReadLine();

            /* KREIRANJE TXT DATOTEKE S BROJEM DATOTEKA U DIREKTORIJU */
            string txtDat = Path.Combine(folder, "DirInfo.txt");
            using(StreamWriter sWriter = new StreamWriter(txtDat, true))
            {
                sWriter.WriteLine($"Datum: {DateTime.Now}");
                sWriter.WriteLine($"Broj datoteka: {datoteke.Length}");
                sWriter.WriteLine("----------");
            }
            Console.WriteLine($"Kreirana datoteka: {txtDat}");
            Console.WriteLine("\nPRITISNITE ENTER");
            Console.ReadLine();

            /* DOHVAT ODREĐENIH DATOTEKA */
            /*  Argumenti za DirectoryInfo.GetFiles metodu
                * - dohvaća sve datoteke
                *.txt - dohvaća datoteke s txt ekstenzijom, *.docx - word datoteke, itd...
                c* - dohvaća sve datoteke čiji naziv započinje s c
             */
            FileInfo[] txtDatoteke = fInfo.GetFiles("*.txt");
            Console.WriteLine($"Folder:{folder}");
            Console.WriteLine("Popis txt datoteka:");
            foreach (FileInfo dat in txtDatoteke)
            {
                Console.WriteLine($"\t{dat.Name,-40} {VratiVelicinu(dat.Length),10}" +
                    $" Kreirana: {dat.CreationTime,15}");
            }

            Console.WriteLine("\nPRITISNITE ENTER");
            Console.ReadLine();


            /* BRISANJE DATOTEKA */
            Console.WriteLine("BRISANJE DATOTEKA");
            Console.WriteLine($"Folder:{noviFolderBackup}");
            // 1. način
            /*string[] datotekeZaBrisanje = Directory.GetFiles(noviFolderBackup);
            foreach (string dat in datotekeZaBrisanje) {
                try {
                    File.Delete(dat);
                    Console.WriteLine($"Izbrisano: {dat}");
                }
                catch (Exception e) {
                    Console.WriteLine($"Problem s brisanjem: {dat} {e.Message}");
                }
            }*/

            // 2. način
            DirectoryInfo dfInfo = new DirectoryInfo(noviFolderBackup);
            FileInfo[] datotekeDelete = dfInfo.GetFiles();
            foreach (FileInfo dat in datotekeDelete) {
                try {
                    dat.Delete();
                    Console.WriteLine($"Izbrisano: {dat.Name}");
                }
                catch (Exception e) {
                    Console.WriteLine($"Problem s brisanjem: {dat.Name} {e.Message}");
                }
            }


            /* BRISANJE FOLDERA I ČITAVOG SADRŽAJA */
            Directory.Delete(noviFolderBackup, true);

            Console.WriteLine("\nKRAJ PROGRAMA");
            Console.ReadKey();
        }

        private static string VratiVelicinu(double velicinaDatoteke)
        {
            string[] sufix = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            int s = 0;

            while (velicinaDatoteke >= 1024)
            {
                s++;
                velicinaDatoteke = velicinaDatoteke / 1024;
            }

            velicinaDatoteke = Math.Round(velicinaDatoteke, 2);
            return velicinaDatoteke.ToString() + " " + sufix[s];
        }
    }
}
