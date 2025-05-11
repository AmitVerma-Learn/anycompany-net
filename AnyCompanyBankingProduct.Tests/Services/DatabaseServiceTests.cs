using Microsoft.VisualStudio.TestTools.UnitTesting;
using AnyCompanyBankingProduct.Services;
using AnyCompanyBankingProduct.Models;
using AnyCompanyBankingProduct.Repositories;
using System;
using System.Collections.Generic;
using Moq;
using System.Reflection;
using System.Configuration;

namespace AnyCompanyBankingProduct.Tests.Services
{
    [TestClass]
    public class DatabaseServiceTests
    {
        // Note: These tests use reflection to test the MySqlProductRepository without actual database connections
        // In a real-world scenario, you might use a test database or mock the database layer

        [TestInitialize]
        public void TestInitialize()
        {
            // Add a test connection string to the configuration if it doesn't exist
            if (ConfigurationManager.ConnectionStrings["MySQLConnection"] == null)
            {
                // Create a test configuration
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var connStringSettings = new ConnectionStringSettings(
                    "MySQLConnection",
                    "Server=localhost;Database=bankingdb_test;Uid=test;Pwd=test;",
                    "MySql.Data.MySqlClient");

                config.ConnectionStrings.ConnectionStrings.Add(connStringSettings);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("connectionStrings");
            }

            // Initialize service locator with required services for testing
            var serviceLocator = ServiceLocator.Instance;
            serviceLocator.RegisterService<IConfigurationService>(new ConfigurationService());
            serviceLocator.RegisterService<IFileSystemService>(new FileSystemService());
            serviceLocator.RegisterService<IConsoleService>(new ConsoleService());
        }

        [TestMethod]
        public void GetProductById_ReturnsNull_WhenProductNotFound()
        {
            // Arrange
            var repository = CreateMockRepository();

            // Act
            var result = repository.GetProductById(999); // Non-existent ID

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetAllProducts_ReturnsEmptyList_WhenNoProductsExist()
        {
            // Arrange
            var repository = CreateMockRepository();
            SetPrivateField(repository, "_connectionString", "invalid_connection_string");

            // Act
            var result = repository.GetAllProducts();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void CreateProduct_ReturnsFalse_WhenConnectionFails()
        {
            // Arrange
            var repository = CreateMockRepository();
            SetPrivateField(repository, "_connectionString", "invalid_connection_string");
            var product = new Product
            {
                Name = "Test Product",
                Type = "Test Type",
                InterestRate = 1.0m,
                Description = "Test Description",
                IsActive = true
            };

            // Act
            var result = repository.CreateProduct(product);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UpdateProduct_ReturnsFalse_WhenConnectionFails()
        {
            // Arrange
            var repository = CreateMockRepository();
            SetPrivateField(repository, "_connectionString", "invalid_connection_string");
            var product = new Product
            {
                Id = 1,
                Name = "Updated Product",
                Type = "Updated Type",
                InterestRate = 2.0m,
                Description = "Updated Description",
                IsActive = true
            };

            // Act
            var result = repository.UpdateProduct(product);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetProductTypes_ReturnsEmptyList_WhenConnectionFails()
        {
            // Arrange
            var repository = CreateMockRepository();
            SetPrivateField(repository, "_connectionString", "invalid_connection_string");

            // Act
            var result = repository.GetProductTypes();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void TestConnection_ReturnsFalse_WhenConnectionStringIsInvalid()
        {
            // Arrange
            var repository = CreateMockRepository();
            SetPrivateField(repository, "_connectionString", "invalid_connection_string");

            // Act
            var result = repository.TestConnection();

            // Assert
            Assert.IsFalse(result);
        }

        // Helper methods
        private MySqlProductRepository CreateMockRepository()
        {
            return new MySqlProductRepository();
        }

        private void SetPrivateField(object instance, string fieldName, object value)
        {
            Type type = instance.GetType();
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(instance, value);
        }
    }
}
