using Resto.Front.Api.Data.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resto.Front.Api.CheckBookPlugin
{
    public class OrderDto
    {
        public Guid Id { get; set; }

        public int Number { get; set; }

        /// <summary>
        /// Subtotal, sum of all not deleted order items with included vat, but without excluded vat and discounts.
        /// </summary>
        public decimal FullSum { get; set; }

        /// <summary>
        /// Total sum to be paid, sum of all not deleted order items with discounts and both included and excluded vat.
        /// </summary>
        public decimal ResultSum { get; set; }

        /// <summary>
        /// Gets the user closed the current order.
        /// </summary>
        public string CashierName { get; set; }

        /// <summary>
        /// тоже какой то IUser вроде как тот кто авторизован на терминале
        /// </summary>
        public string WaiterName { get; set; }

        /// <summary>
        /// Gets the bill cheque print time of the current order.
        /// </summary>
        public DateTime? BillTime { get; set; }

        public DateTime OpenTime { get; set; }

        public IEnumerable<ProductDto> Products { get; set; }

        public IEnumerable<int> Tables { get; set; }

        public OrderStatus Status { get; set; }
    }
}
