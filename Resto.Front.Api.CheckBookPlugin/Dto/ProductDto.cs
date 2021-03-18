using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resto.Front.Api.CheckBookPlugin
{
    public class ProductDto
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal Cost { get; set; }
        public bool Deleted { get; set; }

        /// <summary>
        /// Manually set price of the product.
        /// </summary>
        public decimal? OpenPrice { get; set; }

        public decimal Price { get; set; }

        public bool PricePredefined { get; set; }

        /// <summary>
        /// Total sum to be paid, sum with discounts and both included and excluded vat.
        /// </summary>
        public decimal ResultSum { get; set; }

        /// <summary>
        /// Tax percent or null for not taxed order items.
        /// </summary>
        public decimal? TaxPercent { get; set; }
    }
}
