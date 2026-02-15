namespace Meal_Card.Models
{
    /// <summary>
    /// Defines the <see cref="Produtos_Bar" />
    /// </summary>
    public class Produtos_Bar
    {
        /// <summary>
        /// Gets or sets the Id_produto
        /// </summary>
        public int Id_produto { get; set; }

        /// <summary>
        /// Gets or sets the Id_estabelecimento
        /// </summary>
        public int Id_estabelecimento { get; set; }

        /// <summary>
        /// Gets or sets the Id_categoria
        /// </summary>
        public int Id_categoria { get; set; }

        /// <summary>
        /// Gets or sets the Nome
        /// </summary>
        public string? Nome { get; set; }

        /// <summary>
        /// Gets or sets the Descricao
        /// </summary>
        public string? Descricao { get; set; }

        /// <summary>
        /// Gets or sets the Preco
        /// </summary>
        public decimal? Preco { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Disponivel
        /// </summary>
        public bool Disponivel { get; set; }

        /// <summary>
        /// Gets or sets the Url_imagem
        /// </summary>
        public string? Url_imagem { get; set; }

        /// <summary>
        /// Gets the CaminhoImagem
        /// </summary>
        public string? CaminhoImagem => AppConfig.BaseUrl + Url_imagem;

        /// <summary>
        /// Gets or sets the Categorias
        /// </summary>
        public Categoria? Categorias { get; set; }
    }
}
