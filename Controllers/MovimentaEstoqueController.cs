using Atividade2.Enums;
using Atividade2.Interfaces;
using Atividade2.Models;
using Microsoft.AspNetCore.Mvc;

namespace Atividade2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimentaEstoqueController
    {
        private readonly IEstoqueAplications _estoqueAplication;

        public MovimentaEstoqueController(IEstoqueAplications estoqueAplication)
        {
            _estoqueAplication = estoqueAplication;
        }

        [HttpPost]
        public List<EstoqueProdutoResponse> MovimentaEstoque(TipoOperacao tipoEntrada, ListaEstoqueRequest estoqueRequest)
        {
            if (estoqueRequest == null)
            {
                return new List<EstoqueProdutoResponse>();
            }
            var response = _estoqueAplication.MovimentaEstoque(estoqueRequest, tipoEntrada);
            return response;
        }
    }
}
