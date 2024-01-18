using Dapper;
using GerenciadorCertificados.Entidades;
using GerenciadorCertificados.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorCertificados.Repositorio
{
    public class TCertificadoRepositorio : ITCertificadoRepositorio
    {
        private readonly string connectionString = @"Data Source=JEYJR;Initial Catalog=GECertificados;Integrated Security=True;";

        public TCertificadoRepositorio()
        {
            
        }

        public bool Adicionar(TCertificado certificado)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO TCertificado\n" +
                             " (NomeCertificado, CPF, Email, Senha, ChavePrivada, ChavePublica, Emissor, EmissorTipoO, DataValidade, Certificado)\n" +
                             " VALUES (@NomeCertificado, @CPF, @Email, @Senha, @ChavePrivada, @ChavePublica, @Emissor, @EmissorTipoO, @DataValidade, @Certificado)";

                var resultado = connection.Execute(sql, certificado);

                return Convert.ToInt32(resultado) > 0;
            }
        }

        public IEnumerable<TCertificado> ObterTabela()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = $"SELECT * FROM TCertificado";
                IEnumerable<TCertificado> resultado = connection.Query<TCertificado>(sql);
                return resultado;
            }
        }
    }
}
