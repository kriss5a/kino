using kino.Controllers;
using kino.Database;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Xunit;

namespace kino.test.Controllers
{
    public class ScreeningControllerTests
    {
        [Fact]
        public void InstantiatesController()
        {
            var context = InMemoryDbContextFactory.GetDbContext();
            var controller = new ScreeningController(context);

            Assert.NotNull(controller);
        }

        [Fact]
        public void GetsScreenings()
        {
            var context = InMemoryDbContextFactory.GetDbContext();
            context.Screenings.AddRange(new Screening { ScreeningId = 1 }, new Screening { ScreeningId = 2 });
            context.SaveChanges();
            var controller = new ScreeningController(context);

            var response = controller.GetScreenings();

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public async void AddsScreening()
        {
            var context = InMemoryDbContextFactory.GetDbContext();
            context.Movies.Add(new Movie { MovieId = 1 });
            context.ScreeningRooms.Add(new ScreeningRoom { ScreeningRoomId = 1 });
            await context.SaveChangesAsync();

            var controller = new ScreeningController(context);

            var time = DateTime.Now;

            var Screening = new Screening { DateTime = time, ScreeningRoomId=1, MovieId= 1 };

            var response = await controller.AddScreening(Screening);

            Assert.IsType<OkObjectResult>(response.Result);
            var okRes = (OkObjectResult)response.Result;
            Assert.Equal(time, ((Screening)okRes.Value).DateTime);
        }

        //[Fact]
        //public async void PatchesScreening()
        //{
        //    var context = InMemoryDbContextFactory.GetDbContext();
        //    context.Add(new Screening { ScreeningId = 1, Title = "Testing Screening1" });
        //    await context.SaveChangesAsync();
        //    var controller = new ScreeningController(context);

        //    var Screening = new Screening { ScreeningId = 1, Title = "Testing Screening2" };

        //    var response = await controller.PatchScreening(Screening.ScreeningId, Screening);

        //    Assert.IsType<OkObjectResult>(response.Result);
        //    var okRes = (OkObjectResult)response.Result;
        //    Assert.Equal("Testing Screening2", ((Screening)okRes.Value).Title);
        //}

        [Fact]
        public async void DeletesScreening()
        {
            var context = InMemoryDbContextFactory.GetDbContext();
            context.Add(new Screening { ScreeningId = 1 });
            await context.SaveChangesAsync();
            var controller = new ScreeningController(context);

            var response = await controller.DeleteScreening(1);

            Assert.IsType<OkObjectResult>(response.Result);
            var okRes = (OkObjectResult)response.Result;
            Assert.Equal(1, ((Screening)okRes.Value).ScreeningId);
        }
    }
}
