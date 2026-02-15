namespace Meal_Card.Models.Enums
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Defines the StatusTransacao
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StatusTransacao
    {
        /// <summary>
        /// Defines the pendente
        /// </summary>
        pendente = 1,

        /// <summary>
        /// Defines the aprovada
        /// </summary>
        aprovada = 2,

        /// <summary>
        /// Defines the recusada
        /// </summary>
        recusada = 3,

        /// <summary>
        /// Defines the estornada
        /// </summary>
        estornada = 4
    }
}
