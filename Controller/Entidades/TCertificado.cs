using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GerenciadorCertificados.Controller.Entidades
{
    public class TCertificado
    {
        public int PK_Certificado { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public byte[] ChavePrivada { get; set; }
        public string ChavePublica { get; set; }
        public string Emissor { get; set; }
        public string EmissorTipoO { get; set; }
        public DateTime? DataValidade { get; set; }
        public DateTime? DataAssinatura { get; set; }
        public byte[] Certificado { get; set; }

        public List<string> ValidationErrors { get; set; } = new List<string>();
        public bool IsValid
        {
            get
            {
                if (CPF != "" && CPF.Length > 11)
                    ValidationErrors.Add("CPF Inválido");

                return ValidationErrors.Count == 0;
            }
        }
    }
}
