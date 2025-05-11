using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AnyCompanyBankingProduct.Models
{
    public class Product : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private string _type;
        private decimal _interestRate;
        private string _description;
        private DateTime _createdDate;
        private bool _isActive;

        public int Id 
        { 
            get => _id; 
            set 
            { 
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged();
                }
            } 
        }
        
        public string Name 
        { 
            get => _name; 
            set 
            { 
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged();
                }
            } 
        }
        
        public string Type 
        { 
            get => _type; 
            set 
            { 
                if (_type != value)
                {
                    _type = value;
                    NotifyPropertyChanged();
                }
            } 
        }
        
        public decimal InterestRate 
        { 
            get => _interestRate; 
            set 
            { 
                if (_interestRate != value)
                {
                    _interestRate = value;
                    NotifyPropertyChanged();
                }
            } 
        }
        
        public string Description 
        { 
            get => _description; 
            set 
            { 
                if (_description != value)
                {
                    _description = value;
                    NotifyPropertyChanged();
                }
            } 
        }
        
        public DateTime CreatedDate 
        { 
            get => _createdDate; 
            set 
            { 
                if (_createdDate != value)
                {
                    _createdDate = value;
                    NotifyPropertyChanged();
                }
            } 
        }
        
        public bool IsActive 
        { 
            get => _isActive; 
            set 
            { 
                if (_isActive != value)
                {
                    _isActive = value;
                    NotifyPropertyChanged();
                }
            } 
        }

        // Calculate the effective annual yield based on interest rate
        public decimal EffectiveAnnualYield
        {
            get
            {
                // Simple formula: (1 + r/n)^n - 1 where r is rate and n is compounding periods (12 for monthly)
                decimal rate = InterestRate / 100;
                return Math.Round(((decimal)Math.Pow((double)(1 + rate / 12), 12) - 1) * 100, 2);
            }
        }

        // Dynamic property to calculate maturity value based on a principal amount
        public decimal CalculateMaturityValue(decimal principal, int years)
        {
            decimal rate = InterestRate / 100;
            return Math.Round(principal * (decimal)Math.Pow((double)(1 + rate), years), 2);
        }

        public override string ToString()
        {
            return $"{Id}: {Name} ({Type}) - {InterestRate}% - {(IsActive ? "Active" : "Inactive")}";
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
