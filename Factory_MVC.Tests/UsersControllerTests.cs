using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Materials.Controllers;
using Materials.Models;
using Materials.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Materials.Tests
{
    public class UsersControllerTests
    {
        private UsersController GetControllerWithDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestUsersDatabase")
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            
            var adminRole = new Role { RoleId = 1, Name = "Admin" };
            var bossRole = new Role { RoleId = 2, Name = "Boss" };
            var accountantRole = new Role { RoleId = 3, Name = "Accountant" };
            context.Roles.AddRange(adminRole, bossRole, accountantRole);
            context.SaveChanges();

            
            var user1 = new User
                { UserId = 1, UserName = "AdminUser", Password = "12345", RoleId = 1, Role = adminRole };
            var user2 = new User
                { UserId = 2, UserName = "BossUser", Password = "qwerty", RoleId = 2, Role = bossRole };
            context.Users.AddRange(user1, user2);
            context.SaveChanges();

            return new UsersController(context);
        }

        [Fact]
        public async Task Index_ReturnsViewWithUsers()
        {
            // Arrange
            var controller = GetControllerWithDbContext();

            // Act
            var result = await controller.Index() as ViewResult;
            var model = result?.Model as List<User>;

            Console.WriteLine("\n=== ТЕСТ-КЕЙС: Список пользователей ===");
            Console.WriteLine($"[🔵 Ожидалось] 2 пользователя в списке");
            Console.WriteLine($"[✅ Получено] {model?.Count} пользователей");

            if (model != null)
            {
                foreach (var item in model)
                {
                    Console.WriteLine($"[✅ Пользователь] {item.UserName}, Роль: {item.Role.Name}");
                }
            }

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task Create_AddsNewUser()
        {
            // Arrange
            var controller = GetControllerWithDbContext();
            var newUser = new User { UserId = 3, UserName = "AccountantUser", Password = "password", RoleId = 3 };

            // Act
            Console.WriteLine("\n=== ТЕСТ-КЕЙС: Добавление нового пользователя ===");
            var result = await controller.Index() as ViewResult;
            var model = result?.Model as List<User>;
            Console.WriteLine($"[✅ Получено] {model?.Count} пользователей");

            await controller.Create(newUser);
            result = await controller.Index() as ViewResult;
            model = result?.Model as List<User>;
            Console.WriteLine($"[🔵 Ожидалось] 3 пользователя в списке");
            Console.WriteLine($"[✅ Получено] {model?.Count} пользователей");

            if (model != null)
            {
                foreach (var item in model)
                {
                    Console.WriteLine($"[✅ Пользователь] {item.UserName}, Роль: {item.RoleId}");
                }
            }

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Equal(3, model.Count);
            Assert.Contains(model, u => u.UserName == "AccountantUser");
        }

        [Fact]
        public async Task Delete_RemovesUser()
        {
           
            var controller = GetControllerWithDbContext();

           
            var initialResult = await controller.Index() as ViewResult;
            var initialModel = initialResult?.Model as List<User>;

            Console.WriteLine("\n=== ТЕСТ-КЕЙС: Удаление пользователя ===");
            Console.WriteLine("[🔵 Исходное состояние] До удаления:");
            if (initialModel != null)
            {
                foreach (var item in initialModel)
                {
                    Console.WriteLine($"[📌 Пользователь] {item.UserName}, Роль: {item.RoleId}");
                }
            }

           
            await controller.Delete(1);

            
            var result = await controller.Index() as ViewResult;
            var model = result?.Model as List<User>;

            Console.WriteLine("[🔵 Изменённое состояние] После удаления:");
            Console.WriteLine($"[🔵 Ожидалось] 1 пользователь в списке");
            Console.WriteLine($"[✅ Получено] {model?.Count} пользователей");

            if (model != null)
            {
                foreach (var item in model)
                {
                    Console.WriteLine($"[✅ Остался пользователь] {item.UserName}, Роль: {item.RoleId}");
                }
            }

          
            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Single(model);
            Assert.DoesNotContain(model, u => u.UserId == 1);
        }

        
        [Fact]
        public async Task Edit_UpdatesUser()
        {
          
            var controller = GetControllerWithDbContext();

            
            var resultBeforeEdit = await controller.Index() as ViewResult;
            var modelBeforeEdit = resultBeforeEdit?.Model as List<User>;
            var userToEdit = modelBeforeEdit?.FirstOrDefault(u => u.UserId == 1);
    
            if (userToEdit == null)
            {
                Assert.Fail("Пользователь с ID=1 не найден, тест невозможен.");
                return;
            }

            Console.WriteLine("\n=== ТЕСТ-КЕЙС: Редактирование пользователя ===");
            Console.WriteLine($"[📌 Исходное имя пользователя] {userToEdit.UserName}");

            
            userToEdit.UserName = "UpdatedAdmin";

            await controller.Edit(userToEdit);

          
            var resultAfterEdit = await controller.Index() as ViewResult;
            var modelAfterEdit = resultAfterEdit?.Model as List<User>;
            var updatedUser = modelAfterEdit?.FirstOrDefault(u => u.UserId == 1);

            Console.WriteLine($"[🔵 Ожидалось] Пользователь с ID=1 теперь 'UpdatedAdmin'");
            Console.WriteLine($"[✅ Получено] Пользователь с ID=1 теперь '{updatedUser?.UserName}'");

            Assert.NotNull(updatedUser);
            Assert.Equal("UpdatedAdmin", updatedUser.UserName);
        }


    }
}