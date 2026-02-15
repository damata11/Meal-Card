namespace Meal_Card.Models.Enums
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Defines the TipoTransacao
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TipoTransacao
    {
        /// <summary>
        /// Defines the debito
        /// </summary>
        debito = 1,

        /// <summary>
        /// Defines the credito
        /// </summary>
        credito = 2,

        /// <summary>
        /// Defines the recarga
        /// </summary>
        recarga = 3,

        /// <summary>
        /// Defines the estorno
        /// </summary>
        estorno = 4
    }
}
