using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacationManager.Controllers;
using VacationManager.Data;
using VacationManager.Models;
using Xunit;

namespace VacationManager.Tests
{
    public class ProjectControllerTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_ReturnsViewWithProjects()
        {
            using var context = GetDbContext();
            context.Projects.Add(new Project { Name = "Test Project", Description = "Test Description" });
            await context.SaveChangesAsync();

            var controller = new ProjectController(context);
            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Project>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public void Create_GET_ReturnsView()
        {
            var controller = new ProjectController(GetDbContext());
            var result = controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_POST_ValidProject_RedirectsToIndex()
        {
            using var context = GetDbContext();
            var controller = new ProjectController(context);
            controller.ModelState.Clear(); // 💡 Валидираме ModelState

            var project = new Project { Name = "New Project", Description = "Some description" };
            var result = await controller.Create(project);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Single(context.Projects);
        }

        [Fact]
        public async Task Create_POST_InvalidProject_ReturnsViewWithModel()
        {
            using var context = GetDbContext();
            var controller = new ProjectController(context);
            controller.ModelState.AddModelError("Name", "Името е задължително");

            var project = new Project { Description = "Без име" };
            var result = await controller.Create(project);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Project>(viewResult.Model);
            Assert.Empty(context.Projects);
        }

        [Fact]
        public async Task Create_POST_SavesProjectToDatabase()
        {
            using var context = GetDbContext();
            var controller = new ProjectController(context);
            controller.ModelState.Clear();

            var project = new Project { Name = "Saved Project", Description = "Test" };
            await controller.Create(project);

            var saved = await context.Projects.FirstOrDefaultAsync(p => p.Name == "Saved Project");
            Assert.NotNull(saved);
            Assert.Equal("Test", saved.Description);
        }
    }
}
