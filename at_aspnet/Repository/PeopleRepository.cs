using at_aspnet.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Data;

namespace at_aspnet.Repository
{
    public class PeopleRepository
    {
        private string ConnectionString { get; set; }
        public PeopleRepository(IConfiguration configuration)
        {
            this.ConnectionString = configuration.GetConnectionString("PeopleConnection");
        }

        public void Save(Person person)
        {
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                var sql = @"INSERT INTO PEOPLE(Id,FirstName,Surname,Birthday,PersonAge) VALUES (@P1, @P2, @P3, @P4, @P5)";
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                SqlCommand sqlCommand = connection.CreateCommand();
                sqlCommand.CommandText = sql;
                sqlCommand.Parameters.AddWithValue("P1", person.Id);
                sqlCommand.Parameters.AddWithValue("P2", person.FirstName);
                sqlCommand.Parameters.AddWithValue("P3", person.Surname);
                sqlCommand.Parameters.AddWithValue("P4", person.Birthday);
                sqlCommand.Parameters.AddWithValue("P5", person.Age);

                sqlCommand.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void Update(Person person)
        {
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                var sql = @"UPDATE PEOPLE SET FirstName = @P1, Surname = @P2, Birthday = @P3, PersonAge = @P4 WHERE Id = @P5";

                if (connection.State != ConnectionState.Open) { connection.Open(); }

                SqlCommand sqlCommand = connection.CreateCommand();
                sqlCommand.CommandText = sql;
                sqlCommand.Parameters.AddWithValue("P1", person.FirstName);
                sqlCommand.Parameters.AddWithValue("P2", person.Surname);
                sqlCommand.Parameters.AddWithValue("P3", person.Birthday);
                sqlCommand.Parameters.AddWithValue("P4", person.Age);
                sqlCommand.Parameters.AddWithValue("P5", person.Id);
                sqlCommand.ExecuteNonQuery();

                connection.Close();
            }
        }

        public void Delete(Guid id)
        {
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                var sql = @"DELETE FROM PEOPLE WHERE Id = @P1";

                if (connection.State != ConnectionState.Open) { connection.Open(); };

                SqlCommand sqlCommand = connection.CreateCommand();
                sqlCommand.CommandText = sql;
                sqlCommand.Parameters.AddWithValue("P1", id);
                sqlCommand.ExecuteNonQuery();

                connection.Close();
            }
        }

        public List<Person> GetAll()
        {
            List<Person> result = new List<Person>();
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                var sql = @"SELECT ID, FirstName, Surname, Birthday, PersonAge FROM PEOPLE";
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                SqlCommand sqlCommand = connection.CreateCommand();
                sqlCommand.CommandText = sql;
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    Person person = new Person()
                    {
                        Id = reader.GetGuid("Id"),
                        FirstName = reader.GetString("FirstName"),
                        Surname = reader.GetString("Surname"),
                        Birthday = reader.GetDateTime("Birthday"),
                        Age = reader.GetInt32("PersonAge")
                    };
                    result.Add(person);
                }
                connection.Close();
            }
            return result;
        }

        public Person GetById(Guid id)
        {
            List<Person> result = new List<Person>();
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                var sql = @"SELECT Id, FirstName, Surname, Birthday, PersonAge FROM PEOPLE WHERE Id = @P1";
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                SqlCommand sqlCommand = connection.CreateCommand();
                sqlCommand.CommandText = sql;
                sqlCommand.Parameters.AddWithValue("P1", id);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    Person person = new Person()
                    {
                        Id = reader.GetGuid("Id"),
                        FirstName = reader.GetString("FirstName"),
                        Surname = reader.GetString("Surname"),
                        Birthday = reader.GetDateTime("Birthday"),
                        Age = reader.GetInt32("PersonAge")
                    };
                    result.Add(person);
                }
                connection.Close();
            }
            return result.FirstOrDefault();
        }

        public List<Person> SearchPeopleDatabase(string firstName, string surname) 
        {
            List<Person> result = new List<Person>();
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                var sql = $"SELECT * FROM People WHERE FirstName LIKE @P1 OR Surname LIKE @P2";
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                SqlCommand sqlCommand = connection.CreateCommand();
                sqlCommand.CommandText = sql;
                sqlCommand.Parameters.AddWithValue("P1", '%' + firstName + '%');
                sqlCommand.Parameters.AddWithValue("P2", '%' + surname + '%');
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    Person person = new Person()
                    {
                        Id = reader.GetGuid("Id"),
                        FirstName = reader.GetString("FirstName"),
                        Surname = reader.GetString("Surname"),
                        Birthday = reader.GetDateTime("Birthday"),
                        Age = reader.GetInt32("PersonAge")
                    };
                    result.Add(person);
                }
                connection.Close();
            }
            return result;
        }
    }
}
