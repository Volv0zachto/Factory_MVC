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
using Materials.ViewModels;

public class ReportsControllerTests
{
    private ReportsController GetControllerWithDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestReportsDatabase")
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Добавляем тестовые данные
        var material = new Material { MaterialId = 1, Name = "Бумага", Unit = "лист" };
        var equipment = new Equipment { EquipmentId = 1, Name = "Принтер" };
        var user = new User { UserId = 1, UserName = "admin", Password = "password123" };

        var materialRecords = new List<MaterialRecord>
        {
            new MaterialRecord { RecordId = 1, MaterialId = 1, EquipmentId = 1, UserId = 1, Quantity = 5, RecordDate = DateTime.UtcNow.AddDays(-5) },
            new MaterialRecord { RecordId = 2, MaterialId = 1, EquipmentId = 1, UserId = 1, Quantity = 10, RecordDate = DateTime.UtcNow.AddDays(-3) },
            new MaterialRecord { RecordId = 3, MaterialId = 1, EquipmentId = 1, UserId = 1, Quantity = 15, RecordDate = DateTime.UtcNow.AddDays(-1) }
        };

        context.Materials.Add(material);
        context.Equipments.Add(equipment);
        context.Users.Add(user);
        context.MaterialRecords.AddRange(materialRecords);
        context.SaveChanges();

        return new ReportsController(context);
    }

    [Fact]
    public async Task MaterialUsageReport_ReturnsCorrectData()
    {
        // Arrange
        var controller = GetControllerWithDbContext();
        DateTime startDate = DateTime.UtcNow.AddDays(-7);
        DateTime endDate = DateTime.UtcNow;

        // Act
        var result = await controller.MaterialUsageReport(startDate, endDate) as ViewResult;
        var model = result?.Model as List<MaterialUsageReportViewModel>;

        // Формируем вывод как тест-кейс
        Console.WriteLine("\n=== ТЕСТ-КЕЙС: Отчёт об использовании материалов ===");
        Console.WriteLine($"[🔵 Ожидалось] 1 материал в отчёте");
        Console.WriteLine($"[✅ Получено] {model?.Count} материалов");
        
        if (model != null && model.Count > 0)
        {
            foreach (var item in model)
            {
                Console.WriteLine($"[🔵 Ожидалось] Материал: 'Бумага', Всего использовано: 30 листов");
                Console.WriteLine($"[✅ Получено] Материал: '{item.Material.Name}', Всего использовано: {item.TotalUsed} {item.Material.Unit}");
            }
        }
        else
        {
            Console.WriteLine("[❌ Ошибка] Отчёт пуст! Материалы не найдены.");
        }

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(model);
        Assert.Single(model);
        Assert.Equal(30, model[0].TotalUsed);
    }

    [Fact]
    public async Task AverageMaterialConsumptionReport_CalculatesCorrectly()
    {
        // Arrange
        var controller = GetControllerWithDbContext();
        DateTime startDate = DateTime.UtcNow.AddDays(-7);
        DateTime endDate = DateTime.UtcNow;

        // Act
        var result = await controller.AverageMaterialConsumptionReport(startDate, endDate) as ViewResult;
        var model = result?.Model as List<AverageMaterialConsumptionReportViewModel>;

        // Формируем вывод как тест-кейс
        Console.WriteLine("\n=== ТЕСТ-КЕЙС: Средний расход материалов ===");
        Console.WriteLine($"[🔵 Ожидалось] 1 материал в отчёте");
        Console.WriteLine($"[✅ Получено] {model?.Count} материалов");

        if (model != null && model.Count > 0)
        {
            foreach (var item in model)
            {
                double expectedAvg = 30 / 7.0;
                Console.WriteLine($"[🔵 Ожидалось] Материал: 'Бумага', Всего использовано: 30 листов, Средний расход в день: {expectedAvg:F2}");
                Console.WriteLine($"[✅ Получено] Материал: '{item.Material.Name}', Всего использовано: {item.TotalUsed} {item.Material.Unit}, Средний расход в день: {item.AverageConsumptionPerDay:F2}");
            }
        }
        else
        {
            Console.WriteLine("[❌ Ошибка] Отчёт пуст! Материалы не найдены.");
        }

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(model);
        Assert.Single(model);
        Assert.Equal(30, model[0].TotalUsed);
        Assert.Equal(30 / 7.0, model[0].AverageConsumptionPerDay, 2);
    }
}
