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
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

public class MaterialRecordsControllerTests
{
    private MaterialRecordsController GetControllerWithDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestMaterialRecordsDatabase")
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureDeleted(); // Очищаем базу перед тестом
        context.Database.EnsureCreated();

        // Добавляем тестовые данные
        var material = new Material { MaterialId = 1, Name = "Бумага", Unit = "лист", Quantity = 50 };
        var equipment = new Equipment { EquipmentId = 1, Name = "Принтер" };
        var user = new User { UserId = 1, UserName = "admin", Password = "password123" };

        var materialRecords = new List<MaterialRecord>
        {
            new MaterialRecord { RecordId = 1, MaterialId = 1, EquipmentId = 1, UserId = 1, Quantity = 5, RecordDate = DateTime.UtcNow.AddDays(-5) }
        };

        context.Materials.Add(material);
        context.Equipments.Add(equipment);
        context.Users.Add(user);
        context.MaterialRecords.AddRange(materialRecords);
        context.SaveChanges();

        var controller = new MaterialRecordsController(context);

        // Устанавливаем мокнутый `HttpContext` с пользователем
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "admin") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var userPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = userPrincipal };
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        return controller;
    }

    [Fact]
    public async Task Index_ReturnsViewWithMaterialRecords()
    {
        // Arrange
        var controller = GetControllerWithDbContext();

        // Act
        var result = await controller.Index() as ViewResult;
        var model = result?.Model as List<MaterialRecord>;

        // Вывод в формате тест-кейса
        Console.WriteLine("\n=== ТЕСТ-КЕЙС: Список записей расхода материалов ===");
        Console.WriteLine($"[🔵 Ожидалось] 1 запись в системе");
        Console.WriteLine($"[✅ Получено] {model?.Count} записей");

        if (model != null)
        {
            foreach (var item in model)
            {
                Console.WriteLine($"[✅ Запись] Материал: {item.MaterialId}, Количество: {item.Quantity}, Дата: {item.RecordDate}");
            }
        }

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(model);
        Assert.Single(model);
    }

    [Fact]
    public async Task Create_AddsNewMaterialRecord()
    {
        // Arrange
        var controller = GetControllerWithDbContext();
        var newRecord = new MaterialRecord { MaterialId = 1, EquipmentId = 1, UserId = 1, Quantity = 10, RecordDate = DateTime.UtcNow };

        // Act
        await controller.Create(newRecord);
        var result = await controller.Index() as ViewResult;
        var model = result?.Model as List<MaterialRecord>;

        // Вывод в формате тест-кейса
        Console.WriteLine("\n=== ТЕСТ-КЕЙС: Создание новой записи расхода ===");
        Console.WriteLine($"[🔵 Ожидалось] 2 записи в системе");
        Console.WriteLine($"[✅ Получено] {model?.Count} записей");

        if (model != null)
        {
            foreach (var item in model)
            {
                Console.WriteLine($"[✅ Запись] Материал: {item.MaterialId}, Количество: {item.Quantity}, Дата: {item.RecordDate}");
            }
        }

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(model);
        Assert.Equal(2, model.Count);
    }

    [Fact]
public async Task Delete_RemovesMaterialRecordAndRestoresMaterialQuantity()
{
    // Arrange
    var controller = GetControllerWithDbContext();
    var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: "TestMaterialRecordsDatabase")
        .Options);

    var record = context.MaterialRecords.Find(1);
    if (record == null)
    {
        Console.WriteLine("[❌ Ошибка] Запись о расходе материала с ID 1 не найдена! Удаление невозможно.");
        Assert.Fail("MaterialRecord с ID 1 не существует, тест не может быть выполнен.");
        return;
    }

    double originalQuantity = context.Materials.Find(1).Quantity;
    double recordQuantity = record.Quantity;

    Console.WriteLine("\n=== ТЕСТ-КЕЙС: Удаление записи расхода и возврат материала ===");
    Console.WriteLine($"[🔵 Проверка] Исходное количество материала: {originalQuantity}");
    Console.WriteLine($"[🔵 Проверка] Запись о расходе: {recordQuantity}");

    // Act
    Console.WriteLine("[🔵 Вызов] Выполняем удаление записи...");
    await controller.Delete(1);
    Console.WriteLine("[✅ Вызов] Метод Delete() завершён!");

    // Создаём новый `context`, чтобы загрузить актуальные данные
    var updatedContext = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: "TestMaterialRecordsDatabase")
        .Options);

    var updatedMaterial = updatedContext.Materials.Find(1);
    var remainingRecords = updatedContext.MaterialRecords.Count();

    Console.WriteLine($"[🔵 Ожидалось] 0 записей в системе, [✅ Получено] {remainingRecords}");
    Console.WriteLine($"[🔵 Ожидалось] Количество материала: {originalQuantity} + {recordQuantity} = {originalQuantity + recordQuantity}");
    Console.WriteLine($"[✅ Получено] Количество материала: {updatedMaterial?.Quantity ?? -1}");

    // Assert
    Assert.NotNull(updatedMaterial);
    Assert.Equal(originalQuantity + recordQuantity, updatedMaterial.Quantity);
    Assert.Empty(updatedContext.MaterialRecords);
}


}
