using SQLite;

namespace Meal_Card.Models
{

    public class Favorito
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Id_utilizador { get; set; } 

        public int Id_produto { get; set; }

        public string? Nome { get; set; }

        public string? Descricao { get; set; }

        public decimal Preco { get; set; }

        public string? CaminhoImagem { get; set; }

        public bool Isfavorito { get; set; }

        [Ignore]
        public string IconeFavorito => Isfavorito ? "heart_filled.png" : "heart_empty.png";
    }
}
