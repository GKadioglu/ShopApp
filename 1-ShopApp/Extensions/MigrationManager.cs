using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _1_ShopApp.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using shopapp.data.Concrete.EfCore;
using _1_ShopApp.Extensions;

namespace _1_ShopApp.Extensions
{
    public static class MigrationManager
    {
        public static IHost MigrateDatabase(this IHost host)
        {
            using(var scope = host.Services.CreateScope())
            {
                using(var applicationContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>())
                {
                    try
                    {
                        applicationContext.Database.Migrate();
                    }
                    catch (System.Exception)
                    {
                        
                        //loglama işlemleri
                        throw;
                    }
                } 
                using(var shopContext = scope.ServiceProvider.GetRequiredService<ShopContext>())
                {
                    try
                    {
                        shopContext.Database.Migrate();
                    }
                    catch (System.Exception)
                    {
                        
                        //loglama işlemleri
                        throw;
                    }
                } 
                
            }
        
            return host;
        }
    }
}