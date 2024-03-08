using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Albergo.Models
{
    public class Servizio
    {
        [Key]
        public int Servizio_ID { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [Display(Name = "Tipo Servizio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Il tipo servizio deve essere di minimo 2 caratteri e di massimo 50")]
        public string Tipo { get; set; }

    }
}