using BusinessLaag.Interfaces;
using BusinessLaag.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace DataLaag.Repositories
{
    public class VraagRepository : IVraagRepository
    {
        private readonly string _connectionString;

        public VraagRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public List<Vraag> GetVragenByOnderwerp(string onderwerp, bool alleenActief)
        {
            List<Vraag> vragen = new List<Vraag>();

            string sql = @"SELECT v.Id, v.InhoudTekst, v.Moeilijkheidsgraad, v.IsActief 
                           FROM Vragen v
                           JOIN VraagOnderwerpen vo ON v.Id = vo.VraagId
                           JOIN Onderwerpen o ON vo.OnderwerpId = o.Id
                           WHERE o.Naam = @onderwerp";

            if (alleenActief) sql += " AND v.IsActief = 1";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@onderwerp", onderwerp);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["Id"];
                        var opties = GetAntwoordenVoorVraag(id);

                        vragen.Add(new Vraag(
                            id,
                            onderwerp,
                            reader["InhoudTekst"].ToString(),
                            reader["Moeilijkheidsgraad"].ToString(),
                            (bool)reader["IsActief"],
                            opties
                        ));
                    }
                }
            }
            return vragen;
        }


        private List<MeerkeuzeOptie> GetAntwoordenVoorVraag(int vraagId)
        {
            List<MeerkeuzeOptie> opties = new List<MeerkeuzeOptie>();
            string sql = "SELECT Id, Inhoud, IsCorrect FROM Antwoorden WHERE VraagId = @vraagId";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@vraagId", vraagId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        opties.Add(new MeerkeuzeOptie(
                            (int)reader["Id"],
                            reader["Inhoud"].ToString(),
                            (bool)reader["IsCorrect"]
                        ));
                    }
                }
            }
            return opties;
        }


        public void VoegVraagToe(Vraag vraag)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {

                    string sqlCheck = "SELECT COUNT(*) FROM Vragen WHERE InhoudTekst = @tekst"; //hier eventueel zien of ik de levenstien algorithme kan gebruiken om robuuster te checken
                    SqlCommand cmdCheck = new SqlCommand(sqlCheck, conn, trans);
                    cmdCheck.Parameters.AddWithValue("@tekst", vraag.Inhoud);

                    int aantal = (int)cmdCheck.ExecuteScalar();
                    if (aantal > 0)
                    {

                        trans.Rollback();
                        return;
                    }


                    int onderwerpId = GetOfMaakOnderwerpId(vraag.Onderwerp, conn, trans);


                    string sqlVraag = "INSERT INTO Vragen (InhoudTekst, Moeilijkheidsgraad, IsActief) " +
                                      "OUTPUT INSERTED.Id VALUES (@inhoud, @graad, @actief)";
                    SqlCommand cmdVraag = new SqlCommand(sqlVraag, conn, trans);
                    cmdVraag.Parameters.AddWithValue("@inhoud", vraag.Inhoud);
                    cmdVraag.Parameters.AddWithValue("@graad", vraag.Moeilijkheidsgraad);
                    cmdVraag.Parameters.AddWithValue("@actief", vraag.IsActief);

                    int nieuweVraagId = (int)cmdVraag.ExecuteScalar();


                    string sqlLink = "INSERT INTO VraagOnderwerpen (VraagId, OnderwerpId) VALUES (@vId, @oId)";
                    SqlCommand cmdLink = new SqlCommand(sqlLink, conn, trans);
                    cmdLink.Parameters.AddWithValue("@vId", nieuweVraagId);
                    cmdLink.Parameters.AddWithValue("@oId", onderwerpId);
                    cmdLink.ExecuteNonQuery();


                    foreach (var optie in vraag.Keuzes)
                    {
                        string sqlAntwoord = "INSERT INTO Antwoorden (VraagId, Inhoud, IsCorrect) " +
                                             "VALUES (@vId, @tekst, @correct)";
                        SqlCommand cmdAntwoord = new SqlCommand(sqlAntwoord, conn, trans);
                        cmdAntwoord.Parameters.AddWithValue("@vId", nieuweVraagId);
                        cmdAntwoord.Parameters.AddWithValue("@tekst", optie.Tekst);
                        cmdAntwoord.Parameters.AddWithValue("@correct", optie.IsCorrect);
                        cmdAntwoord.ExecuteNonQuery();
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


        private int GetOfMaakOnderwerpId(string naam, SqlConnection conn, SqlTransaction trans)
        {
            string sqlSelect = "SELECT Id FROM Onderwerpen WHERE Naam = @naam";
            using (SqlCommand cmd = new SqlCommand(sqlSelect, conn, trans))
            {
                cmd.Parameters.AddWithValue("@naam", naam);
                object result = cmd.ExecuteScalar();
                if (result != null) return (int)result;
            }

            string sqlInsert = "INSERT INTO Onderwerpen (Naam) OUTPUT INSERTED.Id VALUES (@naam)";
            using (SqlCommand cmd = new SqlCommand(sqlInsert, conn, trans))
            {
                cmd.Parameters.AddWithValue("@naam", naam);
                return (int)cmd.ExecuteScalar();
            }
        }


        public void DeactiveerVraag(int vraagId)
        {
            string sql = "UPDATE Vragen SET IsActief = 0 WHERE Id = @id";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", vraagId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<string> GetAlleOnderwerpen()
        {
            List<string> onderwerpen = new List<string>();


            string sql = "SELECT Naam FROM Onderwerpen";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        onderwerpen.Add(reader["Naam"].ToString());
                    }
                }
            }

            return onderwerpen;
        }

        
    }
}