using Microsoft.Extensions.DependencyInjection;
using Stoocker.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Stoocker.Tests.Common
{
    public abstract class BaseTest : IDisposable
    {
        protected IServiceProvider ServiceProvider { get; private set; }
        protected IServiceScope Scope { get; private set; }
        protected ApplicationDbContext Context { get; private set; }

        protected BaseTest()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
            Scope = ServiceProvider.CreateScope();
            Context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            SeedDatabase();
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
            // In-Memory Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

            // AutoMapper
            services.AddAutoMapper(typeof(Application.ServiceRegistration).Assembly);

            // Add other services as needed
        }

        protected virtual void SeedDatabase()
        {
            // Override in derived classes to seed test data
        }

        protected T GetService<T>() where T : notnull
        {
            return Scope.ServiceProvider.GetRequiredService<T>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context?.Dispose();
                Scope?.Dispose();
                if (ServiceProvider is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
