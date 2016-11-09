using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DynamicQuery
{
    public static class DatabaseExtension
    {
        public static IEnumerable<dynamic> DynamicSqlQuery(this Database database, string sql, params object[] parameters)
        {

            List<dynamic> retorno = new List<dynamic>();

            using (System.Data.IDbCommand command = database.Connection.CreateCommand())
            {
                try
                {
                    database.Connection.Open();
                    command.CommandText = sql;
                    command.CommandTimeout = command.Connection.ConnectionTimeout;
                    foreach (var param in parameters)
                    {
                        command.Parameters.Add(param);
                    }

                    using (System.Data.IDataReader reader = command.ExecuteReader())
                    {
                        var schema = reader.GetSchemaTable();

                        while (reader.Read())
                        {
                            dynamic obj = new JObject();
                            foreach (System.Data.DataRow row in schema.Rows)
                            {
                                string name = (string)row["ColumnName"];
                                obj[name] = reader[name].ToString();
                            }
                            retorno.Add(obj);
                        }
                    }
                }
                finally
                {
                    database.Connection.Close();
                    command.Parameters.Clear();
                }
            }

            return retorno;
        }
        
    }
}
