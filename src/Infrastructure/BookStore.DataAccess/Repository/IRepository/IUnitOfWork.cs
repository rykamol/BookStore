using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository.IRepository
{
	public interface IUnitOfWork
	{
		ICategoryRepository Category { get; }
		ICoverTypeRepository CoverType { get; }
		IProductRepository Products { get; }
		ICompanyRepository Companies { get; }
		IShoppingCartRepository ShoppingCarts { get; }
		IApplicatiionUserRepository ApplicatiionUsers { get; }

		IOrderHeaderRepository OrderHeaders { get; }

		void Save();
	}
}
