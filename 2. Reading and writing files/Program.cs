using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_VJ02_Datoteke
{
    class Program
    {
        static void Main(string[] args)
        {
            //  \ - AltGr + Q
            string putanjaDatoteke = @"C:\temp\os_vj02.txt";

            // Path.GetDirectoryName - vraća putanju do direktorija gdje se nalazi datoteka
            // u ovom slučaju: C:\temp\
            string direktorij = Path.GetDirectoryName(putanjaDatoteke);

            //ako direktorij ne postoji ispisujemo poruku i prekidamo daljnje izvođenje programa naredbom return;
            //ukoliko ne postoji folder C:\temp\ na vašem računalu kreirajte ga ručno
            if (!Directory.Exists(direktorij))
            {
                Console.WriteLine("Putanja ne postoji");
                Console.ReadKey();
                return;
            }

            //koristimo StreamWriter klasu za kreiranje i spremanje u datoteku
            //prvi argument je putanja datoteke
            //drugi(opcionalan) je bool - nadodaj sadržaj u datoteku (append) ako ona već postoji - true
            //ili prepisuje (overwrite) sadržaj datoteke - false
            //Primarni cilj using bloka je osigurati da se resursi (kao što su datotečni tokovi, baze podataka veze, itd.) pravilno oslobode nakon što
            //njihova upotreba više nije potrebna, čak i ako dođe do iznimke (exception) tijekom izvođenja koda unutar bloka.
            //Kada koristite using blok s StreamWriter, on automatski zatvara datotečni tok na koji StreamWriter piše čim se izlazi iz bloka using.
            //To smanjuje rizik od curenja resursa i osigurava da su podaci ispravno zapisani i resursi oslobođeni.
            using (StreamWriter sWriter = new StreamWriter(putanjaDatoteke, false))
            {
                sWriter.WriteLine("Bok");
                sWriter.WriteLine("Pozdrav iz Čakovca");
                sWriter.WriteLine("Danas je: " + DateTime.Now);
                sWriter.WriteLine("Ponedjeljak nije moj dan");
            }

            Console.WriteLine("Čitanje iz datoteke:");

            string line = "";
            //koristimo StreamWriter klasu za čitanje podataka iz datoteke sa određene putanje
            using (StreamReader sReader = new StreamReader(putanjaDatoteke))
            {
                //učitavamo redak po redak datoteke u varijablu line i provjeravamo ako vrijednost varijable nije null
                //ako nije null ispisujemo vrijednost varijable line a ako je null (kraj datoteke) nastavljamo s
                //daljnjim izvođenjem programa - program izlazi iz while petlje
                //npr. ako u datoteci ima 5 linija teksta petlja će se izvršiti 5 puta
                 while((line = sReader.ReadLine()) != null)
                 {
                     Console.WriteLine(line);
                 }

                //2. način
                //line = sReader.ReadToEnd();
                //Console.WriteLine(line);
            }

            //čekamo da korisnik pritisne neku tipku jer bi se inače zatvorila aplikacija
            Console.ReadKey();
        }
    }
}
