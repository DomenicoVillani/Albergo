using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Albergo.Models
{
    public class Ospite
    {
        [Key]
        public int Ospite_ID { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Il Nome deve essere di minimo 2 caratteri e di massimo 50")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Il Nome deve essere di minimo 2 caratteri e di massimo 50")]
        public string Cognome { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "la Citta deve essere di minimo 2 caratteri e di massimo 50")]
        public string Citta { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "la Provincia deve essere di minimo 2 caratteri e di massimo 50")]
        public string Provincia { get; set; }

        [Required(ErrorMessage = "Email Campo Obbligatorio")]
        [Display(Name = "E-mail")]
        [StringLength(255, ErrorMessage = "L'email deve avere massimo 255 caratteri")]
        [RegularExpression(@"^[\w-.]+@([\w-]+.)+[\w-]{2,4}$", ErrorMessage = "Inserisci un indirizzo email valido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [Display(Name = "Telefono/Cellulare")]
        [StringLength(20, MinimumLength = 7, ErrorMessage = "Minimo 7 cifre, massimo 20")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Il campo Tel accetta solo numeri.")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [Display(Name = "Codice Fiscale")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "16 cifre")]
        public string Cod_Fisc { get; set; }
    }
}