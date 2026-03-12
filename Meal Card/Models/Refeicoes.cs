using Meal_Card.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Meal_Card.Models
{


    public class Refeicoes
    {
        public int? Id_refeicao { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public string? Tipo_Refeicao { get; set; }
        public string? Categoria { get; set; }
        public string? Ingredientes { get; set; }
        public string? Alergenos { get; set; }
        public string? Url_imagem { get; set; }
        public bool? Disponivel { get; set; }
        public string? CaminhoImagem => AppConfig.BaseUrl + Url_imagem;
    }

    public class RefeitorioDia
    {
        public string? Data { get; set; }
        public string? Dia_Semana { get; set; }
        public int Dia { get; set; }
        public bool Fim_Semana { get; set; }
        public bool Hoje { get; set; }
        public Dictionary<string, List<Refeicoes>>? RefeicoesPorTipo { get; set; } = new();
    }

    public class Calendario
    {
        public int Ano { get; set; }
        public int Mes { get; set; }
        public string? Nome_Mes { get; set; }
        public string? Mes_Ano { get; set; }
        public List<RefeitorioDia>? Dias { get; set; } = new();
    }

    public class MarcarAlmoco { 
        public int Id_reserva { get; set; } 
        public int Id_utilizador { get; set; } 
        public DateTime Data_reserva { get; set; } 
        public string? Tipo_refeicao { get; set; } 
        public decimal? Valor { get; set; } public StatusMarcacao Status { get; set; } 
        public DateTime? Data_atualizacao { get; set; } 
        public DateTime? Data_expiracao { get; set; }

    } 
    
    public class CriarReserva
        {
            public DateTime Data_marcacao { get; set; }
            public string? Tipo_refeicao { get; set; }
        }
}
