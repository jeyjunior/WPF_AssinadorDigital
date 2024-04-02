using GerenciadorCertificados.Controller.Entidades;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
            dialog.Multiselect = false;

            var result = dialog.ShowDialog();

            if (result != null && result is bool == false)
                return;

            path = dialog.FileName;
        }

        private void btnValidar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var pdfBytes = File.ReadAllBytes(path);

                var signedCms = new SignedCms();
                signedCms.Decode(pdfBytes);

                SignerInfoCollection signers = signedCms.SignerInfos;

                if (signers.Count > 0)
                {
                    var signerCertificate = signers.OfType<SignerInfo>().Select(i => new TCertificado 
                    {
                        Nome = ExtrairNome(i.Certificate.Subject),
                        EmissorTipoO = GetEmissorTipoO(i.Certificate.IssuerName.Name),
                        Emissor = i.Certificate.Issuer,
                        DataAssinatura = i.Certificate.NotBefore,
                        DataValidade = i.Certificate.NotAfter,
                    });

                    if(signerCertificate != null && signerCertificate.Count() > 0)
                        dtgAssinaturas.ItemsSource = signerCertificate;
                }
                else
                {
                    MessageBox.Show("O PDF não está assinado digitalmente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"O PDF não está assinado digitalmente.");
            }
        }
    
        private string ExtrairNome(string subject)
        {
            var match = Regex.Match(subject, @"CN=([^,]+)");
            return match.Success ? match.Groups[1].Value : subject;
        }

        private string ExtrairCompania(string subject)
        {
            var match = Regex.Match(subject, @"O=([^,]+)");
            return match.Success ? match.Groups[1].Value : "";
        }

        private static string GetEmissorTipoO(string issuerName)
        {
            var match = Regex.Match(issuerName, @"O=([^,]+)");
            return match.Success ? match.Groups[1].Value : "";
        }
    }
}
