using Albergo.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Albergo.Controllers
{
    public class PrenotazioniController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            List<Prenotazione> prenotazioni = new List<Prenotazione>();
            try
            {
                DB.conn.Open();
                var command = new SqlCommand(@"SELECT Prenotazione_ID, Nome , Cognome, Data_Pren, Data_Arrivo, Data_Partenza, Numero, Tipo as TipoPensione
                                      FROM Prenotazioni as p
                                      JOIN Ospiti as o ON o.Ospite_ID = p.Ospite_ID
                                      JOIN Pensioni as pe ON pe.Pensione_ID = p.Pensione_ID
                                      JOIN Camere as c ON c.Camera_ID = p.Camera_ID", DB.conn);
                var reader = command.ExecuteReader();


                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var prenot = new Prenotazione();
                        prenot.Prenotazione_ID = (int)reader["Prenotazione_ID"];
                        prenot.Ospite = new Ospite
                        {
                            Nome = reader["Nome"].ToString(),
                            Cognome = reader["Cognome"].ToString()
                        };
                        prenot.Data_Pren = (DateTime)reader["Data_Pren"];
                        prenot.Data_Arrivo = (DateTime)reader["Data_Arrivo"];
                        prenot.Data_Partenza = (DateTime)reader["Data_Partenza"];
                        prenot.Camera = new Camera { Numero = (int)reader["Numero"] };
                        prenot.Pensione = new Pensione { Tipo = reader["TipoPensione"].ToString() };
                        prenotazioni.Add(prenot);
                    }
                    reader.Close();
                }

            }
            catch
            {

            }
            finally
            {
                DB.conn.Close();

            }

            return View(prenotazioni);

        }

        [Authorize]
        public ActionResult Details(int id)
        {
            //inizializzo prenotazione,lista servizi, totservizi, numero notti
            var pren = new Prenotazione();
            List<PS> servizi = new List<PS>();
            decimal TotServizi = 0;
            int nottiN = 0;

            try
            {
                DB.conn.Open();
                //query per selezionare tutto quanto 
                var command = new SqlCommand(@"SELECT *, o.Nome AS NomeOspite, o.Cognome AS CognomeOspite
                                             FROM Prenotazioni AS p
                                             JOIN Pensioni AS pe ON pe.Pensione_ID = p.Pensione_ID
                                             JOIN Camere AS c ON c.Camera_ID = p.Camera_ID
                                             JOIN Camere AS cam ON cam.Camera_ID = p.Camera_ID
                                             JOIN Categorie AS cat ON cat.Categoria_ID = cam.Categoria_ID
                                             JOIN Ospiti AS o ON o.Ospite_ID = p.Ospite_ID
                                             WHERE Prenotazione_ID=@id", DB.conn);
                command.Parameters.AddWithValue("id", id);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();

                    // Calcola il numero di notti
                    DateTime dataPartenza = (DateTime)reader["Data_Partenza"];
                    DateTime dataArrivo = (DateTime)reader["Data_Arrivo"];
                    TimeSpan difference = dataPartenza - dataArrivo;
                    nottiN = difference.Days;

                    pren.Prenotazione_ID = (int)reader["Prenotazione_ID"];
                    pren.Ospite = new Ospite
                    {
                        Nome = reader["Nome"].ToString(),
                        Cognome = reader["Cognome"].ToString()
                    };



                    pren.Camera = new Camera
                    {
                        Numero = (int)reader["Numero"],
                        Categoria = new Categoria
                        {
                            Caparra = (decimal)reader["Caparra"],
                            TariffaNotte = (decimal)reader["TariffaNotte"]

                        }
                    };

                    pren.Data_Arrivo = (DateTime)reader["Data_Arrivo"];
                    pren.Data_Partenza = (DateTime)reader["Data_Partenza"];
                    pren.Pensione = new Pensione
                    {
                        Tipo = reader["Tipo"].ToString(),
                        Supplemento = (decimal)reader["Supplemento"]
                    };
                }
                reader.Close();

                //richiedo tutti i servizi
                var comServizi = new SqlCommand(@"SELECT *
                                  FROM PS as p
                                  JOIN Servizi as s ON s.Servizio_ID = p.Servizio_ID
                                  WHERE p.Prenotazione_ID=@id", DB.conn);
                comServizi.Parameters.AddWithValue("@id", id);
                var readerServizi = comServizi.ExecuteReader();
                if (readerServizi.HasRows)
                {
                    while (readerServizi.Read())
                    {
                        var servizio = new PS();
                        servizio.Data_Serv = (DateTime)readerServizi["Data_Serv"];
                        servizio.Quantita = (int)readerServizi["Quantita"];
                        servizio.PrezzoServ = (decimal)readerServizi["PrezzoServ"];
                        servizio.Servizio = new Servizio { Tipo = readerServizi["Tipo"].ToString() };
                        servizi.Add(servizio);
                    }
                    readerServizi.Close();
                }
                foreach (var servizio in servizi)
                {
                    TotServizi += servizio.PrezzoServ;
                }
                //calcola totale prenotazione
                var totalePrenotazione = ((pren.Camera.Categoria.TariffaNotte + pren.Pensione.Supplemento) * nottiN) - pren.Camera.Categoria.Caparra + TotServizi;

                pren.Checkout = new Checkout
                {
                    Notti = nottiN,
                    TotServizi = TotServizi,
                    TotPren = totalePrenotazione
                };
            }
            catch
            {

            }
            finally
            {
                DB.conn.Close();
            }

            return View(pren);
        }

    }
}