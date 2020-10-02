using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Xml;

namespace AssinaturaEletronica
{
    public partial class FrmPrincipal : Form
    {
        public FrmPrincipal()
        {
            InitializeComponent();
            Executar();
        }
        System.Timers.Timer timerMoveSprite = new System.Timers.Timer(); // Inicializa

        public void ChamaTime()
        {
            //MessageBox.Show("Iniciou");
            SairTime();
            //Definindo o timer
            timerMoveSprite.Interval = 9999;
            timerMoveSprite.Elapsed += new ElapsedEventHandler(MoveSprite);
            timerMoveSprite.Start();
            //MessageBox.Show("Iniciou");
        }

        public void MoveSprite(object source, ElapsedEventArgs e)
        {
            Executar();
        }

        public void SairTime()
        {
            timerMoveSprite.Stop();
        }

        private void Executar()
        {
            SairTime();
            try
            {
                

                System.Net.IPAddress[] ip = Dns.GetHostAddresses(Dns.GetHostName());
                string xmlUrl = "http://192.168.10.27/sistema/app/a3_ws/ws.php?xip=" + ip[1].ToString();
                XmlDocument oXML = new XmlDocument();
                oXML.Load(xmlUrl);

                string ACAO = null;
                string CAMINHO = null;
                string CAMINHOASSINADO = null;
                string CODIGO = null;

                ACAO = oXML.SelectSingleNode("a3").ChildNodes[0].InnerText;
                CAMINHO = oXML.SelectSingleNode("a3").ChildNodes[1].InnerText;
                CAMINHOASSINADO = oXML.SelectSingleNode("a3").ChildNodes[2].InnerText;
                CODIGO = oXML.SelectSingleNode("a3").ChildNodes[3].InnerText;

                if (ACAO == "1")
                {
                    string url_caminho = "http://192.168.10.27/sistema/app/a3_ws/ws_assinou.php?nCodigo=" + CODIGO;
                    Uri uri = new Uri(url_caminho);

                    WebClient wc = new WebClient();
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    wc.Encoding = Encoding.UTF8;
                    wc.UploadStringCompleted += new UploadStringCompletedEventHandler(wc_UploadStringCompleted);
                    wc.UploadStringAsync(uri, "POST", url_caminho.ToString());

                    var assinante = new Assinante();
                    assinante.Assinar(@"R:\nao_assinado\" + CAMINHO, @"R:\assinado\" + CAMINHOASSINADO);
                    File.Delete(@"R:\nao_assinado\" + CAMINHO);

                }
                ChamaTime();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Erro do sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ChamaTime();
            }
            
        }

        private void wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result);
        }

        private void BandejaWin_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            BandejaWin.Visible = false;
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        { 
            this.Visible = false;
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            BandejaWin.Visible = true;
            System.Net.IPAddress[] ip = Dns.GetHostAddresses(Dns.GetHostName());
            string xmlUrl = "http://192.168.10.27/sistema/app/a3_ws/ws.php?xip=" + ip[1].ToString();
           // MessageBox.Show(xmlUrl.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            BandejaWin.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            BandejaWin.Visible = true;
        }
    }
}
