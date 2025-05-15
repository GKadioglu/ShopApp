using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.business.Abstract;
using shopapp.data.Abstract;
using shopapp.entity;

namespace shopapp.business.Concrete
{
    public class CartManager : ICartService
    {
        private readonly IUnitOfWork _unitofwork;

        public CartManager(IUnitOfWork unitofwork)
        {
            _unitofwork =  unitofwork;
        }

        public void AddtoCart(string userId, int productId, int quantity)
        {
            var cart = GetCartByUserId(userId);
            if(cart !=null)
            {
                // eklenmek isteyen ürün sepette var mı? (güncelleme)
                var index = cart.CardItems.FindIndex(i=>i.ProductId == productId);

                if(index<0)
                {
                    cart.CardItems.Add(new CardItem(){
                        ProductId = productId,
                        Quantity = quantity,
                        CartId = cart.Id

                    });
                }
                else {
                    cart.CardItems[index].Quantity += quantity;
                }
                _unitofwork.Carts.Update(cart);
                _unitofwork.Save();
            }
        }

        public void ClearCart(int CartId)
        {
            _unitofwork.Carts.ClearCart(CartId);
        }

        public void DeleteFromCart(string userId, int ProductId)
        {
            var cart = GetCartByUserId(userId);
            if(cart!=null) {
                _unitofwork.Carts.DeleteFromCart(cart.Id,ProductId);
            }
        }

        public Cart GetCartByUserId(string userId)
        {
            return _unitofwork.Carts.GetByUserId(userId);
        }

        public void InitializeCart(string userId)
        {
            _unitofwork.Carts.Create(new Cart(){UserId = userId});
            _unitofwork.Save();
        }
    }
}