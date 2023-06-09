using BookStore.Domain.Models;

namespace BookStore.DataAccess.Repository.IRepository
{
	public interface ICompanyRepository: IRepository<Company>
	{
		void Update(Company company);
	}
}