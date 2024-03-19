using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VJ_04_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() {
            InitializeComponent();
        }

        string izvorisniFolder;
        string odredisniFolder;
        string[] popisDatotekaIzvoriste;
        string[] popisDatotekaOdrediste;

        private void BtnFolderIzvor_Click(object sender, RoutedEventArgs e) {
            FolderBrowserDialog folderBD = new FolderBrowserDialog();

            if (folderBD.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                izvorisniFolder = folderBD.SelectedPath;
                txtFolderIzvor.Text = izvorisniFolder;
            }

            RefreshListBoxIzvoriste();
        }

        private void BtnFolderOdrediste_Click(object sender, RoutedEventArgs e) {
            FolderBrowserDialog folderBD = new FolderBrowserDialog();

            if (folderBD.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                odredisniFolder = folderBD.SelectedPath;
                txtFolderOdrediste.Text = odredisniFolder;
            }

            RefreshListBoxOdrediste();
        }

        //Metoda koja ažurira u list box kontroli popis dostupnih datoteka u izvorišnom direktoriju
        private void RefreshListBoxIzvoriste() {
            //učitavamo popis datoteka u direktoriju
            popisDatotekaIzvoriste = Directory.GetFiles(izvorisniFolder);

            //brišemo zapise ako postoje u list box kontroli
            lbDatotekeIzvorFolder.Items.Clear();

            //iteracija kroz polje s datotekama i ubacujemo u list box
            foreach (string datoteka in popisDatotekaIzvoriste) {
                lbDatotekeIzvorFolder.Items.Add(datoteka);
            }
        }

        //Metoda koja ažurira u list box kontroli popis dostupnih datoteka u odredišnom direktoriju
        private void RefreshListBoxOdrediste() {
            popisDatotekaOdrediste = Directory.GetFiles(odredisniFolder);

            lbDatotekeOdredisteFolder.Items.Clear();
            foreach (string datoteka in popisDatotekaOdrediste) {
                lbDatotekeOdredisteFolder.Items.Add(datoteka);
            }
        }

        private string VratiVelicinu(double velicinaDatoteke) {
            string[] sufix = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            int s = 0;

            while (velicinaDatoteke >= 1024) {
                s++;
                velicinaDatoteke = velicinaDatoteke / 1024;
            }

            velicinaDatoteke = Math.Round(velicinaDatoteke, 2);
            return velicinaDatoteke.ToString() + " " + sufix[s];
        }

        private void BtnKopiraj_Click(object sender, RoutedEventArgs e)
        {
            // Provjera ako nije odabrana niti jedna datoteka
            if (lbDatotekeIzvorFolder.SelectedItems.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("Nije odabrana datoteka za kopiranje!");
                return;
            }
            // Provjera ako nije odabran odredišni direktorij
            if (!Directory.Exists(odredisniFolder))
            {
                System.Windows.Forms.MessageBox.Show("Nije odabran odredišni direktorij!");
                return;
            }

            // Prolazimo kroz svaku odabranu stavku u lijevom list boxu
            foreach (string odabranaDatoteka in lbDatotekeIzvorFolder.SelectedItems)
            {
                string naziv = System.IO.Path.GetFileName(odabranaDatoteka);

                // generiramo putanju do nove datoteke na način da spajamo odredišni direktorij i naziv odabrane datoteke
                string novaDat = System.IO.Path.Combine(odredisniFolder, naziv);

                // provjeravamo da li još uvijek postoji odabrana datoteka za kopiranje
                if (File.Exists(odabranaDatoteka))
                {
                    // provjeravamo da li već postoji na odredištu datoteka sa istim nazivom
                    if (File.Exists(novaDat))
                    {
                        // instanciramo klasu FileInfo da bi došli do podataka o odabranoj datoteci
                        // i datoteci pod istim nazivom u odredišnom direktoriju
                        FileInfo odabranDat = new FileInfo(odabranaDatoteka);
                        FileInfo odredisnaDat = new FileInfo(novaDat);

                        // generiramo poruku upozorenja
                        string poruka = $"Oprez, datoteka {naziv} već postoji. "
                            + $"Prepisati? {Environment.NewLine} Odabrana datoteka: {VratiVelicinu(odabranDat.Length)} "
                            + $"ažurirana {odabranDat.LastWriteTime} {Environment.NewLine}"
                            + $"Odredišna datoteka: {VratiVelicinu(odredisnaDat.Length)} "
                            + $"ažurirana {odredisnaDat.LastWriteTime} ";

                        MessageBoxResult dResult = System.Windows.MessageBox.Show(
                            poruka, "Upozorenje", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        // ako korisnik želi prepisati datoteku na odredištu klikne na Yes
                        if (dResult == MessageBoxResult.Yes)
                        {
                            // Pokreće se kopiranje datoteke na odredište s time da se postojeća datoteka
                            // na odredištu prepisuje odabranom datotekom - definira se trećim parametrom - true
                            File.Copy(odabranaDatoteka, novaDat, true);
                        }
                    }
                    // ako na odredištu ne postoji datoteka sa istim nazivom možemo je odmah kopirati
                    else {
                        File.Copy(odabranaDatoteka, novaDat);
                    }
                }
            }

            // pokrećemo metodu koja osvježava prikaz datoteka u odredišnom direktoriju
            RefreshListBoxOdrediste();
        }

        private void BtnPremjesti_Click(object sender, RoutedEventArgs e)
        {
            //Dodajte opciju za premještanje datoteke uz upozorenje korisniku da li je siguran da želi
            //premjesti pojedinu datoteku ako ona već postoji na odredišnom folderu (kao kod kopiranja)
            //HINT: File.Move uz prethodno brisanje datoteke File.Delete ako ona već postoji u odredišnom folderu
        }

        private void BtnIzbrisi_Click(object sender, RoutedEventArgs e)
        {
            // Provjera ako nije odabrana niti jedna datoteka
            if (lbDatotekeIzvorFolder.SelectedItems.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("Nije odabrana datoteka za brisanje!");
                return;
            }

            string poruka = $"Jeste li sigurni da želite izbrisati " +
                $"{lbDatotekeIzvorFolder.SelectedItems.Count} datoteka?";

            // Korisniku postavljamo pitanje dal želi izbrisati odabrane datoteke
            MessageBoxResult dResult = System.Windows.MessageBox.Show(poruka, "Upozorenje",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if(dResult == MessageBoxResult.Yes)
            {
                foreach(string odabranaDat in lbDatotekeIzvorFolder.SelectedItems)
                {
                    try
                    {
                        File.Delete(odabranaDat);
                    }
                    catch(Exception ex)
                    {
                        System.Windows.MessageBox.Show(
                            $"Greška brisanja {odabranaDat} {ex.Message}", "Upozorenje",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }

                // pokrećemo metodu koja osvježava prikaz datoteka u izvornom direktoriju
                RefreshListBoxIzvoriste();
            }
        }
    }
}
