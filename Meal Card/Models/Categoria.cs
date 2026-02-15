namespace Meal_Card.Models
{

    public class Categoria
    {
        public int Id_categoria { get; set; }

        public string? Nome { get; set; }

        public string? Descricao { get; set; }

        public string? Tipo { get; set; }

        public bool Ativo { get; set; }

        public string? Url_imagem { get; set; }

        public string? CaminhoImagem => AppConfig.BaseUrl + Url_imagem;
    }
}
