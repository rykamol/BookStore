using BookStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository.IRepository
{
	public interface IOrderDetailsRepository : IRepository<OrderDetail>
	{
		void Update(OrderDetail orderDetail);
	}
}
