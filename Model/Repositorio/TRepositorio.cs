using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorCertificados.Repositorio
{
    public class TRepositorio
    {
        private string sqlBase = "";
        public string SQL { get => TratametoSQL(sqlBase); set => sqlBase = value; }

        private string TratametoSQL(string sql)
        {
            string tratrameto = $"BEGIN TRY\n {sql}\n END TRY\n BEGIN CATCH\n SELECT  ERROR_MESSAGE() AS ErrorMessage\n END CATCH";

            return tratrameto;
        }
    }
}
