using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdsAuth.Data
{
    public class AdRepository
    {
        private string _connectionString;

        public AdRepository(string cs)
        {
            _connectionString = cs;
        }

        public void AddAd(Ad ad)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Ads (UserId,PhoneNumber,Ad,Date) VALUES (@userId,@phoneNumber,@ad,@date) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@userId", ad.UserId);
            cmd.Parameters.AddWithValue("@phoneNumber", ad.PhoneNumber);
            cmd.Parameters.AddWithValue("@ad", ad.Listing);
            cmd.Parameters.AddWithValue("@date", DateTime.Now.ToShortDateString());
            connection.Open();
            ad.Id = (int)(decimal)cmd.ExecuteScalar();
        }
        public User GetByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Users WHERE UserEmail = @email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new User
            {
                Id = (int)reader["Id"],
                Email = (string)reader["UserEmail"],
                Name = (string)reader["UserName"],
                Password = (string)reader["Password"]
            };
        }

        public void AddUser(User user, string password)
        {

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO USERS (UserName,UserEmail,Password) VALUES (@userName,@email,@password)";
            cmd.Parameters.AddWithValue("@userName", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@password", hashedPassword);
            connection.Open();
            cmd.ExecuteNonQuery();

        }

        public User Login(string email, string password)
        {

            var user = GetByEmail(email);

            if(user == null)
            {
                return null;
            }


            bool validPassword = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!validPassword)
            {
                return null;
            }

            return user;
        }

        public List<Ad> GetAllAds()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Ads";
            List<Ad> ads = new();
            connection.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new()
                {
                    Id = (int)reader["Id"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Listing = (string)reader["Ad"],
                    Date = (DateTime)reader["Date"],
                    UserId = (int)reader["UserId"]
                });
            }
            return ads;
        }

        public void DeleteAd(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM ADS WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public List<Ad> GetAllAdsForMyAccount(string email)
        {

            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            var user = GetByEmail(email);
            cmd.CommandText = "SELECT * FROM Ads WHERE UserId = @id";
            cmd.Parameters.AddWithValue("@id", user.Id);
            List<Ad> ads = new();
            connection.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new()
                {
                    Id = (int)reader["Id"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Listing = (string)reader["Ad"],
                    Date = (DateTime)reader["Date"],
                    UserId = (int)reader["UserId"]
                });
            }
            return ads;

        }


    }
}

