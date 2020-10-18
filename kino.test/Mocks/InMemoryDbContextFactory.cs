using System;
using kino.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

    public static class InMemoryDbContextFactory
    {
        public static KinoContext GetDbContext()
        {
            var internalServiceProviderForDbContext = new ServiceCollection()
               .AddEntityFrameworkInMemoryDatabase()
               .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<KinoContext>()
                .UseInternalServiceProvider(internalServiceProviderForDbContext)
                .UseInMemoryDatabase("KinoContext_UnitTesting" + new Guid().ToString())
                .Options;
            var dbContext = new KinoContext(options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            return dbContext;
        }
    }
