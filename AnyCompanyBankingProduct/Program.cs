using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Linq;
using AnyCompanyBankingProduct.Models;
using AnyCompanyBankingProduct.Services;
using AnyCompanyBankingProduct.Repositories;

namespace AnyCompanyBankingProduct
{
    class Program
    {
        private static LoggingService _logger;
        private static IConsoleService _console;
        private static IProductRepository _productRepository;
        
        static void Main(string[] args)
        {
            // Initialize services
            InitializeServices();
            
            _logger.LogInfo("Application starting");
            
            try
            {
                // Display application header with .NET Framework version
                DisplayApplicationHeader();
                
                // Animated connection attempt with visual feedback
                _console.Write("Connecting to database");
                for (int i = 0; i < 5; i++)
                {
                    _console.Write(".");
                    // Using Sleep but in a way that can be easily replaced with async/await
                    Thread.Sleep(300);
                }
                _console.WriteLine("");
                
                if (_productRepository.TestConnection())
                {
                    // Platform-agnostic console output
                    _console.WriteLineWithColor("Successfully connected to MySQL database!", ConsoleColor.Green);
                    _logger.LogInfo("Database connection successful");
                    
                    bool exit = false;
                    while (!exit)
                    {
                        DisplayMainMenu();
                        
                        string choice = _console.ReadLine();
                        _logger.LogInfo($"User selected menu option: {choice}");
                        
                        switch (choice)
                        {
                            case "1":
                                DisplayAllProducts();
                                break;
                            case "2":
                                ViewProductDetails();
                                break;
                            case "3":
                                CreateNewProduct();
                                break;
                            case "4":
                                UpdateProduct();
                                break;
                            case "5":
                                DisplaySystemInfo();
                                break;
                            case "6":
                                exit = true;
                                _logger.LogInfo("User chose to exit application");
                                break;
                            default:
                                _console.WriteLineWithColor("Invalid choice. Please try again.", ConsoleColor.Red);
                                _logger.LogWarning($"Invalid menu choice: {choice}");
                                break;
                        }
                    }
                }
                else
                {
                    _console.WriteLineWithColor("Failed to connect to the database. Please check your connection string.", ConsoleColor.Red);
                    _logger.LogError("Database connection failed");
                }
            }
            catch (Exception ex)
            {
                _console.WriteLineWithColor($"Error: {ex.Message}", ConsoleColor.Red);
                _logger.LogException(ex, "Main application");
            }
            
            _logger.LogInfo("Application shutting down");
        }
        
        private static void InitializeServices()
        {
            // Initialize service locator first
            var serviceLocator = ServiceLocator.Instance;
            
            // Get logger
            _logger = LoggingService.Instance;
            
            // Get console service
            _console = serviceLocator.GetService<IConsoleService>();
            
            // Create and register product repository
            _productRepository = new MySqlProductRepository();
            serviceLocator.RegisterService<IProductRepository>(_productRepository);
        }
        
        private static void DisplayApplicationHeader()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string frameworkVersion = Environment.Version.ToString();
            
            _console.WriteLine("=======================================================");
            _console.WriteLineWithColor("  AnyCompany Banking Product Catalogue", ConsoleColor.Cyan);
            _console.WriteLine("=======================================================");
            _console.WriteLine($"  Version: {version}");
            _console.WriteLine($"  Framework: {frameworkVersion}");
            _console.WriteLine($"  Date: {DateTime.Now:yyyy-MM-dd}");
            _console.WriteLine("=======================================================");
            _console.WriteLine("");
        }
        
        private static void DisplayMainMenu()
        {
            _console.WriteLine("");
            _console.WriteLineWithColor("MAIN MENU", ConsoleColor.Yellow);
            _console.WriteLine("1. View All Products");
            _console.WriteLine("2. View Product Details");
            _console.WriteLine("3. Create New Product");
            _console.WriteLine("4. Update Product");
            _console.WriteLine("5. System Information");
            _console.WriteLine("6. Exit");
            _console.Write("Enter your choice: ");
        }
        
        private static void DisplayAllProducts()
        {
            _console.WriteLineWithColor("\nALL BANKING PRODUCTS", ConsoleColor.Yellow);
            _console.WriteLine("=======================================================");
            
            List<Product> products = _productRepository.GetAllProducts();
            
            if (products.Count == 0)
            {
                _console.WriteLineWithColor("No products found.", ConsoleColor.Red);
                return;
            }
            
            _console.WriteLine($"{"ID",-5} {"NAME",-30} {"TYPE",-15} {"RATE",-10} {"ACTIVE",-10}");
            _console.WriteLine($"{"-",-5} {"----",-30} {"----",-15} {"----",-10} {"------",-10}");
            
            foreach (var product in products)
            {
                ConsoleColor color = product.IsActive ? ConsoleColor.White : ConsoleColor.DarkGray;
                _console.WriteWithColor($"{product.Id,-5} ", color);
                _console.WriteWithColor($"{product.Name,-30} ", color);
                _console.WriteWithColor($"{product.Type,-15} ", color);
                _console.WriteWithColor($"{product.InterestRate,-10:F2} ", color);
                _console.WriteWithColor($"{(product.IsActive ? "Yes" : "No"),-10}", color);
                _console.WriteLine("");
            }
            
            _console.WriteLine("\nPress any key to continue...");
            _console.ReadLine();
        }
        
