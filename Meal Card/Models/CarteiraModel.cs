namespace Meal_Card.Models
{
    /// <summary>
    /// Defines the <see cref="CarteiraModel" />
    /// </summary>
    public class CarteiraModel
    {
        /// <summary>
        /// Gets or sets the Id_carteira
        /// </summary>
        public int Id_carteira { get; set; }

        /// <summary>
        /// Gets or sets the Id_utilizador
        /// </summary>
        public int Id_utilizador { get; set; }

        /// <summary>
        /// Gets or sets the Saldo_disponivel
        /// </summary>
        public decimal Saldo_disponivel { get; set; }

        /// <summary>
        /// Gets or sets the Limite_diario
        /// </summary>
        public decimal Limite_diario { get; set; }
    }
}
