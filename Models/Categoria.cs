using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Albergo.Models
{
    public class Categoria
    {
        [Key]
        public int Categoria_ID { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [Display(Name = "Tipo Camera")]
        [StringLength(50, MinimumLength =2, ErrorMessage = "Il tipo deve essere di minimo 2 caratteri e di massimo 50")]
        public string Tipo { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [Range(1,1000000, ErrorMessage = "Max 1000000 min 10 cifre")]
        public decimal TariffaNotte { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [Range(1, 1000000, ErrorMessage = "errore nella caparra")]
        public decimal Caparra { get; set; }
    }
}