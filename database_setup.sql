-- Create the database
CREATE DATABASE IF NOT EXISTS bankingdb;
USE bankingdb;

-- Create products table
CREATE TABLE IF NOT EXISTS products (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    type VARCHAR(50) NOT NULL,
    interest_rate DECIMAL(5,2),
    description TEXT,
    created_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    is_active BOOLEAN DEFAULT TRUE
);

-- Insert sample data
INSERT INTO products (name, type, interest_rate, description, is_active) VALUES
('Basic Savings Account', 'Savings', 0.50, 'A simple savings account with no minimum balance requirements', TRUE),
('Premium Checking Account', 'Checking', 0.25, 'Checking account with premium benefits and no monthly fees', TRUE),
('High-Yield Savings', 'Savings', 1.75, 'Higher interest rate for balances over $10,000', TRUE),
('Student Checking', 'Checking', 0.10, 'No-fee checking account for students', TRUE),
('Fixed Deposit - 1 Year', 'Fixed Deposit', 2.50, '1-year term deposit with guaranteed returns', TRUE),
('Fixed Deposit - 3 Year', 'Fixed Deposit', 3.25, '3-year term deposit with guaranteed returns', TRUE),
('Home Loan - 15 Year', 'Mortgage', 4.50, '15-year fixed rate mortgage', TRUE),
('Home Loan - 30 Year', 'Mortgage', 4.75, '30-year fixed rate mortgage', TRUE),
('Personal Loan', 'Loan', 8.50, 'Unsecured personal loan for various needs', TRUE),
('Business Credit Line', 'Credit', 7.25, 'Revolving credit line for small businesses', FALSE);
