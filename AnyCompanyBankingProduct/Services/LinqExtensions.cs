using System;
using System.Collections.Generic;
using System.Linq;

namespace AnyCompanyBankingProduct.Services
{
    /// <summary>
    /// Cross-platform compatible LINQ extension methods
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Cross-platform compatible implementation of Average for decimal values
        /// </summary>
        public static decimal AverageDecimal<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            decimal sum = 0;
            long count = 0;
            
            foreach (TSource item in source)
            {
                sum += selector(item);
                count++;
            }
            
            if (count > 0)
                return sum / count;
            
            throw new InvalidOperationException("Sequence contains no elements");
        }
        
        /// <summary>
        /// Cross-platform compatible implementation of Max for decimal values
        /// </summary>
        public static decimal MaxDecimal<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
                
            bool hasValue = false;
            decimal result = 0;
            
            foreach (TSource item in source)
            {
                decimal value = selector(item);
                if (hasValue)
                {
                    if (value > result)
                        result = value;
                }
                else
                {
                    result = value;
                    hasValue = true;
                }
            }
            
            if (hasValue)
                return result;
                
            throw new InvalidOperationException("Sequence contains no elements");
        }
        
        /// <summary>
        /// Cross-platform compatible implementation of Min for decimal values
        /// </summary>
        public static decimal MinDecimal<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
                
            bool hasValue = false;
            decimal result = 0;
            
            foreach (TSource item in source)
            {
                decimal value = selector(item);
                if (hasValue)
                {
                    if (value < result)
                        result = value;
                }
                else
                {
                    result = value;
                    hasValue = true;
                }
            }
            
            if (hasValue)
                return result;
                
            throw new InvalidOperationException("Sequence contains no elements");
        }
    }
}
