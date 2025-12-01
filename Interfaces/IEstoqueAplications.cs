using Atividade2.Aplications;
using Atividade2.Enums;
using Atividade2.Models;

namespace Atividade2.Interfaces
{
    public interface IEstoqueAplications
    {
        List<EstoqueProdutoResponse> MovimentaEstoque(ListaEstoqueRequest estoqueRequest, TipoOperacao tipoEntrada);
    }
}
