using BookStore.DataAccess.Data;
using BookStore.Domain.Models;
using BookStore.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository.IRepository
{
	public class ApplicationuserRepository : Repository<ApplicationUser>, IApplicatiionUserRepository
	{
		private ApplicationDbContext _db;
		public ApplicationuserRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}
	}
}
