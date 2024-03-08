using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Albergo.Models
{
    public class PS
    {
        public int Prenotazione_ID { get; set; }
        public int Servizio_ID { get; set; }

        public Prenotazione Prenotazione { get; set; }
        public Servizio Servizio { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [Display(Name = "Data")]
        public DateTime Data_Serv {  get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        public int Quantita { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [Display(Name = "Prezzo servizio")]
        [Range(1, 1000000, ErrorMessage = "errore nel prezzo Servizio")]
        public decimal PrezzoServ { get; set; }
    }
}