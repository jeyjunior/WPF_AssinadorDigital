using Dapper;
using GerenciadorCertificados.Entidades;
using GerenciadorCertificados.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace GerenciadorCertificados.Repositorio
{
    public class TCertificadoRepositorio : TRepositorio, ITCertificadoRepositorio
    {
        private readonly string connectionString = @"Data Source=JEYJR;Initial Catalog=GECertificados;Integrated Security=True;";

        public TCertificadoRepositorio()
        {
            
        }

        public bool Adicionar(TCertificado certificado)
        {
            string sql = "  IF NOT EXISTS (SELECT 1 FROM TCertificado WHERE CPF = @CPF AND ChavePublica = @ChavePublica)" +
                         "  BEGIN" +
                         "    INSERT INTO TCertificado (NomeCertificado, CPF, Email, Senha, ChavePrivada, ChavePublica, Emissor, EmissorTipoO, DataValidade, Certificado)" +
                         "    VALUES (@NomeCertificado, @CPF, @Email, @Senha, @ChavePrivada, @ChavePublica, @Emissor, @EmissorTipoO, @DataValidade, @Certificado)" +
                         "  END";

            this.SQL = sql;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var resultado = connection.Execute(SQL, certificado);

                    if (resultado <= 0)
                    {
                        var resultado2 = ObterTabela(certificado);

                        if (resultado2.Count() > 0)
                        {
                            certificado.ValidationErrors.Add("Esse certificado já foi adicionado na base de dados");
                        }
                        else
                        {
                            certificado.ValidationErrors.Add("Falha ao tentar adicionar certificado, certifique-se que todos os dados estão corretos");
                        }

                        return false;
                    }

                    return true;
                }
            }
            catch(SqlException sqlException)
            {
                certificado.ValidationErrors.Add($"Erro: {sqlException.Message}");
                return false;
            }
            catch (Exception ex)
            {
                certificado.ValidationErrors.Add($"Erro: {ex.Message}");
                return false;
            }
        }

        public IEnumerable<TCertificado> ObterTabela()
        {
            return ObterTabela(new TCertificado());
        }

        public IEnumerable<TCertificado> ObterTabela(TCertificado certificado)
        {
            string where = "";

            if (certificado != null)
            {
                if (certificado.PK_Certificado > 0)
                {
                    where += (where.Trim() == "" ? " WHERE " : " AND ") + "PK_Certificado = @PK_Certificado";
                }

                if (!string.IsNullOrEmpty(certificado.NomeCertificado))
                {
                    where += (where.Trim() == "" ? " WHERE " : " AND ") + "NomeCertificado = @NomeCertificado COLLATE Latin1_General_CI_AI";
                }

                if (!string.IsNullOrEmpty(certificado.CPF))
                {
                    where += (where.Trim() == "" ? " WHERE " : " AND ") + "CPF = @CPF COLLATE Latin1_General_CI_AI";
                }

                if (!string.IsNullOrEmpty(certificado.ChavePublica))
                {
                    where += (where.Trim() == "" ? " WHERE " : " AND ") + "ChavePublica = @ChavePublica COLLATE Latin1_General_CI_AI";
                }
            }

            string sql = $"SELECT * FROM TCertificado {where}";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                IEnumerable<TCertificado> resultado = connection.Query<TCertificado>(sql, certificado);

                return resultado;
            }
        }

    }
}
