using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Web;
using System.Configuration;
using System.ComponentModel.DataAnnotations;

namespace ControleEstoque.Web.Models
{
    public class UsuarioModel
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Informe a senha")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Informe o nome")]
        public string Nome { get; set; }

        public static UsuarioModel ValidarUsuario(string login, string senha)
        {
            UsuarioModel ret = null;

            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(ConnectionString))
                {
                    con.Open();

                    using (MySqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = @"select * from usuario where login=@login and senha=@senha";
                        cmd.Parameters.Add(new MySqlParameter("login", login));
                        cmd.Parameters.Add(new MySqlParameter("senha", CriptoHelper.HashMD5(senha)));

                        //ret = (Convert.ToInt32(cmd.ExecuteScalar()) > 0);
                        var reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            ret = new UsuarioModel
                            {
                                Id = (int)reader["id"],
                                Login = (string)reader["login"],
                                Senha = (string)reader["senha"],
                                Nome = (string)reader["nome"]
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}", ex);
            }
            return ret;
        }


        public static int RecuperarQuantidade()
        {
            var ret = 0;

            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(ConnectionString))
                {
                    con.Open();

                    using (MySqlCommand cmd = con.CreateCommand())
                    {

                        cmd.CommandText = @"select count(*) from usuario";

                        ret = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}", ex);
            }
            return ret;
        }


        public static List<UsuarioModel> RecuperarLista(int pagina, int tamPagina)
        {
            var ret = new List<UsuarioModel>();

            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(ConnectionString))
                {
                    con.Open();

                    using (MySqlCommand cmd = con.CreateCommand())
                    {
                        var pos = (pagina - 1) * tamPagina;

                        cmd.CommandText = @"select * from usuario order by nome limit @limit offset @offset";
                        cmd.Parameters.Add(new MySqlParameter("limit", tamPagina));
                        cmd.Parameters.Add(new MySqlParameter("offset", pos));

                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ret.Add(new UsuarioModel
                            {
                                Id = (int)reader["id"],
                                Nome = (string)reader["nome"],
                                Login = (string)reader["login"]
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}", ex);
            }
            return ret;
        }



        public static UsuarioModel RecuperarPeloId(int id)
        {
            UsuarioModel ret = null;

            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(ConnectionString))
                {
                    con.Open();

                    using (MySqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = @"select * from usuario where id=@id";
                        cmd.Parameters.Add(new MySqlParameter("id", id));

                        var reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            ret = new UsuarioModel
                            {
                                Id = (int)reader["id"],
                                Login = (string)reader["login"],
                                Nome = (string)reader["nome"]
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}", ex);
            }
            return ret;
        }



        public static bool ExcluirPeloId(int id)
        {
            var ret = false;

            try
            {
                if (RecuperarPeloId(id) != null)
                {
                    string ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                    using (MySqlConnection con = new MySqlConnection(ConnectionString))
                    {
                        con.Open();

                        using (MySqlCommand cmd = con.CreateCommand())
                        {
                            cmd.CommandText = @"delete from usuario where id=@id";
                            cmd.Parameters.Add(new MySqlParameter("id", id));

                            ret = (cmd.ExecuteNonQuery() > 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}", ex);
            }
            return ret;
        }


        public int Salvar()
        {
            var ret = 0;
            try
            {
                var model = RecuperarPeloId(this.Id);
                string ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;

                using (MySqlConnection con = new MySqlConnection(ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = con.CreateCommand())
                    {
                        if (model == null)
                        {
                            cmd.CommandText = @"insert into usuario (nome, login, senha) values(@nome,@login,@senha);";
                            cmd.Parameters.Add(new MySqlParameter("nome", this.Nome));
                            cmd.Parameters.Add(new MySqlParameter("login", this.Login));
                            cmd.Parameters.Add(new MySqlParameter("senha", CriptoHelper.HashMD5(this.Senha)));
                            //cmd.LastInsertedId.ToString();

                            ret = cmd.ExecuteNonQuery();
                            ret = (int)cmd.LastInsertedId;
                            //ret = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        else
                        {
                            cmd.CommandText = @"update usuario set nome=@nome, login=@login" +
                                (!string.IsNullOrEmpty(this.Senha) ? ", senha=@senha" : "") +
                                " where id=@id;";
                            cmd.Parameters.Add(new MySqlParameter("id", this.Id));
                            cmd.Parameters.Add(new MySqlParameter("nome", this.Nome));
                            cmd.Parameters.Add(new MySqlParameter("login", this.Login));
                            if (!string.IsNullOrEmpty(this.Senha)){
                                cmd.Parameters.Add(new MySqlParameter("senha", CriptoHelper.HashMD5(this.Senha)));
                            }

                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                ret = this.Id;
                            }
                        }
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