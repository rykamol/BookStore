using BookStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository.IRepository
{
	public interface IOrderHeaderRepository : IRepository<OrderHeader>
	{
		void Update(OrderHeader orderHeader);

		void UpdateOrderStatus(int id, string orderStatus, string? paymentStatus = null, string? paymentIntentId=null);
		void UpdateStripePaymentId(int id, string sessionId);
	}
}
