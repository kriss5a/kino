using kino.Controllers;
using kino.Database;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Xunit;

namespace kino.test.Controllers
{
    public class ScreeningRoomControllerTests
    {
        [Fact]
        public void InstantiatesController()
        {
            var context = InMemoryDbContextFactory.GetDbContext();
            var controller = new ScreeningRoomsController(context);

            Assert.NotNull(controller);
        }

        [Fact]
        public void GetsScreeningRooms()
        {
            var context = InMemoryDbContextFactory.GetDbContext();
            context.ScreeningRooms.AddRange(new ScreeningRoom(), new ScreeningRoom());
            context.SaveChanges();
            var controller = new ScreeningRoomsController(context);

            var response = controller.GetScreeningRooms();

            Assert.IsType<OkObjectResult>(response.Result);
            var okRes = (OkObjectResult)response.Result;
            Assert.Equal(2, ((List<ScreeningRoom>)okRes.Value).Count);
        }

        [Fact]
        public async void AddsScreeningRoom()
        {
            var context = InMemoryDbContextFactory.GetDbContext();
            var controller = new ScreeningRoomsController(context);

            var ScreeningRoom = new ScreeningRoom { Name = "room1" };

            var response = await controller.AddScreeningRoom(ScreeningRoom);

            Assert.IsType<OkObjectResult>(response.Result);
            var okRes = (OkObjectResult)response.Result;
            Assert.Equal("room1", ((ScreeningRoom)okRes.Value).Name);
        }

        [Fact]
        public async void PatchesScreeningRoom()
        {
            var context = InMemoryDbContextFactory.GetDbContext();
            context.Add(new ScreeningRoom { ScreeningRoomId = 1, Name = "room1" });
            await context.SaveChangesAsync();
            var controller = new ScreeningRoomsController(context);

            var ScreeningRoom = new ScreeningRoom { ScreeningRoomId = 1, Name = "room2" };

            var response = await controller.PatchRoom(ScreeningRoom.ScreeningRoomId, ScreeningRoom);

            Assert.IsType<OkObjectResult>(response.Result);
            var okRes = (OkObjectResult)response.Result;
            Assert.Equal("room2", ((ScreeningRoom)okRes.Value).Name);
        }

        [Fact]
        public async void DeletesScreeningRoom()
        {
            var context = InMemoryDbContextFactory.GetDbContext();
            context.Add(new ScreeningRoom { ScreeningRoomId = 1, Name = "room1" });
            await context.SaveChangesAsync();
            var controller = new ScreeningRoomsController(context);

            var response = await controller.DeleteScreeningRoom(1);

            Assert.IsType<OkObjectResult>(response.Result);
            var okRes = (OkObjectResult)response.Result;
            Assert.Equal("room1", ((ScreeningRoom)okRes.Value).Name);
        }
    }
}
