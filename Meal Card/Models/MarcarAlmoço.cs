namespace Meal_Card.Models
{
    using Meal_Card.Models.Enums;

    /// <summary>
    /// Defines the <see cref="MarcarAlmoço" />
    /// </summary>
    public class MarcarAlmoço
    {
        /// <summary>
        /// Gets or sets the Id_marca_almoco
        /// </summary>
        public int Id_marca_almoco { get; set; }

        /// <summary>
        /// Gets or sets the Id_utilizador
        /// </summary>
        public int Id_utilizador { get; set; }

        /// <summary>
        /// Gets or sets the Data_marcacao
        /// </summary>
        public DateTime Data_marcacao { get; set; }

        /// <summary>
        /// Gets or sets the Tipo_refeicao
        /// </summary>
        public TipoAlmoco Tipo_refeicao { get; set; }

        /// <summary>
        /// Gets or sets the Valor
        /// </summary>
        public decimal? Valor { get; set; }

        /// <summary>
        /// Gets or sets the Status
        /// </summary>
        public StatusMarcacao Status { get; set; }

        /// <summary>
        /// Gets or sets the Data_atualizacao
        /// </summary>
        public DateTime? Data_atualizacao { get; set; }

        /// <summary>
        /// Gets or sets the Data_expiracao
        /// </summary>
        public DateTime? Data_expiracao { get; set; }
    }
}
