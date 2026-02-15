namespace Meal_Card.Models
{

    public class MenuCantina
    {
        private DateTime date;

        public MenuCantina( DateTime date )
        {
            this.date = date;
        }

        public DateTime Data { get; set; }
        public bool IsCurrentMonth { get; internal set; }
        public int DayNumber { get; internal set; }
        public bool HasMeal { get; internal set; }
        public bool IsToday { get; internal set; }
    }

    public class Refeicoes
    {
        public int Id_refeicao { get; set; }

        public int Id_estabelecimento { get; set; }

        public int? Id_categoria { get; set; }

        public string? Nome { get; set; }

        public string? Descricao { get; set; }

        public string? Tipo_refeicao { get; set; }

        public string? Ingredientes { get; set; }

        public string? Informacoes_nutricionais { get; set; }

        public bool Disponivel { get; set; } = true;

        public string? Url_imagem { get; set; }

        public string? CaminhoImagem => AppConfig.BaseUrl + Url_imagem;
    }
}
