using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Albergo.Models
{
    public class Pensione

    {
        [Key]
        public int Pensione_ID { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [Display(Name = "Tipo pensione")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Il tipo pensione deve essere di minimo 2 caratteri e di massimo 50")]
        public string Tipo { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [Range(1, 1000000, ErrorMessage = "errore nel supplemento")]
        public decimal Supplemento { get; set; }
    }
}