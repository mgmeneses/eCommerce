using CasaDoCodigo.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Models
{
    public class UpdateQtdeResponse
    {
        public UpdateQtdeResponse(ItemPedido itemPedido, CarrinhoViewModel carrinhoViewModel)
        {
            this.ITemPedido = itemPedido;
            this.CarrinhoViewModel = carrinhoViewModel;
        }

        public ItemPedido ITemPedido { get; }
        public CarrinhoViewModel CarrinhoViewModel { get; }
    }
}
