using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using CampusBackend.Models;

namespace CampusBackend.Data
{
    public class CampusRepository
    {
        private readonly string _connectionString = "Data Source=campus.db";

        public CampusRepository()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            
            // Create table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Jobs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Company TEXT NOT NULL,
                    RequiredSkills TEXT NOT NULL
                )";
            command.ExecuteNonQuery();

            // Insert demo data if the table is empty
            command.CommandText = "SELECT COUNT(*) FROM Jobs";
            long count = Convert.ToInt64(command.ExecuteScalar());
            if (count == 0)
            {
                command.CommandText = @"
                    INSERT INTO Jobs (Title, Company, RequiredSkills) VALUES 
                    ('Frontend Developer', 'TechCorp', 'React, JavaScript, HTML, CSS'),
                    ('Backend Engineer', 'DataSys', 'C#, .NET, SQL'),
                    ('Full Stack Developer', 'WebSolutions', 'React, C#, SQL, JavaScript');
                ";
                command.ExecuteNonQuery();
            }
        }

        public List<Job> GetAllJobs()
        {
            var jobs = new List<Job>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Title, Company, RequiredSkills FROM Jobs";
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                jobs.Add(new Job {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Company = reader.GetString(2),
                    RequiredSkills = reader.GetString(3)
                });
            }
            return jobs;
        }
    }
}