using GerenciadorCertificados.Entidades;
using GerenciadorCertificados.Interfaces;
using Microsoft.Win32;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GerenciadorCertificados
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ITCertificadoRepositorio tCertificadoRepositorio;

        public MainWindow()
        {
            InitializeComponent();

            tCertificadoRepositorio = DISetup.DISetup.Container.GetInstance<ITCertificadoRepositorio>();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnAdicionarCertificado_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            dialog.Title = "Selecione um certificado";
            dialog.Filter = "Certificados (*.pfx;*.cer;*.pkcs12;*.pkcs7)|*.pfx;*.cer;*.pkcs12;*.pkcs7";

            var resultado = dialog.ShowDialog();

            if (resultado != true)
                return;

            var certificadoByte = File.ReadAllBytes(dialog.FileName);
            var x509 = new X509Certificate2(certificadoByte, "1234");

            var certificado = new TCertificado()
            {
                NomeCertificado = x509.FriendlyName,
                CPF = "1234567890",
                EmissorTipoO = "Lacuna Software",
                Certificado = certificadoByte,
                Emissor = "Lacuna CA Test v1",
                ChavePublica = x509.GetPublicKeyString(),
                Email = "teste@teste.com.br",
                DataValidade = DateTime.Today.AddDays(180),
                Senha = "1234",
                ChavePrivada = null,
            };

            var result = tCertificadoRepositorio.Adicionar(certificado);

            MessageBox.Show(result ? $"Certificado adicionado com sucesso!" : "Falha ao salvar certificado");
        }

        private void btnAdicionarCertificadoInstalado_Click(object sender, RoutedEventArgs e)
        {
            X509Store x509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            x509Store.Open(OpenFlags.ReadOnly);

            var x509 = x509Store.Certificates.Where(i => i.Subject.Contains("Wayne Enterprises, Inc")).FirstOrDefault();

            if (x509 != null)
            {
                var certificado = new TCertificado()
                {
                    NomeCertificado = x509.FriendlyName,
                    CPF = "1234567890",
                    EmissorTipoO = "Lacuna Software",
                    Certificado = x509.Export(X509ContentType.Pkcs12),
                    Emissor = "Lacuna CA Test v1",
                    ChavePublica = x509.GetPublicKeyString(),
                    Email = "teste@teste.com.br",
                    DataValidade = DateTime.Today.AddDays(180),
                    Senha = "1234",
                    ChavePrivada = null,
                };

                var result = tCertificadoRepositorio.Adicionar(certificado);

                MessageBox.Show(result ? $"Certificado adicionado com sucesso!" : "Falha ao salvar certificado");
            }
        }


        private void btnAtualizarGrid_Click(object sender, RoutedEventArgs e)
        {
            var certificadoCollection = tCertificadoRepositorio.ObterTabela();

            if (certificadoCollection == null) return;

            dtgCertificados.ItemsSource = certificadoCollection;
        }

        private void btnAssinarPDF_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}