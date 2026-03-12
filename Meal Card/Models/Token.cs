namespace Meal_Card.Models
{
    using System.Text.Json.Serialization;

    public class Token
    {

        public string? AccessToken { get; set; }

        public string? TokenType { get; set; }

        public int Expires_in { get; set; }

        public UtilizadorResponse? Utilizador { get; set; }
    }

    public class UtilizadorResponse
    {

        public int Id { get; set; }

        public int Id_escola { get; set; }

        public string? Nome { get; set; }

        public string? Sobrenome { get; set; }

        public string? Email { get; set; }

        public string? Card { get; set; }

        public string? Tipo { get; set; }
    }
}
