namespace Meal_Card.Models.Enums
{
    public class MarcarAlmoco
    {
        public int Id_marca_almoco { get; set; }

        public int Id_utilizador { get; set; }

        public DateTime Data_marcacao { get; set; }

        public TipoAlmoco Tipo_refeicao { get; set; }

        public decimal? Valor { get; set; }

        public StatusMarcacao Status { get; set; }

        public DateTime? Data_atualizacao { get; set; }

        public DateTime? Data_expiracao { get; set; }

        // Métodos de negócio
        public bool PodeSerCancelada()
        {
            return Status == StatusMarcacao.Pendente ||
                   Status == StatusMarcacao.Confirmado;
        }

        public bool ValidarHorarioMarcacao()
        {
            var agora = DateTime.Now.TimeOfDay;
            var limite = new TimeSpan(10, 0, 0);
            return agora < limite;
        }

        public bool EstaExpirada()
        {
            return Status == StatusMarcacao.Expirado ||
                   Data_expiracao.HasValue && DateTime.Now > Data_expiracao.Value;
        }

        public decimal? CalcularValorRefeicao()
        {
            return Tipo_refeicao == TipoAlmoco.gratuito ? 0 : Valor;
        }

        public bool RequerVerificacaoSaldo()
        {
            return Tipo_refeicao == TipoAlmoco.pago;
        }
    }
}
