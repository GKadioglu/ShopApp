using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shopapp.data.Abstract
{
    public interface IUnitOfWork: IDisposable
    {

        ICartRepository Carts {get;}
        ICategoryRepository Categories {get;}
        IOrderRepository Orders {get;}
        IProductRepository Products {get;}
        void Save();
        Task<int> SaveAsync();

    }
}