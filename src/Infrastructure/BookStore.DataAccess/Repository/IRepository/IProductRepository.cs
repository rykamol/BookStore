using BookStore.Domain.Models;

namespace BookStore.DataAccess.Repository.IRepository
{
	public interface IProductRepository: IRepository<Product>
	{
		void Update(Product product);

	}
}