using BusinessLaag.Interfaces;
using BusinessLaag.Model;
using Microsoft.Data.SqlClient;

namespace DataLaag.Repositories
{
    public class ScoreRepository : IScoreRepository
    {
        private readonly string _connectionString;

        public ScoreRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public void SlaScoreOp(string studentNaam, int testId, int score)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {

                    int studentId;
                    string sqlStudent = "SELECT Id FROM Studenten WHERE Naam = @naam";
                    using (SqlCommand cmd = new SqlCommand(sqlStudent, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@naam", studentNaam);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {

                            studentId = (int)result;
                        }
                        else
                        {

                            string sqlNieuw = "INSERT INTO Studenten (Naam) OUTPUT INSERTED.Id VALUES (@naam)";
                            using (SqlCommand cmdNieuw = new SqlCommand(sqlNieuw, conn, trans))
                            {
                                cmdNieuw.Parameters.AddWithValue("@naam", studentNaam);
                                studentId = (int)cmdNieuw.ExecuteScalar();
                            }
                        }
                    }


                    string sqlResultaat = @"INSERT INTO TestResultaten (StudentId, TestId, Score, DatumUitvoering)
                                            VALUES (@studentId, @testId, @score, @datum)";
                    using (SqlCommand cmd = new SqlCommand(sqlResultaat, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@studentId", studentId);
                        cmd.Parameters.AddWithValue("@testId", testId);
                        cmd.Parameters.AddWithValue("@score", score);
                        cmd.Parameters.AddWithValue("@datum", DateTime.Now);
                        cmd.ExecuteNonQuery();
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


        public List<TestResultaat> GetAlleScores()
        {
            List<TestResultaat> resultaten = new List<TestResultaat>();

            string sql = @"SELECT tr.Id, tr.Score, tr.DatumUitvoering,
                                  s.Id AS StudentId, s.Naam AS StudentNaam,
                                  t.Id AS TestId, t.Code AS TestCode,
                                  (SELECT COUNT(*) FROM TestVragen WHERE TestId = t.Id) AS AantalVragen
                           FROM TestResultaten tr
                           JOIN Studenten s ON tr.StudentId = s.Id
                           JOIN Testen t ON tr.TestId = t.Id
                           ORDER BY tr.DatumUitvoering DESC";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var student = new Student(
                            (int)reader["StudentId"],
                            reader["StudentNaam"].ToString()
                        );


                        var test = new Test(
                            (int)reader["TestId"],
                            reader["TestCode"].ToString(),
                            DateTime.MinValue,
                            new List<Vraag>(),
                            new List<string>()
                        );

                        resultaten.Add(new TestResultaat(
                            (int)reader["Id"],
                            student,
                            test,
                            (int)reader["Score"],
                            (DateTime)reader["DatumUitvoering"],
                            (int)reader["AantalVragen"]
                        ));
                    }
                }
            }

            return resultaten;
        }
    }
}