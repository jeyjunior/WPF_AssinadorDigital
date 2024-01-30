﻿using GerenciadorCertificados.Entidades;
using GerenciadorCertificados.Interfaces;
using GerenciadorCertificados.Model;
using Microsoft.Win32;
using System.Data;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.Pkcs;
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
    public partial class MainWindow : Window
    {
        private readonly ITCertificadoRepositorio tCertificadoRepositorio;
        private List<PDF> pDFCollectin;
        private List<TCertificado> certificadoCollectin;

        public MainWindow()
        {
            InitializeComponent();

            tCertificadoRepositorio = DISetup.DISetup.Container.GetInstance<ITCertificadoRepositorio>();
            pDFCollectin = new List<PDF>();
        }

        #region Eventos
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnAdicionarCertificado_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog();

                dialog.Title = "Selecione um certificado";
                dialog.Filter = "Certificados (*.pfx;*.cer;*.pkcs12;*.pkcs7)|*.pfx;*.cer;*.pkcs12;*.pkcs7";
                dialog.Multiselect = true;

                var resultado = dialog.ShowDialog();

                if (resultado != true)
                    return;

                var arquivos = dialog.FileNames;

                var certificados = arquivos.Select(i => File.ReadAllBytes(i))
                                           .Select(i => new X509Certificate2(i, "1234"))
                                           .Select(i => new TCertificado
                                           {
                                               Nome = i.FriendlyName,
                                               CPF = "1234567890",
                                               EmissorTipoO = "Lacuna Software",
                                               Certificado = i.GetRawCertData(),
                                               Emissor = "Lacuna CA Test v1",
                                               ChavePublica = i.GetPublicKeyString(),
                                               Email = "teste@teste.com.br",
                                               DataValidade = DateTime.Today.AddDays(180),
                                               Senha = "1234",
                                               ChavePrivada = null,
                                           })
                                           .ToList();

                int qtd = 0;

                for (int i = 0; i < certificados.Count; i++)
                {
                    var result = tCertificadoRepositorio.Adicionar(certificados[i]);

                    if (result)
                        qtd++;
                }

                if (qtd == 1)
                {
                    MessageBox.Show("Um certificado foi adicionado com sucesso!");
                }
                else if(qtd > 1)
                {
                    MessageBox.Show($"{qtd} certificados foram adicionados com sucesso!");
                }
                else
                {

                    var erro = certificados.Where(i => i.ValidationErrors.Count > 0).Select(i => i.ValidationErrors[0]).ToList();

                    if(erro.Count() > 0)
                    {
                        string msgErros = "";

                        for (int i = 0; i < erro.Count(); i++)
                        {
                            msgErros += erro[i].ToString() + "\n";  
                        }

                        throw new Exception(msgErros);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdicionarCertificadoInstalado_Click(object sender, RoutedEventArgs e)
        {
            X509Store x509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            x509Store.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection x509Collection = x509Store.Certificates;
            
            certificadoCollectin = x509Collection
                                        .Select(i => new TCertificado 
                                        {
                                            Nome = i.FriendlyName.Trim() != "" ? i.FriendlyName : i.Issuer.Replace("CN=",""),
                                            CPF = "1234567890",
                                            EmissorTipoO = "Lacuna Software",
                                            Emissor = "Lacuna CA Test v1",
                                            ChavePublica = i.GetPublicKeyString(),
                                            Email = "teste@teste.com.br",
                                            DataValidade = DateTime.Today.AddDays(180),
                                            Senha = "1234",
                                            ChavePrivada = null,
                                            Certificado = i.Export(X509ContentType.Pkcs12),
                                        }).ToList();

            dtgCertificados.ItemsSource = certificadoCollectin;
        }

        private void btnAtualizarGrid_Click(object sender, RoutedEventArgs e)
        {
            var certificadoCollection = tCertificadoRepositorio.ObterTabela();

            if (certificadoCollection == null) 
            {
                MessageBox.Show("Nenhum certificado encontrado");
            };

            dtgCertificados.ItemsSource = certificadoCollection;
        }

        private void btnAssinarPDF_Click(object sender, RoutedEventArgs e)
        {
            var certificadoSelecionado = (TCertificado)dtgCertificados.SelectedItem;
            var certificado = certificadoCollectin.FirstOrDefault(i => i.Nome == certificadoSelecionado.Nome);
            var arquivoSelecionado = (PDF)dtgPDF.SelectedItem;
            
            if (certificado == null)
                return;


            var arquivoBytes = File.ReadAllBytes(arquivoSelecionado.Caminho);
            ContentInfo pdf = new ContentInfo(arquivoBytes);
            SignedCms signedCms = new SignedCms(pdf);

            var cert = new X509Certificate2(certificado.Certificado);
            CmsSigner cmsSigner = new CmsSigner(cert);
            cmsSigner.IncludeOption = X509IncludeOption.WholeChain;

            signedCms.ComputeSignature(cmsSigner);

            byte[] signature = signedCms.Encode();

            File.WriteAllBytes(arquivoSelecionado.Caminho, signature);

            File.WriteAllBytes("arquivo_assinado.pdf", signature);
        }

        private void btnAdicionarPDF_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Selecione arquivo PDF";
            dialog.Filter = "Arquivos PDF|*.pdf";
            dialog.Multiselect = true;

            var result = dialog.ShowDialog();

            if (result != null && result is bool == false)
                return;

            var arquivos = dialog.FileNames;

            foreach (var item in arquivos)
            {
                //C:\Users\jeyjr\Desktop\del\pdf para assinar\001.pdf
                var indiceNome = item.LastIndexOf("\\");
                var indiceFormato = item.LastIndexOf(".");
                
                var nome = item.Substring(indiceNome + 1);
                var formato = item.Substring(indiceFormato + 1);

                var pdf = new PDF
                {
                    Caminho = item,
                    Nome = nome,
                    Formato = formato,
                    Assinado = false
                };

                pDFCollectin.Add(pdf);
            }

            dtgPDF.ItemsSource = pDFCollectin;
            //Selecionar o documento (Referencia em memoria)
            //Exibir dados do documento selecionado no dtg
        }

        private void btnVerificarAssinatura_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dtgCertificados_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ArquivoSelecionado_SelectionChanged<TCertificado>(dtgCertificados, lblCertificadoSelecionado, "Certificado: ");
        }

        private void dtgPDF_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ArquivoSelecionado_SelectionChanged<PDF>(dtgPDF, lblPDFSelecionado, "Arquivo: ");
        }
        #endregion


        #region Metodos
        private void ArquivoSelecionado_SelectionChanged<T>(DataGrid dtg, Label lbl, string content) where T : class
        {
            if(dtg ==  null || lbl == null) 
                return;

            if(dtg.Items.Count <= 0)
            {
                lbl.Content = content;
                return;
            }

            var item = dtg.SelectedItem as T;

            TextBlock textBlock = new TextBlock
            {
                Style = (Style)Resources["lblItemSelecionado"],
            };

            if (item != null)
                textBlock.Text = $"{content} {item.GetType().GetProperty("Nome").GetValue(item).ToString()}";
            else
                textBlock.Text = $"{content}";

            lbl.Content = textBlock;
        }
        #endregion
    }
}