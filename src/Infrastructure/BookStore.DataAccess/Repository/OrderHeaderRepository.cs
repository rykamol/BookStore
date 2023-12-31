﻿using BookStore.DataAccess.Data;
using BookStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository.IRepository
{
	public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
	{
		private ApplicationDbContext _db;
		public OrderHeaderRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public void Update(OrderHeader orderHeader)
		{
			_db.OrderHeaders.Update(orderHeader);
		}

		public void UpdateOrderStatus(int id, string orderStatus, string? paymentStatus = null,string? paymentIntentId=null)
		{
			var orderFromDb = _db.OrderHeaders.FirstOrDefault(x => x.Id == id);

			if (orderFromDb != null)
			{
				orderFromDb.OrderStatus = orderStatus;

				if (paymentStatus != null)
				{
					orderFromDb.PaymentStatus = paymentStatus;
				}
			}

			orderFromDb.PaymentIntentId = paymentIntentId;
		}

		public void UpdateStripePaymentId(int id, string sessionId )
		{
			var orderFromDb = _db.OrderHeaders.FirstOrDefault(x => x.Id == id);
			orderFromDb.PaymentDate=DateTime.Now;
			orderFromDb.SeassionId = sessionId;
		}
	}
}
