using Albergo.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Albergo.Controllers
{
    public class OspitiController : Controller
    {
        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Ospite ospite)
        {
            try
            {
                DB.conn.Open();
                var cmdOspite = new SqlCommand(@"INSERT INTO Ospiti
                                           (Nome, Cognome, Citta, Provincia, Email, Telefono, Cod_Fisc)
                                           VALUES (@nome, @cognome, @citta, @prov, @email, @tel, @cod_fisc)
                                           SELECT SCOPE_IDENTITY()", DB.conn);
                cmdOspite.Parameters.AddWithValue("@nome", ospite.Nome);
                cmdOspite.Parameters.AddWithValue("@cognome", ospite.Cognome);
                cmdOspite.Parameters.AddWithValue("@citta", ospite.Cognome);
                cmdOspite.Parameters.AddWithValue("@prov", ospite.Provincia);
                cmdOspite.Parameters.AddWithValue("@email", ospite.Email);
                cmdOspite.Parameters.AddWithValue("@tel", ospite.Telefono);
                cmdOspite.Parameters.AddWithValue("@cod_fisc", ospite.Cod_Fisc);
                int id= Convert.ToInt32(cmdOspite.ExecuteScalar());
                if(id > 0)
                {
                    TempData["Mes"] = "Ospite inserito con successo";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["MesError"] = "Errore nell'inserimento dell'ospite nel database";
                    return RedirectToAction("Index");
                }
                
            }
            catch (Exception ex)
            {

            }
            finally 
            { 

            }
            return View();
        }
    }
}