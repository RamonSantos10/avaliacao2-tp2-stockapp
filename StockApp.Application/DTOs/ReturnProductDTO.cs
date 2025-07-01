using System.ComponentModel.DataAnnotations;

public class ReturnProductDTO
{
    [Required(ErrorMessage = "O ID do pedido é obrigatório")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "O ID do produto é obrigatório")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "A quantidade é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "O motivo da devolução é obrigatório")]
    [MinLength(10, ErrorMessage = "O motivo deve ter pelo menos 10 caracteres")]
    public string ReturnReason { get; set; }
}