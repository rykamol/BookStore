using BookStore.DataAccess.Data;
using BookStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository.IRepository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
	{
		private ApplicationDbContext _db;
		public ShoppingCartRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public void DecrementCount(ShoppingCart shoppingCart, int count)
		{
			shoppingCart.Count -= count;
		}

		public void IncrementCount(ShoppingCart shoppingCart, int count)
		{
			shoppingCart.Count += count;
		}
	}
}
