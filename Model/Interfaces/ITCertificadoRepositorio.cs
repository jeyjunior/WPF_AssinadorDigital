using GerenciadorCertificados.Controller.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorCertificados.Interfaces
{
    public interface ITCertificadoRepositorio
    {
        bool Adicionar(TCertificado certificado);
        IEnumerable<TCertificado> ObterTabela();
    }
}
