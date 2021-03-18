using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resto.Front.Api.CheckBookPlugin
{
    public class OrderPackage
    {
        public Guid RestaurantId { get; set; }
        public string CompanyName { get; set; }

        public OrderDto Order { get; set; }
    }
}
