using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using shopapp.data.Abstract;
using shopapp.entity;

namespace shopapp.data.Concrete.EfCore
{
    public class EfCoreCartRepository : EfCoreGenericRepository<Cart>, ICartRepository
    {
        public EfCoreCartRepository(ShopContext context): base(context)
        {
            
        }
        private ShopContext ShopContext
        {
            get {return context as ShopContext;} 
        }
        public void ClearCart(int cartId)
        {
                var cmd = @"delete from CardItems where CartId=@p0";
                ShopContext.Database.ExecuteSqlRaw(cmd,cartId);
            
        }

        public void DeleteFromCart(int cartId, int productId)
        {
                var cmd = @"delete from CardItems where CartId=@p0 and ProductId=@p1";
                ShopContext.Database.ExecuteSqlRaw(cmd,cartId,productId);
            
        }

        public Cart GetByUserId(string userId)
        {
            return ShopContext.Carts
                            .Include(i=>i.CardItems)
                            .ThenInclude(i=>i.Product)
                            .FirstOrDefault(i=>i.UserId==userId);
        }

        public override void Update(Cart entity)
        {
                ShopContext.Carts.Update(entity);
        }

    }
}