using BookStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
	{
		void IncrementCount(ShoppingCart existingCart, int count);
		void DecrementCount(ShoppingCart existingCart, int count);
	}
}
