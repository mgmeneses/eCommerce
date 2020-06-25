using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CasaDoCodigo.Repositories
{
    public interface IPedidoRepository
    {
        Pedido GetPedido();
        void AddItem(string codigo);
        UpdateQtdeResponse UpdateQtde(ItemPedido itemPedido);
        Pedido UpdateCadastro(Cadastro cadastro);
    }
    public class PedidoRepository : BaseRepository<Pedido>, IPedidoRepository
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IItemPedidoRepository itemPedidoRepository;
        private readonly ICadastroRepository cadastroRepository;

        public PedidoRepository(ApplicationContext contexto,
            IHttpContextAccessor contextAccessor,
            IItemPedidoRepository itemPedidoRepository,
            ICadastroRepository cadastroRepository) : base(contexto)
        {
            this.contextAccessor = contextAccessor;
            this.itemPedidoRepository = itemPedidoRepository;
            this.cadastroRepository = cadastroRepository;
        }

        public void AddItem(string codigo)
        {
            var produto = contexto.Set<Produto>()
                .Where(p => p.Codigo == codigo)
                .SingleOrDefault();
            if (produto == null)
            {
                throw new ArgumentException("Produto não encontrado.");
            }
            var pedido = GetPedido();

            var itemPedido = contexto.Set<ItemPedido>()
                .Where(i => i.Produto.Codigo == codigo
                        && i.Pedido.Id == pedido.Id)
                .SingleOrDefault();
            if (itemPedido == null)
            {
                itemPedido = new ItemPedido(pedido, produto, 1, produto.Preco);
                contexto.Set<ItemPedido>()
                    .Add(itemPedido);
                contexto.SaveChanges();
            }

        }

        public Pedido GetPedido()
        {
            var pedidoId = GetPedidoId();
            var pedido = dbSet
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .Include(c => c.Cadastro)
                .Where(p => p.Id == pedidoId)
                .SingleOrDefault();
            if (pedido == null)
            {
                pedido = new Pedido();
                dbSet.Add(pedido);
                contexto.SaveChanges();
                SetPedidoId(pedido.Id);
            }

            return pedido;
        }

        private int? GetPedidoId()
        {
            return contextAccessor.HttpContext.Session.GetInt32("pedidoId");
        }

        private void SetPedidoId(int pedidoId)
        {
            contextAccessor.HttpContext.Session.SetInt32("pedidoId", pedidoId);
        }
        public UpdateQtdeResponse UpdateQtde(ItemPedido itemPedido)
        {
            var itemPedidoDB = itemPedidoRepository.GetItemPedido(itemPedido.Id);

            if (itemPedidoDB != null)
            {
                itemPedidoDB.AtualizaQtde(itemPedido.Quantidade);

                if (itemPedido.Quantidade == 0)
                {
                    itemPedidoRepository.RemoveItemPedido(itemPedido.Id);
                }
                contexto.SaveChanges();

                var carrinhoViewModel = new CarrinhoViewModel(GetPedido().Itens);
                return new UpdateQtdeResponse(itemPedidoDB, carrinhoViewModel);
            }
            throw new ArgumentException("Item do Pedido não encontrado.");
        }

        public Pedido UpdateCadastro(Cadastro cadastro)
        {
            var pedido = GetPedido();
            cadastroRepository.Update(pedido.Cadastro.Id, cadastro);
            return pedido;
        }
    }
}
