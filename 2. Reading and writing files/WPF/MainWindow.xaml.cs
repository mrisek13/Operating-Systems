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

namespace OS_VJ02_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOtvori_Click(object sender, RoutedEventArgs e)
        {
            //Koristimo OpenFileDialog klasu za pokretanje dijaloškog okvira za odabir datoteke
            OpenFileDialog openFD = new OpenFileDialog();
            //definiramo koje datoteke (ekstenziju) možemo odabrati
            openFD.Filter = "txt files(*.txt)|*.txt";

            //Prikaz dijaloškog okvira za odabir datoteke pokrećemo metodom ShowDialog
            //koja vraća DialogResult.OK ako je korisnik odabrao datoteku i kliknuo OK
            if (openFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //putanju odabrane datoteke dobivamo kroz property FileName i spremamo u varijablu putanja
                string putanja = openFD.FileName;
                //u txt polje na formi ispisujemo putanju odabrane datoteke
                txtDatCitaj.Text = putanja;

                //u varijablu sadrzaj učitavamo sadržaj tekstualne datoteke koristeći klasu File i metodu ReadAllText
                string sadrzaj = File.ReadAllText(putanja);
                //ispisujemo sadržaj varijable sadrzaj u tekstualno polje na formi
                txtIspis.Text = sadrzaj;
            }
        }

        private void btnFolder_Click(object sender, RoutedEventArgs e)
        {
            //Koristimo FolderBrowserDialog za pokretanje dijaloškog okvira za odabir foldera
            FolderBrowserDialog folderBD = new FolderBrowserDialog();

            //Prikaz dijaloškog okvira za odabir foldera pokrećemo metodom ShowDialog
            //koja vraća DialogResult.OK ako je korisnik odabrao folder i kliknuo OK
            if (folderBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //u txt polje na formi ispisujemo putanju odabranog foldera
                txtFolderSpremi.Text = folderBD.SelectedPath;
            }
            //ako korisnik odustane od odabira foldera ispisujemo poruku na ekranu
            else {
                System.Windows.MessageBox.Show("Nije odabran folder za pohranu!");
            }
        }

        private void btnSpremi_Click(object sender, RoutedEventArgs e)
        {
            //ako je vrijednost polja na formi txtFolderZaSpremanje prazna ispisujemo poruku i prekidamo daljnje izvođenje programa
            if (!Directory.Exists(txtFolderSpremi.Text))
            {
                System.Windows.MessageBox.Show("Definirajte folder!");
                return;
            }

            //ako je vrijednost polja na formi txtNazivDatoteke prazna ispisujemo poruku i prekidamo daljnje izvođenje programa
            if (String.IsNullOrWhiteSpace(txtDatNaziv.Text))
            {
                System.Windows.MessageBox.Show("Definirajte datoteku!");
                return;
            }

            //kombiniramo folder za spremanje i naziv datoteke u varijablu putanjaDatoteke
            string putanjaDatoteke = System.IO.Path.Combine(txtFolderSpremi.Text, txtDatNaziv.Text);

            /*
            * 1. način spremanja sadržaja u datoteku
           //koristimo StreamWriter da kreiramo i zapišemo podatke iz polja txtUpis u datoteku
           //u prethodom zadatku smo koristili sWriter = new StreamWriter(filePath)
           //dok ovdje koristimo File.CreateText() koji u svojoj implementaciji ima liniju return new StreamWriter(path, false)
           //tako da dođe na isto koristili jedan ili drugi način jedino što korištenjem
           //File.CreateText ne možemo definirati da se sadržaj doda u datoteku ako ona već postoji već je uvijek prepisuje
           using(StreamWriter sWriter = File.CreateText(putanja))
           {
               sWriter.Write(txtSadrzajZaUpis.Text);
           }*/

            //Kreiramo datoteku razredom File i metodom WriteAllText
            File.WriteAllText(putanjaDatoteke, txtUpis.Text);

            //Prikazujemo putanju novokreirane datoteke
            System.Windows.MessageBox.Show("Kreirana datoteka: " + putanjaDatoteke);

        }
    }
}
