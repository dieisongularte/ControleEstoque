using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ControleEstoque.Web.Models
{
    public class GrupoProdutoModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Preencha o nome")]
        public string Nome { get; set; }

        public bool Ativo { get; set; }


        public static List<GrupoProdutoModel> RecuperarLista()
        {
            var ret = new List<GrupoProdutoModel>();

            try
            {
                string ConnectionString = "Server='localhost';User='root';Password='123456';Database='controle_estoque';SslMode=none";
                using (MySqlConnection con = new MySqlConnection(ConnectionString))
                {
                    con.Open();

                    using (MySqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = @"select *, rtrim(nome) from grupo_produto order by nome";
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ret.Add(new GrupoProdutoModel
                            {
                                Id = (int)reader["id"],
                                Nome = (string)reader["nome"],
                                Ativo = Convert.ToBoolean(reader["ativo"])
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



        public static GrupoProdutoModel RecuperarPeloId(int id)
        {
            GrupoProdutoModel ret = null;

            try
            {
                string ConnectionString = "Server='localhost';User='root';Password='123456';Database='controle_estoque';SslMode=none";
                using (MySqlConnection con = new MySqlConnection(ConnectionString))
                {
                    con.Open();

                    using (MySqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = @"select * from grupo_produto where id=@id";
                        cmd.Parameters.Add(new MySqlParameter("id", id));

                        var reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            ret = new GrupoProdutoModel
                            {
                                Id = (int)reader["id"],
                                Nome = (string)reader["nome"],
                                Ativo = Convert.ToBoolean(reader["ativo"])
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
                    string ConnectionString = "Server='localhost';User='root';Password='123456';Database='controle_estoque';SslMode=none";
                    using (MySqlConnection con = new MySqlConnection(ConnectionString))
                    {
                        con.Open();

                        using (MySqlCommand cmd = con.CreateCommand())
                        {
                            cmd.CommandText = @"delete * from grupo_produto where id=@id";
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
                string ConnectionString = "Server='localhost';User='root';Password='123456';Database='controle_estoque';SslMode=none";

                using (MySqlConnection con = new MySqlConnection(ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = con.CreateCommand())
                    {
                        if (model == null)
                        {
                            cmd.CommandText = @"insert into grupo_produto (nome, ativo) values(@nome,@ativo); select convert(scope_identity(),int)";
                            cmd.Parameters.Add(new MySqlParameter("nome", this.Nome));
                            cmd.Parameters.Add(new MySqlParameter("ativo", this.Ativo));

                            ret = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        else
                        {
                            cmd.CommandText = @"update grupo_produto set nome=@nome, ativo=@ativo where id=@id;";
                            cmd.Parameters.Add(new MySqlParameter("id", this.Id));
                            cmd.Parameters.Add(new MySqlParameter("nome", this.Nome));
                            cmd.Parameters.Add(new MySqlParameter("ativo", this.Ativo));

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