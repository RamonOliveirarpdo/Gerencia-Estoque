using Atividade2.Configurations;
using Atividade2.Enums;
using Atividade2.Interfaces;
using Atividade2.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Atividade2.Aplications
{
    public class EstoqueAplications : IEstoqueAplications
    {
        private readonly IMemoryCache _cache;
        private readonly AppConfig _config;

        public EstoqueAplications(IMemoryCache cache, IOptions<AppConfig> appConfig)
        {
            _cache = cache;
            _config = appConfig.Value;
        }

        public List<EstoqueProdutoResponse> MovimentaEstoque(ListaEstoqueRequest estoqueRequest, TipoOperacao tipoEntrada)
        {
            var listaEstoque = new List<EstoqueProdutoResponse>();
            string keyCache = _config.ChaveEstoqueTotal;

            listaEstoque = ValidaListaEstoque(keyCache, listaEstoque);

            foreach (var item in estoqueRequest.Estoque)
            {
                var produto = listaEstoque.FirstOrDefault(x => x.CodigoProduto == item.CodigoProduto);
                if (produto != null)
                {
                    produto = CalculaEstoque(tipoEntrada, produto, item);
                }
                else
                {
                    produto = AdicionaProdutoEstoque(item);
                    listaEstoque.Add(produto);

                    continue;
                }
            }

            AdicionaCache(keyCache, listaEstoque);

            return listaEstoque;
        }

        private List<EstoqueProdutoResponse> ValidaListaEstoque(string keyCache, List<EstoqueProdutoResponse> listaEstoque)
        {
            if (!_cache.TryGetValue(keyCache, out listaEstoque))
            {
                listaEstoque = new List<EstoqueProdutoResponse>();
            }
            else
            {
                var cacheStoque = _cache.Get(keyCache);

                if (cacheStoque != null)
                {
                    listaEstoque = (List<EstoqueProdutoResponse>)cacheStoque;
                }
            }

            return listaEstoque;
        }

        private EstoqueProdutoResponse CalculaEstoque(TipoOperacao tipoEntrada, EstoqueProdutoResponse produtoResponse, EstoqueProduto produto)
        {
            if (tipoEntrada == TipoOperacao.Entrada && produto.DescricaoProduto == produtoResponse.Nome && produto.CodigoProduto == produtoResponse.CodigoProduto)
            {
                produtoResponse.QuantidadeProduto += produto.Estoque;
            }
            else if (tipoEntrada == TipoOperacao.Saida && produto.DescricaoProduto == produtoResponse.Nome && produto.CodigoProduto == produtoResponse.CodigoProduto)
            {
                produtoResponse.QuantidadeProduto -= produto.Estoque;
            }

            return produtoResponse;
        }

        private EstoqueProdutoResponse AdicionaProdutoEstoque(EstoqueProduto produto)
        {
            var produtoResponse = new EstoqueProdutoResponse()
            {
                CodigoProduto = produto.CodigoProduto,
                Nome = produto.DescricaoProduto,
                QuantidadeProduto = produto.Estoque
            };

            return produtoResponse;
        }
        private void AdicionaCache(string key, List<EstoqueProdutoResponse> estoqueProduto)
        {
            var opcoes = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(_config.ExpiracaoCache));

            _cache.Set(key, estoqueProduto);
        }
    }
}
