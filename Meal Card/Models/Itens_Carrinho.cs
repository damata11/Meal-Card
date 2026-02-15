namespace Meal_Card.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;


    public class Itens_Carrinho : INotifyPropertyChanged
    {

        public int Id_pedido_itens { get; set; }
        public int Id_utilizador { get; set; }
        public int Id_produto { get; set; }

        public string? Nome { get; set; }

        public string? Descricao { get; set; }
        private int _quantidade { get; set; }
        public int Quantidade
        {
            get => _quantidade;
            set
            {
                if (_quantidade != value)
                {
                    _quantidade = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal Preco { get; set; }

        public decimal Subtotal { get; set; }

        public DateTime Data_criacao { get; set; }

        public string? Url_imagem { get; set; }

        public string? CaminhoImagem => AppConfig.BaseUrl + Url_imagem;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Incluir_Carrinho
    {
        [Required]
        public int Id_produto { get; set; }

        [Required]
        public int Quantidade { get; set; } = 1;
    }
}
