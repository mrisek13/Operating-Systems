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

namespace VJ03_DatInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() {
            InitializeComponent();

        }

        private void btnOdaberiDirektorij_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBD = new FolderBrowserDialog();

            // Prikaz dijaloškog okvira za odabir foldera pokrećemo metodom ShowDialog
            // koja vraća DialogResult.OK ako je korisnik odabrao folder i kliknuo OK
            if (folderBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // u txt polje na formi ispisujemo putanju odabranog foldera
                txtDirektorij.Text = folderBD.SelectedPath;
            }
            // ako korisnik odustane od odabira foldera ispisujemo poruku na ekranu
            else {
                System.Windows.MessageBox.Show("Nije odabran folder!");
            }
        }

        private void btnDohvatiDatoteke_Click(object sender, RoutedEventArgs e)
        {
            // provjeravamo ako je korisnik definirao direktorij i ako on postoji na disku
            // Exists vraća true ako direktorij postoji il false ako ne postoji i u tom slučaju opet korisniku nudimo odabir direktorija
            if (!Directory.Exists(txtDirektorij.Text))
            {
                FolderBrowserDialog folderBD = new FolderBrowserDialog();

                if (folderBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtDirektorij.Text = folderBD.SelectedPath;
                }
                // ako korisnik klikne na X ili Cancel kod odabira direktorija tada
                // zaustavljamo daljnje izvođenje metode
                else {
                    System.Windows.MessageBox.Show("Nije odabran folder");
                    return;
                }
            }

            // brišemo sadržaj list box kontrole, tj. datoteke iz prethodnog direktorija
            lbDatoteke.Items.Clear();

            // Directory.GetFiles vraća popis datoteka u direktoriju u obliku string polja
            string[] datoteke = Directory.GetFiles(txtDirektorij.Text);

            // ako je broj elemenata string polja jednak 0 znači da nema datoteka u odabranom direktoriju
            // ispisujemo poruku i zaustavljamo daljnje izvođenje metode naredbom return
            if (datoteke.Length == 0)
            {
                System.Windows.MessageBox.Show("Nema datoteka u direktoriju");
                return;
            }

            // svaku putanju datoteke dodajemo u ListBox kontrolu na formi
            foreach (string dat in datoteke)
            {
                lbDatoteke.Items.Add(dat);
            }
        }

        private void btnObrisi_Click(object sender, RoutedEventArgs e)
        {
            // brišemo sadržaj list box kontrole, tj. datoteke iz prethodnog direktorija
            lbDatoteke.Items.Clear();
        }

        private void lbDatoteke_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ako datoteka nije odabrana prekidamo daljnje izvršavanje metode
            if (lbDatoteke.SelectedItem == null)
                return;

            txtOdabranIndex.Text = lbDatoteke.SelectedIndex.ToString();

            // pretvaramo object tip podatka u string - casting
            string odabranaDatoteka = (string)lbDatoteke.SelectedItem;

            txtOdabranaDatoteka.Text = odabranaDatoteka;

            // za dohvat detalja o datoteci koristimo FileInfo klasu
            FileInfo dat = new FileInfo(odabranaDatoteka);
            //txtVelicinaDatoteke.Text = dat.Length.ToString() + "B";

            txtVelicinaDatoteke.Text = VratiVelicinu(dat.Length);

            txtVrijemeKreiranja.Text = dat.CreationTime.ToString();
            txtVrijemeZadnjegPristupa.Text = dat.LastAccessTime.ToString();
            txtAtributi.Text = dat.Attributes.ToString();

            // 1. način:
            /* if (dat.IsReadOnly)
                txtSamoCitanje.Text = "Da";
            else
                txtSamoCitanje.Text = "Ne";*/

            // 2. način ternarni upit
            txtSamoCitanje.Text = dat.IsReadOnly ? "Da" : "Ne";
        }

        private string VratiVelicinu(long velicina)
        {
            // polje s veličinama
            string[] sufix = { "B", "KB", "MB", "GB", "TB", "PB" };

            // brojač koliko puta smo broj uspjeli podijeliti s 1024
            int brojac = 0;

            // postavljamo inicijalnu vrijednost varijable za veličinu datoteke
            double tmpVelicina = velicina;

            // tako dugo dok je veličina varijable tmpVelicina veća od 1024
            while (tmpVelicina > 1024)
            {
                // brojimo koji put se izvršava while petlja, tj. koliko puta smo broj uspjeli podijeliti s 1024
                brojac++;
                // nova veličina varijable tmpVelicina je podijeljena sa 1024
                tmpVelicina = tmpVelicina / 1024;
            }

            // zaokružujemo vrijednost varijable size na 2 decimale
            double zaokruzeno = Math.Round(tmpVelicina, 2);

            // metoda vraća zaokruženu vrijednost varijable iz prethodnog koraka i dodaje
            // podatkovnu veličinu iz polja sufixa prema broju dijeljenja s 1024 koje nam služi kao indeks
            return zaokruzeno + " " + sufix[brojac];

            /*npr 1. primjer: ako je veličina datoteke 741 byteova tada se while petlja neće izvršiti jer je 741 < 1024
                  2. primjer: ako je veličina datoteke 1.785 byteova:
                   ulazi u while petlju
                   dijeli 1.785/1.024 = 1,743 i uvećava broj prolaza (brojac) za 1
                   provjerava ako je 1,743 veće od 1.024 i nije pa će nastaviti sa programom
                   ispisat će da je veličina 1,743 KB, KB jer je bio 1 prolaz a na indeksu
                   1 string polja sufix je element KB
                  3. primjer: ako je veličina datoteke 4.823.449 byteova:
                   ulazi u while petlju
                   prvi put će veličina biti 4.710,40 (4.823.449/1.024), uvećava broj prolaza
                   u drugom prolazu će biti 4,6 (4.710,4/1024), uvećava broj prolaza
                   nastavlja s programom jer je 4,6 manje od 1.024
                   ispisuje 4,6 MB, MB - jer su bila 2 prolaza while petlje
           */
        }
    }
}
