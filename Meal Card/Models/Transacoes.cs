namespace Meal_Card.Models
{
    using Meal_Card.Models.Enums;

    /// <summary>
    /// Defines the <see cref="Transacoes" />
    /// </summary>
    public class Transacoes
    {
        /// <summary>
        /// Gets or sets the Id_transacao
        /// </summary>
        public int Id_transacao { get; set; }

        /// <summary>
        /// Gets or sets the Id_utilizador
        /// </summary>
        public int Id_utilizador { get; set; }

        /// <summary>
        /// Gets or sets the Id_estabelecimento
        /// </summary>
        public int? Id_estabelecimento { get; set; }

        /// <summary>
        /// Gets or sets the Id_pedido
        /// </summary>
        public int? Id_pedido { get; set; }

        /// <summary>
        /// Gets or sets the Tipo
        /// </summary>
        public TipoTransacao Tipo { get; set; }

        /// <summary>
        /// Gets or sets the Valor
        /// </summary>
        public decimal Valor { get; set; }

        /// <summary>
        /// Gets or sets the Saldo_anterior
        /// </summary>
        public decimal Saldo_anterior { get; set; }

        /// <summary>
        /// Gets or sets the Saldo_posterior
        /// </summary>
        public decimal Saldo_posterior { get; set; }

        /// <summary>
        /// Gets or sets the Descricao
        /// </summary>
        public string? Descricao { get; set; }

        /// <summary>
        /// Gets or sets the Codigo_seguranca
        /// </summary>
        public string? Codigo_seguranca { get; set; }

        /// <summary>
        /// Gets or sets the Status
        /// </summary>
        public StatusTransacao Status { get; set; }

        /// <summary>
        /// Gets or sets the Data_transacao
        /// </summary>
        public DateTime Data_transacao { get; set; } = DateTime.UtcNow;
    }
}
