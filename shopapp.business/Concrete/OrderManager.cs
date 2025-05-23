using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.business.Abstract;
using shopapp.data.Abstract;
using shopapp.entity;

namespace shopapp.business.Concrete
{
    public class OrderManager : IOrderService
    {
        private readonly IUnitOfWork _unitofwork;

        public OrderManager(IUnitOfWork unitofwork)
        {
            _unitofwork =  unitofwork;
        }
        public void Create(Order entity)
        {
            _unitofwork.Orders.Create(entity);
        }

        public List<Order> GetOrders(string userId)
        {
            return _unitofwork.Orders.GetOrders(userId);
        }
    }
}