        private static void ViewProductDetails()
        {
            _console.Write("\nEnter Product ID: ");
            string input = _console.ReadLine();
            
            if (!int.TryParse(input, out int id))
            {
                _console.WriteLineWithColor("Invalid ID. Please enter a number.", ConsoleColor.Red);
                return;
            }
            
            Product product = _productRepository.GetProductById(id);
            
            if (product == null)
            {
                _console.WriteLineWithColor($"No product found with ID {id}", ConsoleColor.Red);
                return;
            }
            
            _console.WriteLineWithColor("\nPRODUCT DETAILS", ConsoleColor.Yellow);
            _console.WriteLine("=======================================================");
            _console.WriteLine($"ID: {product.Id}");
            _console.WriteLine($"Name: {product.Name}");
            _console.WriteLine($"Type: {product.Type}");
            _console.WriteLine($"Interest Rate: {product.InterestRate:F2}%");
            _console.WriteLine($"Effective Annual Yield: {product.EffectiveAnnualYield:F2}%");
            _console.WriteLine($"Description: {product.Description}");
            _console.WriteLine($"Created Date: {product.CreatedDate:yyyy-MM-dd}");
            _console.WriteLine($"Status: {(product.IsActive ? "Active" : "Inactive")}");
            
            // Show maturity value calculation example
            decimal principal = 10000;
            int years = 5;
            decimal maturityValue = product.CalculateMaturityValue(principal, years);
            
            _console.WriteLine("\nExample Calculation:");
            _console.WriteLine($"Principal: ${principal:N2}");
            _console.WriteLine($"Term: {years} years");
            _console.WriteLine($"Maturity Value: ${maturityValue:N2}");
            
            _console.WriteLine("\nPress any key to continue...");
            _console.ReadLine();
        }
        
        private static void CreateNewProduct()
        {
            _console.WriteLineWithColor("\nCREATE NEW PRODUCT", ConsoleColor.Yellow);
            _console.WriteLine("=======================================================");
            
            Product product = new Product();
            
            _console.Write("Name: ");
            product.Name = _console.ReadLine();
            
            _console.Write("Type: ");
            product.Type = _console.ReadLine();
            
            _console.Write("Interest Rate (%): ");
            if (decimal.TryParse(_console.ReadLine(), out decimal rate))
            {
                product.InterestRate = rate;
            }
            
            _console.Write("Description: ");
            product.Description = _console.ReadLine();
            
            _console.Write("Active (y/n): ");
            product.IsActive = _console.ReadLine().ToLower() == "y";
            
            if (_productRepository.CreateProduct(product))
            {
                _console.WriteLineWithColor($"Product created successfully with ID {product.Id}", ConsoleColor.Green);
            }
            else
            {
                _console.WriteLineWithColor("Failed to create product", ConsoleColor.Red);
            }
            
            _console.WriteLine("\nPress any key to continue...");
            _console.ReadLine();
        }
        
        private static void UpdateProduct()
        {
            _console.Write("\nEnter Product ID to update: ");
            string input = _console.ReadLine();
            
            if (!int.TryParse(input, out int id))
            {
                _console.WriteLineWithColor("Invalid ID. Please enter a number.", ConsoleColor.Red);
                return;
            }
            
            Product product = _productRepository.GetProductById(id);
            
            if (product == null)
            {
                _console.WriteLineWithColor($"No product found with ID {id}", ConsoleColor.Red);
                return;
            }
            
            _console.WriteLineWithColor("\nUPDATE PRODUCT", ConsoleColor.Yellow);
            _console.WriteLine("=======================================================");
            _console.WriteLine($"Current Name: {product.Name}");
            _console.Write("New Name (leave empty to keep current): ");
            string name = _console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                product.Name = name;
            }
            
            _console.WriteLine($"Current Type: {product.Type}");
            _console.Write("New Type (leave empty to keep current): ");
            string type = _console.ReadLine();
            if (!string.IsNullOrWhiteSpace(type))
            {
                product.Type = type;
            }
            
            _console.WriteLine($"Current Interest Rate: {product.InterestRate:F2}%");
            _console.Write("New Interest Rate (leave empty to keep current): ");
            string rateInput = _console.ReadLine();
            if (!string.IsNullOrWhiteSpace(rateInput) && decimal.TryParse(rateInput, out decimal rate))
            {
                product.InterestRate = rate;
            }
            
            _console.WriteLine($"Current Description: {product.Description}");
            _console.Write("New Description (leave empty to keep current): ");
            string description = _console.ReadLine();
            if (!string.IsNullOrWhiteSpace(description))
            {
                product.Description = description;
            }
            
            _console.WriteLine($"Current Status: {(product.IsActive ? "Active" : "Inactive")}");
            _console.Write("New Status (a=Active, i=Inactive, leave empty to keep current): ");
            string status = _console.ReadLine().ToLower();
            if (status == "a")
            {
                product.IsActive = true;
            }
            else if (status == "i")
            {
                product.IsActive = false;
            }
            
            if (_productRepository.UpdateProduct(product))
            {
                _console.WriteLineWithColor("Product updated successfully", ConsoleColor.Green);
            }
            else
            {
                _console.WriteLineWithColor("Failed to update product", ConsoleColor.Red);
            }
            
            _console.WriteLine("\nPress any key to continue...");
            _console.ReadLine();
        }
        
        private static void DisplaySystemInfo()
        {
            _console.WriteLineWithColor("\nSYSTEM INFORMATION", ConsoleColor.Yellow);
            _console.WriteLine("=======================================================");
            _console.WriteLine($"OS Version: {Environment.OSVersion}");
            _console.WriteLine($"Machine Name: {Environment.MachineName}");
            _console.WriteLine($"Processor Count: {Environment.ProcessorCount}");
            _console.WriteLine($"CLR Version: {Environment.Version}");
            _console.WriteLine($"64-bit OS: {Environment.Is64BitOperatingSystem}");
            _console.WriteLine($"64-bit Process: {Environment.Is64BitProcess}");
            _console.WriteLine($"Current Directory: {Environment.CurrentDirectory}");
            
            _console.WriteLine("\nPress any key to continue...");
            _console.ReadLine();
        }
    }
}
