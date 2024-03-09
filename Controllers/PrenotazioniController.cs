﻿using Albergo.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
                        var pren = new Prenotazione();
                        pren.Prenotazione_ID = (int)reader["Prenotazione_ID"];
                        pren.Ospite = new Ospite
                        {
                            Nome = reader["Nome"].ToString(),
                            Cognome = reader["Cognome"].ToString()
                        };

                        pren.Data_Pren = (DateTime)reader["Data_Pren"];
                        pren.Data_Arrivo = (DateTime)reader["Data_Arrivo"];
                        pren.Data_Partenza = (DateTime)reader["Data_Partenza"];
                        pren.Camera = new Camera { Numero = (int)reader["Numero"] };
                        pren.Pensione = new Pensione { Tipo = reader["TipoPensione"].ToString() };
                        prenotazioni.Add(pren);
                    }
                    reader.Close();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

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
            var pren = new Prenotazione();
            List<PS> servizi = new List<PS>();
            decimal TotServizi = 0;
            int nottiN = 0;

            try
            {
                DB.conn.Open();
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

                var totalePrenotazione = ((pren.Camera.Categoria.TariffaNotte + pren.Pensione.Supplemento) * nottiN) - pren.Camera.Categoria.Caparra + TotServizi;

                pren.Checkout = new Checkout
                {
                    Notti = nottiN,
                    TotServizi = TotServizi,
                    TotPren = totalePrenotazione
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                DB.conn.Close();
            }

            return View(pren);
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddServizi(int id)
        {
            var pren = new Prenotazione();
            List<PS> servizi = new List<PS>();
            decimal TotServizi = 0;
            int nottiN = 0;

            try
            {
                DB.conn.Open();
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

                var totalePrenotazione = ((pren.Camera.Categoria.TariffaNotte + pren.Pensione.Supplemento) * nottiN) - pren.Camera.Categoria.Caparra + TotServizi;

                pren.Checkout = new Checkout
                {
                    Notti = nottiN,
                    TotServizi = TotServizi,
                    TotPren = totalePrenotazione
                };



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                DB.conn.Close();
            }

            try
            {
                DB.conn.Open();
                List<Servizio> lista = new List<Servizio>();
                var cmd = new SqlCommand("SELECT * FROM Servizi", DB.conn);
                var listaservizi = cmd.ExecuteReader();

                if (listaservizi.HasRows)
                {
                    while (listaservizi.Read())
                    {
                        var servizio = new Servizio();
                        servizio.Servizio_ID = (int)listaservizi["Servizio_ID"];
                        servizio.Tipo = (string)listaservizi["Tipo"];
                        lista.Add(servizio);
                    }
                    TempData["Servizi"] = lista;
                    listaservizi.Close();
                }
            }
            catch { }
            finally
            {
                DB.conn.Close();
            }





            return View(pren);
        }

        [HttpPost]
        public ActionResult AddServizi(PS ps)
        {
            try
            {


                DB.conn.Open();

                var cmd = new SqlCommand(@"INSERT INTO PS
                                        (Prenotazione_ID, Servizio_ID, Data_Serv, Quantita, PrezzoServ)
                                          VALUES
                                          (@prenotazione_id, @servizio_id, @data_serv, @quantita, @prezzoserv)", DB.conn);
                cmd.Parameters.AddWithValue("@prenotazione_id", ps.Prenotazione_ID);
                cmd.Parameters.AddWithValue("@servizio_id", ps.Servizio_ID);
                cmd.Parameters.AddWithValue("@data_serv", ps.Data_Serv);
                cmd.Parameters.AddWithValue("@quantita", ps.Quantita);
                cmd.Parameters.AddWithValue("@prezzoserv", ps.PrezzoServ);
                cmd.ExecuteNonQuery();

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }


        [HttpGet]
        [Authorize]
        public ActionResult AddPren()
        {
            try
            {
                DB.conn.Open();

                List<Servizio> listaServizi = new List<Servizio>();
                var cmdServizi = new SqlCommand("SELECT * FROM Servizi", DB.conn);
                var readerServizi = cmdServizi.ExecuteReader();
                while (readerServizi.Read())
                {
                    var servizio = new Servizio();
                    servizio.Servizio_ID = (int)readerServizi["Servizio_ID"];
                    servizio.Tipo = (string)readerServizi["Tipo"];
                    listaServizi.Add(servizio);
                }
                ViewBag.Servizi = listaServizi;
                readerServizi.Close();

                List<Pensione> listaPensioni = new List<Pensione>();
                var cmdPensioni = new SqlCommand("SELECT * FROM Pensioni", DB.conn);
                var readerPensioni = cmdPensioni.ExecuteReader();
                while (readerPensioni.Read())
                {
                    var pensione = new Pensione();
                    pensione.Pensione_ID = (int)readerPensioni["Pensione_ID"];
                    pensione.Tipo = (string)readerPensioni["Tipo"];
                    pensione.Supplemento = (decimal)readerPensioni["Supplemento"];
                    listaPensioni.Add(pensione);
                }
                ViewBag.Pensioni = listaPensioni;
                readerPensioni.Close();

                List<Camera> listaCamere = new List<Camera>();
                var cmdCamere = new SqlCommand("SELECT * FROM Camere", DB.conn);
                var readerCamere = cmdCamere.ExecuteReader();
                while (readerCamere.Read())
                {
                    var camera = new Camera();
                    camera.Camera_ID = (int)readerCamere["Camera_ID"];
                    camera.Numero = (int)readerCamere["Numero"];
                    camera.Descrizione = (string)readerCamere["Descrizione"];
                    camera.Categoria_ID = (int)readerCamere["Categoria_ID"];
                    listaCamere.Add(camera);
                }
                ViewBag.Camere = listaCamere;
                readerCamere.Close();

                List<Ospite> listaOspiti = new List<Ospite>();
                var cmdOspite = new SqlCommand("SELECT * FROM Ospiti", DB.conn);
                var readerOspiti = cmdOspite.ExecuteReader();
                while (readerOspiti.Read())
                {
                    var ospite = new Ospite();
                    ospite.Ospite_ID = (int)readerOspiti["Ospite_ID"];
                    ospite.Nome = (string)readerOspiti["Nome"];
                    ospite.Cognome = (string)readerOspiti["Cognome"];
                    ospite.Citta = (string)readerOspiti["Citta"];
                    ospite.Provincia = (string)readerOspiti["Provincia"];
                    ospite.Email = (string)readerOspiti["Email"];
                    ospite.Telefono = (string)readerOspiti["Telefono"];
                    ospite.Cod_Fisc = (string)readerOspiti["Cod_Fisc"];
                    listaOspiti.Add(ospite);
                }
                ViewBag.Ospiti = listaOspiti;
                readerOspiti.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                DB.conn.Close();
            }

            return View();
        }
        [HttpPost]
        public ActionResult AddPren(Prenotazione pren)
        {
            try
            {
                DB.conn.Open();
                var cmdPren = new SqlCommand(@"INSERT INTO Prenotazioni 
                            (Data_Pren, Data_Arrivo, Data_Partenza, Pensione_ID, Ospite_ID, Camera_ID)
                            VALUES(@data_pren, @data_arrivo, @data_partenza, @pensione_id, @ospite_id, @camera_id)
                            SELECT SCOPE_IDENTITY()", DB.conn);
                cmdPren.Parameters.AddWithValue("@data_pren", pren.Data_Pren);
                cmdPren.Parameters.AddWithValue("@data_arrivo", pren.Data_Arrivo);
                cmdPren.Parameters.AddWithValue("@data_partenza", pren.Data_Partenza);
                cmdPren.Parameters.AddWithValue("@pensione_id", pren.Pensione_ID);
                cmdPren.Parameters.AddWithValue("@ospite_id", pren.Ospite_ID);
                cmdPren.Parameters.AddWithValue("@camera_id", pren.Camera_ID);
                int lastInsertedId = Convert.ToInt32(cmdPren.ExecuteScalar());
                if (lastInsertedId != 0)
                {
                    return RedirectToAction("Details", new { id = lastInsertedId });
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View();
            }
            finally
            {
                DB.conn.Close();
            }
        }

        public JsonResult GetCliente(string codFisc)
        {
            List<Ospite> prenotazioni = new List<Ospite>();

            try
            {
                DB.conn.Open();

                var cmd = new SqlCommand("SELECT * FROM Ospiti WHERE Cod_Fisc = @cod", DB.conn);
                cmd.Parameters.AddWithValue("@cod", codFisc);
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var prenotazione = new Ospite()
                        {
                            Ospite_ID = (int)reader["Ospite_ID"],
                            Nome = (string)reader["Nome"],
                            Cognome = (string)reader["Cognome"],
                            Citta = (string)reader["Citta"],
                            Provincia = (string)reader["Provincia"],
                            Email = (string)reader["Email"],
                            Telefono = (string)reader["Telefono"],
                            Cod_Fisc = (string)reader["Cod_Fisc"],
                        };

                        prenotazioni.Add(prenotazione);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
            finally
            {
                DB.conn.Close();
            }

            return Json(prenotazioni, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPensioneCompleta()
        {
            try
            {
                DB.conn.Open();

                var cmd = new SqlCommand("SELECT COUNT(*) FROM Prenotazioni WHERE Pensione_ID = @id", DB.conn);
                cmd.Parameters.AddWithValue("@id", 1);
                var reader = cmd.ExecuteReader();

                int count = 0;
                if (reader.Read())
                {
                    count = (int)reader[0];
                }

                var result = new
                {
                    NumeroPrenotazioni = count
                };

                Console.WriteLine("Numero di prenotazioni trovate: " + count);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Si è verificato un errore: " + ex.Message });
            }
            finally
            {
                DB.conn.Close();
            }
        }

    }
}