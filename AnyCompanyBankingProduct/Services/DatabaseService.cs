using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using AnyCompanyBankingProduct.Models;
using MySql.Data.MySqlClient;

namespace AnyCompanyBankingProduct.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;
        private readonly LoggingService _logger;

        public DatabaseService()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
            
            // Replace placeholders with environment variables if they exist
            string dbUser = Environment.GetEnvironmentVariable("DB_USER");
            string dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
            string dbServer = Environment.GetEnvironmentVariable("DB_SERVER");
            
            if (!string.IsNullOrEmpty(dbServer))
                connectionString = connectionString.Replace("localhost", dbServer);
                
            if (!string.IsNullOrEmpty(dbUser))
                connectionString = connectionString.Replace("root", dbUser);
                
            if (!string.IsNullOrEmpty(dbPassword))
                connectionString = connectionString.Replace("Pwd=;", $"Pwd={dbPassword};");
            
            _connectionString = connectionString;
            _logger = LoggingService.Instance;
        }

        public bool TestConnection()
        {
            try
            {
                _logger.LogInfo("Testing database connection");
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    _logger.LogInfo("Database connection successful");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Database connection failed: {ex.Message}");
                return false;
            }
        }

        public List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();

            try
            {
                _logger.LogInfo("Retrieving all products from database");
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM products";
                    
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Product product = new Product
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Name = reader["name"].ToString(),
                                    Type = reader["type"].ToString(),
                                    InterestRate = reader["interest_rate"] != DBNull.Value ? Convert.ToDecimal(reader["interest_rate"]) : 0,
                                    Description = reader["description"] != DBNull.Value ? reader["description"].ToString() : null,
                                    CreatedDate = reader["created_date"] != DBNull.Value ? Convert.ToDateTime(reader["created_date"]) : DateTime.MinValue,
                                    IsActive = Convert.ToBoolean(reader["is_active"])
                                };
                                
                                products.Add(product);
                            }
                        }
                    }
                }
                _logger.LogInfo($"Retrieved {products.Count} products from database");
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "GetAllProducts");
            }

            return products;
        }

        public Product GetProductById(int id)
        {
            try
            {
                _logger.LogInfo($"Retrieving product with ID {id}");
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM products WHERE id = @Id";
                    
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Product product = new Product
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Name = reader["name"].ToString(),
                                    Type = reader["type"].ToString(),
                                    InterestRate = reader["interest_rate"] != DBNull.Value ? Convert.ToDecimal(reader["interest_rate"]) : 0,
                                    Description = reader["description"] != DBNull.Value ? reader["description"].ToString() : null,
                                    CreatedDate = reader["created_date"] != DBNull.Value ? Convert.ToDateTime(reader["created_date"]) : DateTime.MinValue,
                                    IsActive = Convert.ToBoolean(reader["is_active"])
                                };
                                
                                _logger.LogInfo($"Retrieved product: {product.Name}");
                                return product;
                            }
                        }
                    }
                }
                _logger.LogWarning($"No product found with ID {id}");
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"GetProductById({id})");
            }

            return null;
        }

        public bool CreateProduct(Product product)
        {
            try
            {
                _logger.LogInfo($"Creating new product: {product.Name}");
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO products (name, type, interest_rate, description, is_active) 
                                    VALUES (@Name, @Type, @InterestRate, @Description, @IsActive)";
                    
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", product.Name);
                        command.Parameters.AddWithValue("@Type", product.Type);
                        command.Parameters.AddWithValue("@InterestRate", product.InterestRate);
                        command.Parameters.AddWithValue("@Description", product.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IsActive", product.IsActive);
                        
                        int rowsAffected = command.ExecuteNonQuery();
                        bool success = rowsAffected > 0;
                        
                        if (success)
                        {
                            _logger.LogInfo($"Product created successfully with ID {command.LastInsertedId}");
                            product.Id = Convert.ToInt32(command.LastInsertedId);
                        }
                        else
                        {
                            _logger.LogWarning("Failed to create product - no rows affected");
                        }
                        
                        return success;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "CreateProduct");
                return false;
            }
        }

        public bool UpdateProduct(Product product)
        {
            try
            {
                _logger.LogInfo($"Updating product with ID {product.Id}: {product.Name}");
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"UPDATE products 
                                    SET name = @Name, 
                                        type = @Type, 
                                        interest_rate = @InterestRate, 
                                        description = @Description, 
                                        is_active = @IsActive 
                                    WHERE id = @Id";
                    
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", product.Id);
                        command.Parameters.AddWithValue("@Name", product.Name);
                        command.Parameters.AddWithValue("@Type", product.Type);
                        command.Parameters.AddWithValue("@InterestRate", product.InterestRate);
                        command.Parameters.AddWithValue("@Description", product.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IsActive", product.IsActive);
                        
                        int rowsAffected = command.ExecuteNonQuery();
                        bool success = rowsAffected > 0;
                        
                        if (success)
                        {
                            _logger.LogInfo($"Product updated successfully: ID {product.Id}");
                        }
                        else
                        {
                            _logger.LogWarning($"Failed to update product - no rows affected for ID {product.Id}");
                        }
                        
                        return success;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"UpdateProduct({product.Id})");
                return false;
            }
        }

        public bool DeleteProduct(int id)
        {
            try
            {
                _logger.LogInfo($"Deleting product with ID {id}");
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM products WHERE id = @Id";
                    
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        
                        int rowsAffected = command.ExecuteNonQuery();
                        bool success = rowsAffected > 0;
                        
                        if (success)
                        {
                            _logger.LogInfo($"Product deleted successfully: ID {id}");
                        }
                        else
                        {
                            _logger.LogWarning($"Failed to delete product - no rows affected for ID {id}");
                        }
                        
                        return success;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"DeleteProduct({id})");
                return false;
            }
        }

        public List<string> GetProductTypes()
        {
            List<string> types = new List<string>();

            try
            {
                _logger.LogInfo("Retrieving product types");
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT DISTINCT type FROM products ORDER BY type";
                    
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                types.Add(reader["type"].ToString());
                            }
                        }
                    }
                }
                _logger.LogInfo($"Retrieved {types.Count} product types");
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "GetProductTypes");
            }

            return types;
        }
    }
}
