using BookStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Domain.ViewModels
{
    public class ShoppingCartViewModel
	{
		public IEnumerable<ShoppingCart> ShoppingCarts { get; set; }

		public OrderHeader OrderHeader { get; set; }

	}
}
