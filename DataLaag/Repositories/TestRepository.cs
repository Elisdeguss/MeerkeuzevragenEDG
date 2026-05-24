using BusinessLaag.Interfaces;
using BusinessLaag.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataLaag.Repositories
{
    public class TestRepository : ITestRepository
    {
        private readonly string _connectionString;

        public TestRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

  
        public void OpslaanTest(string testCode, List<Vraag> vragen)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                   
                    string sqlCheck = "SELECT COUNT(*) FROM Testen WHERE Code = @code";
                    using (SqlCommand cmdCheck = new SqlCommand(sqlCheck, conn, trans))
                    {
                        cmdCheck.Parameters.AddWithValue("@code", testCode);
                        int aantal = (int)cmdCheck.ExecuteScalar();
                        if (aantal > 0)
                            throw new Exception("Deze testcode bestaat al! Kies een unieke code.");
                    }

                    
                    string sqlTest = "INSERT INTO Testen (Code, Datum) OUTPUT INSERTED.Id VALUES (@code, @datum)";
                    int nieuwTestId;

                    using (SqlCommand cmdTest = new SqlCommand(sqlTest, conn, trans))
                    {
                        cmdTest.Parameters.AddWithValue("@code", testCode);
                        cmdTest.Parameters.AddWithValue("@datum", DateTime.Now);
                        nieuwTestId = (int)cmdTest.ExecuteScalar();
                    }

                    
                    foreach (var vraag in vragen)
                    {
                        string sqlKoppel = "INSERT INTO TestVragen (TestId, VraagId) VALUES (@testId, @vraagId)";
                        using (SqlCommand cmdKoppel = new SqlCommand(sqlKoppel, conn, trans))
                        {
                            cmdKoppel.Parameters.AddWithValue("@testId", nieuwTestId);
                            cmdKoppel.Parameters.AddWithValue("@vraagId", vraag.Id);
                            cmdKoppel.ExecuteNonQuery();
                        }
                    }

                    trans.Commit();
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }


        public List<Test> GetAlleTests()
        {
            List<Test> alleTests = new List<Test>();
            string sql = "SELECT Id, Code, Datum FROM Testen ORDER BY Datum DESC";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            alleTests.Add(new Test(
                                (int)reader["Id"],
                                reader["Code"].ToString(),
                                (DateTime)reader["Datum"],
                                new List<Vraag>(),
                                new List<string>() 
                            ));
                        }
                    }
                }

               
                string sqlOnderwerpen = @"SELECT o.Naam 
                                  FROM Onderwerpen o
                                  JOIN TestOnderwerpen tond ON o.Id = tond.OnderwerpId
                                  WHERE tond.TestId = @testId";

                foreach (var test in alleTests)
                {
                    using (SqlCommand cmdOnd = new SqlCommand(sqlOnderwerpen, conn))
                    {
                        cmdOnd.Parameters.AddWithValue("@testId", test.Id);

                        using (SqlDataReader reader = cmdOnd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                test.Onderwerpen.Add(reader["Naam"].ToString());
                            }
                        }
                    }
                }
            }

            return alleTests;
        }


        public Test GetTestByCode(string testCode)
        {
            Test gevondenTest = null;
            List<Vraag> vragen = new List<Vraag>();

            string sqlTest = "SELECT Id, Code, Datum FROM Testen WHERE Code = @code";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                int testId;

                
                using (SqlCommand cmdTest = new SqlCommand(sqlTest, conn))
                {
                    cmdTest.Parameters.AddWithValue("@code", testCode);

                    using (SqlDataReader reader = cmdTest.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        testId = (int)reader["Id"];
                        gevondenTest = new Test(
                            testId,
                            reader["Code"].ToString(),
                            (DateTime)reader["Datum"],
                            new List<Vraag>(),
                            new List<string>()  
                        );
                    }
                }

                
                string sqlOnderwerpen = @"SELECT o.Naam 
                                  FROM Onderwerpen o
                                  JOIN TestOnderwerpen tond ON o.Id = tond.OnderwerpId
                                  WHERE tond.TestId = @testId";

                using (SqlCommand cmdOnd = new SqlCommand(sqlOnderwerpen, conn))
                {
                    cmdOnd.Parameters.AddWithValue("@testId", testId);

                    using (SqlDataReader reader = cmdOnd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            gevondenTest.Onderwerpen.Add(reader["Naam"].ToString());
                        }
                    }
                }

               
                string sqlVragen = @"SELECT v.Id, v.InhoudTekst, v.Moeilijkheidsgraad, v.IsActief,
                                    o.Naam AS Onderwerp
                             FROM Vragen v
                             JOIN TestVragen tv ON v.Id = tv.VraagId
                             JOIN VraagOnderwerpen vo ON v.Id = vo.VraagId
                             JOIN Onderwerpen o ON vo.OnderwerpId = o.Id
                             WHERE tv.TestId = @testId";

                using (SqlCommand cmdVragen = new SqlCommand(sqlVragen, conn))
                {
                    cmdVragen.Parameters.AddWithValue("@testId", testId);

                    using (SqlDataReader reader = cmdVragen.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            vragen.Add(new Vraag(
                                (int)reader["Id"],
                                reader["Onderwerp"].ToString(),
                                reader["InhoudTekst"].ToString(),
                                reader["Moeilijkheidsgraad"].ToString(),
                                (bool)reader["IsActief"],
                                new List<MeerkeuzeOptie>()
                            ));
                        }
                    }
                }

               
                foreach (var vraag in vragen)
                {
                    string sqlOpties = "SELECT Id, Inhoud, IsCorrect FROM Antwoorden WHERE VraagId = @vraagId";

                    using (SqlCommand cmdOpties = new SqlCommand(sqlOpties, conn))
                    {
                        cmdOpties.Parameters.AddWithValue("@vraagId", vraag.Id);

                        using (SqlDataReader reader = cmdOpties.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                vraag.Keuzes.Add(new MeerkeuzeOptie(
                                    (int)reader["Id"],
                                    reader["Inhoud"].ToString(),
                                    (bool)reader["IsCorrect"]
                                ));
                            }
                        }
                    }
                }

                gevondenTest.VragenInTest = vragen;
            }

            return gevondenTest;
        }

    }
}