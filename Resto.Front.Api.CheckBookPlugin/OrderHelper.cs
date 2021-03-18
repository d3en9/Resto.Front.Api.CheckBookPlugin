using Newtonsoft.Json;
using Resto.Front.Api.Data.Orders;
using Resto.Front.Api.Data.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resto.Front.Api.CheckBookPlugin
{
    public static class OrderHelper
    {
        public static string GetOrderPackageJson(IOrder order)
        {
            IRestaurant restaurant = null;
            try
            {
                restaurant = PluginContext.Operations.GetHostRestaurant();
            }
            catch (Exception e)
            {
                PluginContext.Log.Error($"get restaurant error", e);
                throw;
            }
            string json = null;
            try
            {
                var orderDto = OrderHelper.GetOrder(order);
                var package = new OrderPackage()
                {
                    RestaurantId = restaurant.Id,
                    CompanyName = restaurant.CompanyName,
                    Order = orderDto
                };
                json = JsonConvert.SerializeObject(package);
            }
            catch (Exception e)
            {
                PluginContext.Log.Error($"serialize order error", e);
                throw;
            }
            return json;
        }

        public static string GetOrderClosePackageJson(IOrder order)
        {
            string json = JsonConvert.SerializeObject(order.Id);
            return json;
        }

        public static OrderDto GetOrder(IOrder order)
        {
            if (order == null) return null;
            return new OrderDto
            {
                Id = order.Id,
                Number = order.Number,
                FullSum = order.FullSum,
                ResultSum = order.ResultSum,
                CashierName = order.Cashier?.Name,
                WaiterName = order.Waiter?.Name,
                BillTime = order.BillTime,
                OpenTime = order.OpenTime,
                Products = GetProducts(order.Items),
                Tables = order.Tables?.Select(_ => _.Number),
                Status = order.Status
            };
        }

        public static IEnumerable<ProductDto> GetProducts(IReadOnlyList<IOrderRootItem> items)
        {
            if (items == null) return null;
            return items?.OfType<IOrderProductItem>()?.Select(_ => 
                new ProductDto
                {
                    Name = _.Product?.Name,
                    Amount = _.Amount,
                    Cost = _.Cost,
                    Deleted = _.Deleted,
                    OpenPrice = _.OpenPrice,
                    Price = _.Price,
                    PricePredefined = _.PricePredefined,
                    ResultSum = _.ResultSum,
                    TaxPercent = _.TaxPercent
                });
        }
    }
}
