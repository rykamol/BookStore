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
	public class ProductRepository : Repository<Product>, IProductRepository
	{
		private ApplicationDbContext _db;
		public ProductRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public void Update(Product product)
		{
			var productToUpdate = _db.Products.FirstOrDefault(x => x.Id == product.Id);
			if (productToUpdate != null)
			{
				productToUpdate.Title = product.Title;
				productToUpdate.Description = product.Description;
				productToUpdate.ISBN = product.ISBN;
				productToUpdate.ListPrice = product.ListPrice;
				productToUpdate.Price = product.Price;
				productToUpdate.Price50 = product.Price50;
				productToUpdate.Price100 = product.Price100;
				productToUpdate.Author = product.Author;
				productToUpdate.CoverTypeId = product.CoverTypeId;
				productToUpdate.CategoryId = product.CategoryId;

				if (product.ImageUrl != null)
				{
					productToUpdate.ImageUrl = product.ImageUrl;
				}
			}
		}
	}
}
