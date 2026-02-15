namespace Meal_Card.Models.Enums;

public class Transacao
{

    public int Id_transacao { get; set; }
    public int Id_utilizador { get; set; }
    public int? Id_estabelecimento { get; set; }
    public int? Id_pedido { get; set; }
    public TipoTransacao Tipo { get; set; }
    public decimal Valor { get; set; }
    public decimal Saldo_anterior { get; set; }
    public decimal Saldo_posterior { get; set; }
    public string? Descricao { get; set; }
    public string? Codigo_seguranca { get; set; }
    public StatusTransacao Status { get; set; }
    public DateTime Data_transacao { get; set; }

}
