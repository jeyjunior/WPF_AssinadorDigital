using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GerenciadorCertificados.View
{
    /// <summary>
    /// Interaction logic for ValidarAssinatura.xaml
    /// </summary>
    public partial class ValidarAssinatura : Window
    {
        string path = "";
        
        public ValidarAssinatura()
        {
            InitializeComponent();
        }

        private void dtgAssinatura_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dtgAssinaturas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnFecharTela_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCarregarPDF_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Selecione arquivo PDF";
            dialog.Filter = "Arquivos PDF|*.pdf";
            dialog.Multiselect = true;

            var result = dialog.ShowDialog();

            if (result != null && result is bool == false)
                return;

            path = dialog.FileName;
        }

        private void btnValidar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] pdfBytes = File.ReadAllBytes(path);

                SignedCms signedCms = new SignedCms();
                signedCms.Decode(pdfBytes);

                SignerInfoCollection signers = signedCms.SignerInfos;

                if (signers.Count > 0)
                {
                    foreach (SignerInfo signerInfo in signers)
                    {
                        X509Certificate2 signerCertificate = signerInfo.Certificate;
                    }
                }
                else
                {
                    MessageBox.Show("O PDF não está assinado digitalmente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao verificar a assinatura do PDF: {ex.Message}");
            }
        }
    }
}
