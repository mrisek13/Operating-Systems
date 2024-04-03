using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Vjezba_6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Process proces = null;
        List<Process> listaProcesa;

        public MainWindow() {
            InitializeComponent();
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
            return velicina.ToString() + " " + sufix[brojac];
        }

        private void btnProces_Click(object sender, RoutedEventArgs e)
        {
            // OpenFileDialog prikazuje dijaloški okvir za odabir datoteke
            OpenFileDialog openFD = new OpenFileDialog();
            // ako je korisnik odabrao datoteku i kliknuo OK
            if (openFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtPutanjaProcesa.Text = openFD.FileName;
            }
        }

        private void btnPokreni_Click(object sender, RoutedEventArgs e)
        {
            // provjeravamo ako postoji datoteka
            if (!File.Exists(txtPutanjaProcesa.Text))
            {
                System.Windows.Forms.MessageBox.Show("Odaberite datoteku!");
                return;
            }
            // instanciramo objekt klase Process
            proces = new Process();
            // podesimo putanju do aplikacije/datoteke koju želimo pokrenuti
            proces.StartInfo.FileName = txtPutanjaProcesa.Text;
            // pokrećemo aplikaciju, tj. pokreće se proces
            proces.Start();
        }

        private void btnZaustavi_Click(object sender, RoutedEventArgs e)
        {
            // provjeravamo ako smo pokrenuli proces, tj. on nije null i nije još ugašen
            if (proces != null && !proces.HasExited)
            {
                // ako je odgovor korisnika Da, odnosno Yes
                if (System.Windows.Forms.MessageBox.Show("Jeste li sigurni da želite" +
                    "prekinuti izvođenje procesa?", "Upozorenje", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    // stopiramo proces
                    proces.Kill();
                }
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            DohvatiProcese();
        }

        // Metoda koja dohvaća listu procesa i prikazuje ih u ListBox kontroli
        private void DohvatiProcese()
        {
            // čistimo popis postojećih procesa u ListBox kontroli (s lijeve strane)
            lbProcesi.Items.Clear();

            // dohvaćamo popis procesa koji se trenutačno izvršavaju pozivom metode GetProcesses() klase Process
            Process[] procesi = Process.GetProcesses();

            // sortiramo polje procesa i spremamo u listu listaProcesa
            // OrderBy je LINQ naredba, više na: https://www.tutorialsteacher.com/linq
            listaProcesa = procesi.OrderBy(x => x.ProcessName).ToList();

            // prolazimo kroz elemente liste i dodajemo u ListBox kontrolu
            foreach (Process p in listaProcesa)
            {
                lbProcesi.Items.Add(p.ProcessName);
            }

            // Ispisujemo broj procesa
            lblBrojProcesa.Content = listaProcesa.Count;
        }

        private void lbProcesi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // čistimo popis postojećih svojstva procesa u ListBox kontroli (s desne strane)
            lbSvojstvaProcesa.Items.Clear();

            // provjeravamo ako je odabran neki proces, ako nije onda je vrijednost SelectedIndex -1
            if (lbProcesi.SelectedIndex > -1)
            {
                // dohvaćamo podatke o selektiranom procesu preko SelectedIndex svojstva ListBox kontrole
                // koje služi kao index u listi procesa listaProcesa
                Process proc = listaProcesa[lbProcesi.SelectedIndex];
                if(proc.ProcessName != "Idle")
                {
                    lbSvojstvaProcesa.Items.Add($"Ime procesa: {proc.ProcessName}");
                    lbSvojstvaProcesa.Items.Add($"ID: {proc.Id}");
                    lbSvojstvaProcesa.Items.Add($"Virtualna memorija: " +
                        $"{PretvoriVelicinu(proc.VirtualMemorySize64)})");
                    lbSvojstvaProcesa.Items.Add($"Memorija: " +
                        $"{PretvoriVelicinu(proc.WorkingSet64)})");

                    // MainModule.FileName ne možemo dohvatiti za sve procese jer neki
                    // bacaju iznimku na ovoj liniji pa onda tu iznimku moramo uloviti
                    try {
                        lbSvojstvaProcesa.Items.Add($"Putanja: {proc.MainModule.FileName}");
                    }
                    catch(Exception ex) { }

                    // StartTime ne možemo dohvatiti za sve procese jer neki
                    // bacaju iznimku na ovoj liniji pa onda tu iznimku moramo uloviti
                    try {
                        lbSvojstvaProcesa.Items.Add($"Vrijeme pokretanja: {proc.StartTime}");
                    }
                    catch (Exception ex) { }
                }
            }
        }

        private void btnZaustaviOdabrani_Click(object sender, RoutedEventArgs e)
        {
            // ako je odabran neki proces
            if (lbProcesi.SelectedIndex > -1)
            {
                // dohvaćamo podatke o selektiranom procesu preko SelectedIndex svojstva ListBox kontrole
                Process proc = listaProcesa[lbProcesi.SelectedIndex];
                if (proc != null && !proc.HasExited)
                {
                    // ako je odgovor korisnika Da, odnosno Yes
                    if (System.Windows.Forms.MessageBox.Show("Jeste li sigurni da želite" +
                        $" prekinuti izvođenje programa {proc.ProcessName}",
                        "Upozorenje", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == System.Windows.Forms.DialogResult.Yes)
                    {
                        try
                        {
                            // stopiramo proces
                            proc.Kill();

                            // čistimo popis postojećih svojstva procesa u ListBox kontroli (s desne strane)
                            lbSvojstvaProcesa.Items.Clear();

                            // privremeno uspavljujemo aplikaciju na 1000 ms, odnosno 1 sekundu
                            // kako bi se proces stigao ugasiti dok mi ponovno dohvaćamo listu
                            // aktivnih procesa u metodi DohvatiProcese()
                            Thread.Sleep(1000);

                            // osvježavamo popis s lijeve strane
                            DohvatiProcese();
                        }
                        catch( Exception ex)
                        {
                            System.Windows.Forms.MessageBox.Show($"Greška zaustavljanja procesa {proc.ProcessName}: {ex.Message}");
                        }
                    }
                }
            }
        }
    }
}
