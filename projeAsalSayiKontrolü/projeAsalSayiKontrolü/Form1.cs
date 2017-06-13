using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SqlClient;
using System.Threading;
using System.Collections;

namespace projeAsalSayiKontrolü
{
    public partial class Form1 : Form
    {
        bool sonuc = false;
        int[] tablomsu;
        int siradakiSayi = 0;
        int[] baslaIndex ;
        int[] bitirIndex ;
        int threadEleman = 0;
        ArrayList threadler = new ArrayList();
        ArrayList sonuclar = new ArrayList();
        Thread main;
        


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            main=new Thread(new ThreadStart(mainThread));
            main.Start();
        }

        public void mainThread()
        {
            int i = 0;

            int[] tablomtrak = tabloYarat();
            i = tablomtrak[tablomtrak.Length-1]+2;
            System.Windows.Forms.Form.CheckForIllegalCrossThreadCalls = false;

            for (; ; i = i + 2)
            {
                label2.Text = " " + i;
                siradakiSayi = i;
                tablomtrak = tabloYarat();
                tablomsu = tablomtrak;
                int tavanIndex = 0;
                int threadSayisi = 0;
                threadEleman = 0;
                int sonThreadEleman = 0;

                listBox1.Items.Clear();
                listBox2.Items.Clear();

                for (tavanIndex = 0; tablomtrak[tavanIndex] <= Convert.ToInt32(Math.Sqrt(i)); )
                {
                    ++tavanIndex;

                }

                if (tavanIndex >= 100)
                {
                    threadSayisi = (int)(tavanIndex / 100) + 1;
                }
                else
                    threadSayisi = 2;

                Thread[] threadDizim = new Thread[threadSayisi];

                threadEleman = (int)((tavanIndex) / threadSayisi);
                sonThreadEleman = tavanIndex - (threadEleman * (threadSayisi - 1));

                baslaIndex = new int[threadSayisi];
                bitirIndex = new int[threadSayisi];

                 
                for (int j = 0; j < threadSayisi; ++j)
                {

                    if (j != (threadSayisi - 1))
                    {
                        

                        this.baslaIndex[j] = (threadEleman * j);
                        
                        if(j!=0)
                            baslaIndex[j]=baslaIndex[j]+1;
                        
                        this.bitirIndex[j] = baslaIndex[j] + threadEleman;
                        int tabloSayac = baslaIndex[j];
                        int[] parametre = new int[(bitirIndex[j] - baslaIndex[j])];
                        for (int sayac = 0; sayac < (bitirIndex[j] - baslaIndex[j]); ++sayac)
                        {
                            parametre[sayac] = tablomsu[tabloSayac];
                            ++tabloSayac;
                        }

                        threadDizim[j] = new Thread(() => ilkAsalMi(parametre));
                        threadDizim[j].Start();

                    }
                    else
                    {
                        this.baslaIndex[j] = (threadEleman * j);
                        this.bitirIndex[j] = tavanIndex;
                        int tabloSayac = baslaIndex[j];
                        int[] parametre = new int[(bitirIndex[j] - baslaIndex[j])];
                        for (int sayac = 0; sayac <(bitirIndex[j] - baslaIndex[j]) ; ++sayac)
                        {
                            parametre[sayac] = tablomsu[tabloSayac];
                            ++tabloSayac;
                        }
                        threadDizim[j] = new Thread(() => sonAsalMi(parametre));
                        threadDizim[j].Start();

                    }
                }
                for (int kontrol = 0; kontrol < threadSayisi; )
                    if (!threadDizim[kontrol].IsAlive)
                        ++kontrol;

                if (sonuc == true)
                    listBox2.Items.Add(siradakiSayi + " sayisi asal değildir!\n");
                else
                {
                    listBox2.Items.Add(siradakiSayi + " sayisi asaldir!\n");
                    Thread ekle=new Thread(new ThreadStart(tabloyaEkle));
                    ekle.Start();
                    while(!ekle.IsAlive);
                }
                foreach (String s in threadler) listBox1.Items.Add(s);
                foreach (String c in sonuclar) listBox2.Items.Add(c);
                
                threadler.Clear();
                sonuclar.Clear();
                sonuc = false;
                Thread.Sleep(2000);
            }
            
        }

        public void tabloyaEkle()
        {
            SqlConnection bag = new SqlConnection("Data Source=.; Initial Catalog=projeAsal; Integrated Security=SSPI");
            bag.Open();

            SqlCommand komut = new SqlCommand("INSERT INTO asalSayilar(sayilar) VALUES('"+siradakiSayi+"')", bag);
            komut.ExecuteNonQuery();
            
            bag.Close();
        }


        public void ilkAsalMi(int[] tablocuk)
        {
            int threadSirasi = 0;
            sonuc = false;
            for (int i = 0; i < tablomsu.Length; ++i)
            {
                if (tablocuk[0] == tablomsu[i])
                {
                    threadSirasi = (int)i / threadEleman;
                    break;
                }
            }
            for (int i = 0; i < tablocuk.Length; ++i)
            {
                if (siradakiSayi % tablocuk[i] == 0)
                {

                    sonuc = true;
                    sonuclar.Add("\n" + threadSirasi + ". Thread içerisindeki " + tablocuk[i] + " sayisina bölünmüştür.");
                    break;
                }
                    
            }

            threadler.Add("\n" + threadSirasi + ". Thread başlangıç ve bitiş:" + tablocuk[0] + "..." + tablocuk[tablocuk.Length - 1] + "\t->\t" + sonuc);
        }

        public void sonAsalMi(int[] tablocuk)
        {
            int threadSirasi = 0;
            sonuc = false;
            for (int i = 0; i < tablomsu.Length; ++i)
            {
                if (tablocuk[0] == tablomsu[i])
                {
                    threadSirasi = (int)i / threadEleman;
                    break;
                }
            }

            for (int i = tablocuk.Length-1; i >= 0; --i)
            {
                if (siradakiSayi % tablocuk[i] == 0)
                {
                    sonuc = true;
                    sonuclar.Add("\n"+threadSirasi+". Thread içerisindeki " + tablocuk[i] + " sayisina bölünmüştür.");
                    break;
                }
            }
            threadler.Add("\n" + threadSirasi + ". Thread başlangıç ve bitiş:" + tablocuk[tablocuk.Length-1] + "..." + tablocuk[0] + "\t->\t" + sonuc);
        }

        public int[] tabloYarat()
        {
            SqlConnection bag = new SqlConnection("Data Source=.; Initial Catalog=projeAsal; Integrated Security=SSPI");
            SqlCommand komut = new SqlCommand("SELECT * FROM asalSayilar", bag);
            SqlCommand satirKomut = new SqlCommand("SELECT COUNT(*) FROM asalSayilar", bag);
            bag.Open();
            int boyut = Convert.ToInt32(satirKomut.ExecuteScalar());
            int[] tabloDizi = new int[boyut];

            DataTable tablo = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(komut);
            da.Fill(tablo);

            for (int i = 0; i < boyut; ++i)
            {
                tabloDizi[i] = Convert.ToInt32(tablo.Rows[i]["sayilar"]);
            }

            bag.Close();
            return tabloDizi;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            main.Abort();
        }
    }
}
