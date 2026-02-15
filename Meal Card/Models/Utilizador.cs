using SQLite;

namespace Meal_Card.Models
{

    public class Utilizador
    {
        [PrimaryKey]
        public int Id_utilizador { get; set; }

        public string? Uid_Nfc { get; set; }

        public string? Nome { get; set; }

        public string? Sobrenome { get; set; }

        public string? Email { get; set; }

        public string? Card { get; set; }

        public string? Senha { get; set; }

        public string? Tipo { get; set; }

        public string? Url_imagem { get; set; }

        public string? CaminhoImagem => AppConfig.BaseUrl + Url_imagem;
    }

    public class FotoPerfil
    {
        public string? Url_imagem { get; set; }

        public string? CaminhoImagem => AppConfig.BaseUrl + Url_imagem;
    }

    public class UpdateSenha
    {
        public string? Corrente { get; set; }
        public string? Nova { get; set; }
    }
}
