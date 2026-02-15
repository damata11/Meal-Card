namespace Meal_Card.Models
{
    public class HistoricoList
    {

        public required string Nome { get; set; }

        public required string Descricao { get; set; }

        public required string Data { get; set; }

        public required string valor { get; set; }

        public required string status { get; set; }

        public string? Icon { get; set; }

        public string? CaminhoImagem => AppConfig.BaseUrl + Icon;
    }
}
