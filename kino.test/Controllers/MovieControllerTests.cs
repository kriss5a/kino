using kino.Controllers;
using kino.Database;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Xunit;

namespace kino.test.Controllers
{
    public class MovieControllerTests
    {
        [Fact]
        public void InstantiatesController()
        {
            var context = InMemoryDbContextFactory.GetDbContext();
            var controller = new MovieController(context);

            Assert.NotNull(controller);
        }

        [Fact]
        public void GetsMovies()
        {
            var context = InMemoryDbContextFactory.GetDbContext();
            context.Movies.AddRange(new Movie(), new Movie());
            context.SaveChanges();
            var controller = new MovieController(context);

            var response = controller.GetMovies();

            Assert.IsType<OkObjectResult>(response.Result);
            var okRes = (OkObjectResult)response.Result;
            Assert.Equal(2, ((List<Movie>)okRes.Value).Count);
        }

        [Fact]
        public async void AddsMovie()
        {
            var context = InMemoryDbContextFactory.GetDbContext();
            var controller = new MovieController(context);

            var movie = new Movie { Title = "Testing Movie1" };

            var response = await controller.AddMovie(movie);

            Assert.IsType<OkObjectResult>(response.Result);
            var okRes = (OkObjectResult)response.Result;
            Assert.Equal("Testing Movie1", ((Movie)okRes.Value).Title);
        }

        [Fact]
        public async void PatchesMovie()
        {
            var context = InMemoryDbContextFactory.GetDbContext();
            context.Add(new Movie { MovieId = 1, Title = "Testing Movie1" });
            await context.SaveChangesAsync();
            var controller = new MovieController(context);

            var movie = new Movie { MovieId = 1, Title = "Testing Movie2" };

            var response = await controller.PatchMovie(movie.MovieId, movie);

            Assert.IsType<OkObjectResult>(response.Result);
            var okRes = (OkObjectResult)response.Result;
            Assert.Equal("Testing Movie2", ((Movie)okRes.Value).Title);
        }

        [Fact]
        public async void DeletesMovie()
        {
            var context = InMemoryDbContextFactory.GetDbContext();
            context.Add(new Movie { MovieId = 1, Title = "Testing Movie1" });
            await context.SaveChangesAsync();
            var controller = new MovieController(context);

            var response = await controller.DeleteMovie(1);

            Assert.IsType<OkObjectResult>(response.Result);
            var okRes = (OkObjectResult)response.Result;
            Assert.Equal("Testing Movie1", ((Movie)okRes.Value).Title);
        }
    }
}
