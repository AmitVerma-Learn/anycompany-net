using Microsoft.VisualStudio.TestTools.UnitTesting;
using AnyCompanyBankingProduct.Models;
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace AnyCompanyBankingProduct.Tests.Models
{
    [TestClass]
    public class ProductTests
    {
        [TestMethod]
        public void Product_Constructor_InitializesProperties()
        {
            // Arrange & Act
            var product = new Product();
            
            // Assert
            Assert.AreEqual(0, product.Id);
            Assert.IsNull(product.Name);
            Assert.IsNull(product.Type);
            Assert.AreEqual(0m, product.InterestRate);
            Assert.IsNull(product.Description);
            Assert.AreEqual(default(DateTime), product.CreatedDate);
            Assert.AreEqual(false, product.IsActive);
        }

        [TestMethod]
        public void Product_SetProperties_ChangesValues()
        {
            // Arrange
            var product = new Product();
            
            // Act
            product.Id = 1;
            product.Name = "Test Product";
            product.Type = "Savings";
            product.InterestRate = 2.5m;
            product.Description = "Test Description";
            var date = DateTime.Now;
            product.CreatedDate = date;
            product.IsActive = true;
            
            // Assert
            Assert.AreEqual(1, product.Id);
            Assert.AreEqual("Test Product", product.Name);
            Assert.AreEqual("Savings", product.Type);
            Assert.AreEqual(2.5m, product.InterestRate);
            Assert.AreEqual("Test Description", product.Description);
            Assert.AreEqual(date, product.CreatedDate);
            Assert.AreEqual(true, product.IsActive);
        }

        [TestMethod]
        public void EffectiveAnnualYield_CalculatesCorrectly()
        {
            // Arrange
            var product = new Product { InterestRate = 5.0m };
            
            // Act
            decimal yield = product.EffectiveAnnualYield;
            
            // Assert - approximate comparison due to floating point calculations
            Assert.IsTrue(Math.Abs(yield - 5.12m) < 0.01m);
        }

        [TestMethod]
        public void CalculateMaturityValue_CalculatesCorrectly()
        {
            // Arrange
            var product = new Product { InterestRate = 5.0m };
            decimal principal = 1000m;
            int years = 3;
            
            // Act
            decimal maturityValue = product.CalculateMaturityValue(principal, years);
            
            // Assert - approximate comparison due to floating point calculations
            Assert.IsTrue(Math.Abs(maturityValue - 1157.63m) < 0.01m);
        }

        [TestMethod]
        public void ToString_ReturnsFormattedString()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Type = "Savings",
                InterestRate = 2.5m,
                IsActive = true
            };
            
            // Act
            string result = product.ToString();
            
            // Assert
            Assert.AreEqual("1: Test Product (Savings) - 2.5% - Active", result);
        }

        [TestMethod]
        public void PropertyChanged_FiresWhenPropertyChanges()
        {
            // Arrange
            var product = new Product();
            List<string> changedProperties = new List<string>();
            product.PropertyChanged += (sender, e) => changedProperties.Add(e.PropertyName);
            
            // Act
            product.Name = "Test Product";
            product.InterestRate = 3.5m;
            
            // Assert
            Assert.AreEqual(2, changedProperties.Count);
            Assert.IsTrue(changedProperties.Contains("Name"));
            Assert.IsTrue(changedProperties.Contains("InterestRate"));
        }
    }
}
