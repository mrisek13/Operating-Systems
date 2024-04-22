using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Vjezba_08_Posluzitelj
{
    class Program
    {
        static void Main(string[] args)
        {
            /* Korištenje HTTP protokola je moguće uz pokretanje Visual Studia
             * sa administratorskim ovlastima (Desni klik na ikonu pa Run as administrator)
               */
            ServiceHost host = new ServiceHost(typeof(VJ8Komunikacija),
                   new Uri[]{ new Uri("net.pipe://localhost"),
                              new Uri("http://localhost:8000")
            });

            // Imenovani cjevovod moguće je koristiti bez administratorskih ovlasti
            // MSDN: "NetNamedPipeBinding - Provides a secure and reliable binding that is optimized for on-machine communication."
            NetNamedPipeBinding pipeBinding = new NetNamedPipeBinding();
            host.AddServiceEndpoint(typeof(IKomunikacija), pipeBinding, "VJ8Pipeline");

            /* Potrebne administratorske ovlasti
            */
            // MSDN: "The BasicHttpBinding uses HTTP as the transport for sending SOAP 1.1 messages."
            BasicHttpBinding httpBinding = new BasicHttpBinding();
            host.AddServiceEndpoint(typeof(IKomunikacija), httpBinding, "VJ8HTTP");

            host.Open();
            Console.WriteLine("Servis pokrenut\nPritisnite ENTER za zaustavljanje servisa...");
            Console.ReadLine();
            host.Close();

        }
    }

    // implementacija servisa
    public class VJ8Komunikacija : IKomunikacija
    {
        public int Oduzmi(int a, int b)
        {
            return a - b;
        }

        public int Zbroji(int a, int b)
        {
            return a + b;
        }
        public void Pozdrav(string poruka)
        {
            Console.WriteLine($"Proces klijent šalje: {poruka}");
        }

        public List<Proces> DohvatiProcese()
        {
            List<Process> procesi = Process.GetProcesses() // dohvaćamo popis pokrenutih procesa
                .OrderByDescending(x => x.WorkingSet64) // sortiramo procese padajuće po zauzeću memorije
                .Take(10) // uzimamo prvih 10 procesa
                .ToList(); // kolekciju pretvaramo u listu

            List<Proces> lista = new List<Proces>();
            // klasu Process pretvaramo u vlastitu klasu Proces
            foreach (Process p in procesi)
            {
                Proces proc = new Proces();
                proc.Id = p.Id;
                proc.Naziv = p.ProcessName;
                proc.Memorija = p.WorkingSet64;

                lista.Add(proc);
            }
            return lista;
        }
    }

    // dodati referencu na System.ServiceModel u References
    // popis metoda koje ovaj program nudi prema van, tj. koje drugi procesi mogu pozivati
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

    // dodati referencu na System.Runtime.Serialization u References
    // treba nam za serijalizaciju tipa Proces i DataContract i DataMember atribute

    // Ako ne koristimo primitivne tipove kao ulazno izlazne argumente iz metoda,
    // već koristimo složene tipove poput razreda, i klijent i poslužitelj moraju
    // raditi sa istim tipovima, a to znači da mora biti i isti namespace.
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
