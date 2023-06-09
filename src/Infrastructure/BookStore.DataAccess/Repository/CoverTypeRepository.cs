using BookStore.DataAccess.Data;
using BookStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository.IRepository
{
	public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
	{
		private ApplicationDbContext _db;
		public CoverTypeRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public void Update(CoverType category)
		{
			_db.CoverTypes.Update(category);
		}
	}
}
