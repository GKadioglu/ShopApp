using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shopapp.business.Abstract
{
    public interface IValidator<T>
    {
        string ErrorMesage {get; set;}
        bool Validation(T entity);
    }
}