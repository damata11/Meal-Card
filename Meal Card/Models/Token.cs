namespace Meal_Card.Models
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Defines the <see cref="Token" />
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Gets or sets the AccessToken
        /// </summary>
        [JsonPropertyName("accessToken")]
        public string? AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the TokenType
        /// </summary>
        [JsonPropertyName("tokenType")]
        public string? TokenType { get; set; }

        /// <summary>
        /// Gets or sets the Expires_in
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int? Expires_in { get; set; }

        /// <summary>
        /// Gets or sets the Utilizador
        /// </summary>
        public UtilizadorResponse? Utilizador { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="UtilizadorResponse" />
    /// </summary>
    public class UtilizadorResponse
    {
        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the Id_escola
        /// </summary>
        [JsonPropertyName("id_escola")]
        public int Id_escola { get; set; }

        /// <summary>
        /// Gets or sets the Nome
        /// </summary>
        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        /// <summary>
        /// Gets or sets the Sobrenome
        /// </summary>
        [JsonPropertyName("sobrenome")]
        public string? Sobrenome { get; set; }

        /// <summary>
        /// Gets or sets the Email
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the Card
        /// </summary>
        [JsonPropertyName("card")]
        public string? Card { get; set; }

        /// <summary>
        /// Gets or sets the Tipo
        /// </summary>
        [JsonPropertyName("tipo")]
        public string? Tipo { get; set; }
    }
}
