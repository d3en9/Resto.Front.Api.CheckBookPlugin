using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resto.Front.Api.CheckBookPlugin
{
    public class OrderClosePackage
    {
        public Guid RestaurantId { get; set; }
        public Guid OrderId { get; set; }
    }
}
