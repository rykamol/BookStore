using BookStore.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository.IRepository
{
	public class UnitOfWork : IUnitOfWork
	{
		private ApplicationDbContext _db;
		public UnitOfWork(ApplicationDbContext db)
		{
			_db = db;
			Category = new CategoryRepository(_db);
			CoverType = new CoverTypeRepository(_db);
			Products = new ProductRepository(_db);
			Companies = new CompanyRepository(_db);
		}

		public ICategoryRepository Category { get; private set; }
		public ICoverTypeRepository CoverType { get; private set; }
		public IProductRepository Products { get; private set; }
		public ICompanyRepository Companies { get; private set; }


		public void Save()
		{
			_db.SaveChanges();
		}
	}
}
