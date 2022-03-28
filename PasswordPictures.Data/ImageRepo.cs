using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PasswordPictures.Data
{
    public class ImageRepo
    {
        private string _connString;
        public ImageRepo(string connString)
        {
            _connString = connString;
        }

        public int AddImage(Image image)
        {
            using var conn = new SqlConnection(_connString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = ("INSERT INTO Images (FileName, Password, Views) " +
                                "VALUES(@fileName, @password, @views) " +
                                "SELECT SCOPE_IDENTITY()");
            cmd.Parameters.AddWithValue("@fileName", image.FileName);
            cmd.Parameters.AddWithValue("@password", image.Password);
            cmd.Parameters.AddWithValue("@views", image.Views);
            conn.Open();
            return (int)(decimal)cmd.ExecuteScalar();
        }

        public Image GetImage(int id)
        {
            using var connection = new SqlConnection(_connString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Images WHERE ID = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            return new Image
            {
                Id = (int)reader["Id"],
                FileName = (string)reader["FileName"],
                Password = (string)reader["Password"],
                Views = (int)reader["Views"]
            };
        }


        public string GetPassword(int id)
        {
            using var connection = new SqlConnection(_connString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Password FROM Images WHERE ID = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            return (string)cmd.ExecuteScalar();
        }

        [HttpPost]
        public void IncrementViews(int id)
        {
            using var connection = new SqlConnection(_connString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Images SET ViewCount += 1 WHERE ID = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
