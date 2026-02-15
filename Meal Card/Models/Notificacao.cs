using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meal_Card.Models
{
    public class Notificacao
    {
        public int Id_notificacao { get; set; }

        public int Id_utilizador { get; set; }

        public string? Titulo { get; set; }

        public string? Mensagem { get; set; }

        public string? Tipo { get; set; }

        public bool Lida { get; set; } = false;

        public DateTime Data_envio { get; set; } = DateTime.UtcNow;

        public DateTime? Data_leitura { get; set; }

    }
}
