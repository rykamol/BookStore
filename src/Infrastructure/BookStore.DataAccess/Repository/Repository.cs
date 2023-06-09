using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly ApplicationDbContext _db;
		internal DbSet<T> dbSet;

		public Repository(ApplicationDbContext db)
		{
			_db = db;
			dbSet = _db.Set<T>();
		}

		public void Create(T entity)
		{
			dbSet.Add(entity);
		}

		//includeProperties="Product,CoverType"
		public IEnumerable<T> GetAll(string? includeProperties = null)
		{
			IQueryable<T> query = dbSet;

			if (includeProperties != null)
			{
				foreach(var property in includeProperties.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(property);

				}
			}
			return query.ToList();
		}

		public void Delete(T entity)
		{
			dbSet.Remove(entity);
		}

		//includeProperties="Product,CoverType"

		public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null)
		{
			IQueryable<T> query = dbSet;

			if (includeProperties != null)
			{
				foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(property);

				}
			}
			return query.Where(filter).FirstOrDefault();
		}

		public void RemoveRange(IEnumerable<T> entities)
		{
			dbSet.RemoveRange(entities);
		}
	}
}
