using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Vjezba08_Klijent
{

    class Program
    {
        private static IKomunikacija pipelineProxy;
        private static IKomunikacija httpProxy;

        static void Main(string[] args)
        {
            //definiramo kanal za Pipeline komunikaciju sa drugim procesom
            ChannelFactory<IKomunikacija> pipelineFactory =
                new ChannelFactory<IKomunikacija>( // tip sučelja - popis metoda koja se mogu pozivati
                    new NetNamedPipeBinding(), // vrsta komunikacijskog protokola
                    new EndpointAddress("net.pipe://localhost/VJ8Pipeline")); // adresa za komunikaciju sa drugim procesom, tj. gdje on osluškuje

            //  stvaranje instance proxy-a koji se može koristiti za pozivanje metoda na udaljenom servisu kao da su lokalne
            pipelineProxy = pipelineFactory.CreateChannel();

            // definiramo kanal za HTTP komunikaciju sa drugim precesom
            ChannelFactory<IKomunikacija> httpFactory =
                new ChannelFactory<IKomunikacija>(
                    new BasicHttpBinding(),
                    new EndpointAddress("http://localhost:8000/VJ8HTTP"));
            httpProxy = httpFactory.CreateChannel();

            // kroz pipelineProxy (pipeline komunikacija) pozivamo metodu Zbroji na drugom procesu: Vjezba 08 Posluzitelj
            int zbroj = pipelineProxy.Zbroji(2, 3);
            Console.WriteLine($"Suma (preko pipeline): {zbroj}");

            int razlika = pipelineProxy.Oduzmi(5, 1);
            Console.WriteLine($"Razlika (preko pipeline): {razlika}");

            int zbroj2 = httpProxy.Zbroji(5, 5);
            Console.WriteLine($"Suma (preko http): {zbroj2}");

            int razlika2 = httpProxy.Oduzmi(3, 2);
            Console.WriteLine($"Razlika (preko http): {razlika2}");

            pipelineProxy.Pozdrav("Ja sam klijent!");

            // pokrećemo metodu koja vraća popis procesa a nalazi se u drugom procesu: Vjezba 08 Posluzitelj
            List<Proces> procesi = pipelineProxy.DohvatiProcese();

            Console.WriteLine($"{"Process name",-21} {"PID",9} {"Memorija",10}");
            // new String('=', 21) kreira string od 21 znaka =
            Console.WriteLine($"{new String('=', 21)} {new String('=', 9)} {new String('=', 10)}");

            foreach (Proces p in procesi)
            {
                string naziv;
                // provjeravamo ako je naziv procesa veći od 21
                if (p.Naziv.Length > 21)
                    // kratimo naziv procesa na 20 znakova
                    naziv = p.Naziv.Substring(0, 21);
                else
                    naziv = p.Naziv;

                Console.WriteLine($"{naziv,-21} {p.Id,9} {PretvoriVelicinu(p.Memorija),10}");
            }
            Console.WriteLine($"{new String('=', 21)} {new String('=', 9)} {new String('=', 10)}");

            Console.ReadLine();
        }

        private static string PretvoriVelicinu(double velicina)
        {
            string[] sufix = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            int brojac = 0;

            while (velicina >= 1024)
            {
                brojac++;
                velicina = velicina / 1024;
            }

            velicina = Math.Round(velicina, 2);
            return velicina.ToString("N2") + " " + sufix[brojac];
        }
    }

    // program mora znati koje metode može pozivati iz prvog programa (poslužitelja)
    // pa isto tako mora imati definiciju sučelja
    [ServiceContract]
    public interface IKomunikacija
    {
        [OperationContract]
        int Zbroji(int a, int b);

        [OperationContract]
        int Oduzmi(int a, int b);

        [OperationContract]
        void Pozdrav(string poruka);

        [OperationContract]
        List<Proces> DohvatiProcese();
    }

    [DataContract(Namespace = "http://www.mev.hr")]
    public class Proces
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Naziv { get; set; }
        [DataMember]
        public long Memorija { get; set; }
    }
}
