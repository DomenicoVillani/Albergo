using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Albergo.Models
{
    public class Camera
    {
        [Key]
        public int Camera_ID { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        public int Numero {  get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [StringLength(255, ErrorMessage ="La descrizione deve essere di massimo 255 caratteri")]
        public string Descrizione { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        public int Categoria_ID { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        public Categoria Categoria { get; set; }
    }
}