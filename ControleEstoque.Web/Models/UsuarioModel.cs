using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Web;

namespace ControleEstoque.Web.Models
{
    public class UsuarioModel
    {
        public static bool ValidarUsuario(string login, string senha)
        {
            var ret = false;

            try
            {
                string ConnectionString = "Server='localhost';User='root';Password='123456';Database='controle_estoque';SslMode=none";
                using (MySqlConnection con = new MySqlConnection(ConnectionString))
                {
                    con.Open();

                    using (MySqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = @"select count(*) from usuario where login=@login and senha=@senha";
                        cmd.Parameters.Add(new MySqlParameter("login", login));
                        cmd.Parameters.Add(new MySqlParameter("senha", senha));

                        ret = (Convert.ToInt32(cmd.ExecuteScalar()) > 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}", ex);
            }
            return ret;
        }
    }
}