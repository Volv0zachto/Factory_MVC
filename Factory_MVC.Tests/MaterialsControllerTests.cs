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

public class MaterialsControllerTests
{
    private MaterialsController GetControllerWithDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestMaterialsDatabase")
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Добавляем тестовые данные
        var material1 = new Material { MaterialId = 1, Name = "Бумага", Unit = "лист", Quantity = 50 };
        var material2 = new Material { MaterialId = 2, Name = "Чернила", Unit = "мл", Quantity = 100 };

        context.Materials.AddRange(material1, material2);
        context.SaveChanges();

        return new MaterialsController(context);
    }

    [Fact]
    public async Task Index_ReturnsViewWithMaterials()
    {
        // Arrange
        var controller = GetControllerWithDbContext();

        // Act
        var result = await controller.Index() as ViewResult;
        var model = result?.Model as List<Material>;

        // Формируем вывод как тест-кейс
        Console.WriteLine("\n=== ТЕСТ-КЕЙС: Список материалов ===");
        Console.WriteLine($"[🔵 Ожидалось] 2 материала в списке");
        Console.WriteLine($"[✅ Получено] {model?.Count} материалов");

        if (model != null)
        {
            foreach (var item in model)
            {
                Console.WriteLine($"[✅ Материал] {item.Name}, Количество: {item.Quantity} {item.Unit}");
            }
        }
        else
        {
            Console.WriteLine("[❌ Ошибка] Список материалов пуст!");
        }

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(model);
        Assert.Equal(2, model.Count);
    }

    [Fact]
    public async Task Create_AddsNewMaterial()
    {
        // Arrange
        var controller = GetControllerWithDbContext();
        var newMaterial = new Material { MaterialId = 3, Name = "Картридж", Unit = "шт", Quantity = 2 };

        // Act
        await controller.Create(newMaterial);
        var result = await controller.Index() as ViewResult;
        var model = result?.Model as List<Material>;

        // Формируем вывод как тест-кейс
        Console.WriteLine("\n=== ТЕСТ-КЕЙС: Добавление нового материала ===");
        Console.WriteLine($"[🔵 Ожидалось] 3 материала в списке");
        Console.WriteLine($"[✅ Получено] {model?.Count} материалов");

        if (model != null)
        {
            foreach (var item in model)
            {
                Console.WriteLine($"[✅ Материал] {item.Name}, Количество: {item.Quantity} {item.Unit}");
            }
        }

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(model);
        Assert.Equal(3, model.Count);
        Assert.Contains(model, m => m.Name == "Картридж" && m.Quantity == 2);
    }

    [Fact]
    public async Task Delete_RemovesMaterial()
    {
        // Arrange
        var controller = GetControllerWithDbContext();
    
        // Получаем список материалов ДО удаления
        var initialResult = await controller.Index() as ViewResult;
        var initialModel = initialResult?.Model as List<Material>;

        Console.WriteLine("\n=== ТЕСТ-КЕЙС: Удаление материала ===");
        if (initialModel != null)
        {
            foreach (var item in initialModel)
            {
                Console.WriteLine($"[🔵 Материал] {item.Name}, Количество: {item.Quantity} {item.Unit}");
            }
        }

        // Act
        await controller.Delete(1);

        // Получаем список материалов ПОСЛЕ удаления
        var result = await controller.Index() as ViewResult;
        var model = result?.Model as List<Material>;

        Console.WriteLine("[🔵 Изменённое состояние] После удаления:");
        Console.WriteLine($"[🔵 Ожидалось] 1 материал в списке (вместо 2)");
        Console.WriteLine($"[✅ Получено] {model?.Count} материалов");

        if (model != null)
        {
            foreach (var item in model)
            {
                Console.WriteLine($"[✅ Остался материал] {item.Name}, Количество: {item.Quantity} {item.Unit}");
            }
        }

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(model);
        Assert.Single(model);
        Assert.DoesNotContain(model, m => m.MaterialId == 1);
    }

    [Fact]
    public async Task IncreaseQuantity_UpdatesMaterialQuantity()
    {
        // Arrange
        var controller = GetControllerWithDbContext();
        int materialId = 1;
        double addedQuantity = 20;

        // Act
        var result = await controller.Index() as ViewResult;
        var model = result?.Model as List<Material>;
        var updatedMaterial = model?.FirstOrDefault(m => m.MaterialId == materialId);
        Console.WriteLine("\n=== ТЕСТ-КЕЙС: Пополнение количества материала ===");

        Console.WriteLine($"[✅ Получено]  Материал '{updatedMaterial?.Name}'  {updatedMaterial?.Quantity} {updatedMaterial?.Unit}");
        await controller.IncreaseQuantity(materialId, addedQuantity);
        result = await controller.Index() as ViewResult;
        model = result?.Model as List<Material>;
        updatedMaterial = model?.FirstOrDefault(m => m.MaterialId == materialId);
        // Формируем вывод как тест-кейс
        Console.WriteLine($"[🔵 Ожидалось] У материала 'Бумага' должно стать 70 листов (50+20)");
        Console.WriteLine($"[✅ Получено]  Материал '{updatedMaterial?.Name}'  {updatedMaterial?.Quantity} {updatedMaterial?.Unit}");

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(model);
        Assert.Equal(70, updatedMaterial?.Quantity);
    }
}
