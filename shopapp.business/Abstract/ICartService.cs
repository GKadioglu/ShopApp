using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.data.Abstract;
using shopapp.entity;

namespace shopapp.business.Abstract
{
    public interface ICartService
    {
        void InitializeCart(string userId);
        Cart GetCartByUserId(string userId);

        void AddtoCart(string userId, int productId, int quantity);

        void DeleteFromCart(string userId,int ProductId);

        void ClearCart(int CartId);
    }
}