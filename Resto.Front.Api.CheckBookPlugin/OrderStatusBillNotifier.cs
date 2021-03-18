using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Runtime.Remoting;
using Resto.Front.Api.Attributes.JetBrains;
using Resto.Front.Api.Data.Orders;
using Resto.Front.Api.Data.Organization;
using Resto.Front.Api.Extensions;

namespace Resto.Front.Api.CheckBookPlugin
{
    public class OrderStatusBillNotifier : IDisposable
    {
        #region Internal logic
        private readonly Stack<IDisposable> subscriptions = new Stack<IDisposable>();

        public OrderStatusBillNotifier()
        {
            //var orders = PluginContext.Operations.GetOrders();
            //var billOrderIds = orders
            //   .Where(o => o.Status == OrderStatus.Bill)
            //   .Select(o => o.Id)
            //   .ToList();
            //subscriptions.Push(
            //    PluginContext.Notifications.OrderChanged
            //        .Select(e => e.Entity)
            //        .Where(o => o.Status == OrderStatus.Bill
            //            && !billOrderIds.Contains(o.Id))
            //        .Do(o => billOrderIds.Add(o.Id))
            //        .Subscribe(OnOrderBill));

            //var closedOrderIds = orders
            //   .Where(o => o.Status == OrderStatus.Closed)
            //   .Select(o => o.Id)
            //   .ToList();
            //subscriptions.Push(
            //    PluginContext.Notifications.OrderChanged
            //        .Select(e => e.Entity)
            //        .Where(o => o.Status == OrderStatus.Closed
            //            && !closedOrderIds.Contains(o.Id))
            //        .Do(o => closedOrderIds.Add(o.Id))
            //        .Subscribe(OnOrderClosed));
            PluginContext.Log.Info("plugin OrderStatusBillNotifier initialize.");
            subscriptions.Push(
                PluginContext.Notifications.OrderChanged
                    .Select(e => e.Entity)
                    .Subscribe(OnOrderChange));
        }

        public void Dispose()
        {
            PluginContext.Log.Info("plugin OrderStatusBillNotifier dispose.");
            while (subscriptions.Any())
            {
                var subscription = subscriptions.Pop();
                try
                {
                    subscription.Dispose();
                }
                catch (RemotingException re)
                {
                    // nothing to do with the lost connection
                    PluginContext.Log.Error("dispose error", re);
                }
                catch (Exception ex)
                {
                    PluginContext.Log.Error("dispose error", ex);
                }
            }
        }
        #endregion

        /*private void OnOrderClosed(IOrder order)
        {
            PluginContext.Log.Info($"Order {order.Number} ({order.Id}) is Close.");
            var json = OrderHelper.GetOrderClosePackageJson(order);
            HttpSender.AddTask(new HttpSenderTask { Url = HttpSender.PostChequeUrl, Json = json, Method = HttpMethod.Delete });
        }*/

        /*private void OnOrderBill(IOrder order)
        {
            PluginContext.Log.Info($"Order {order.Number} ({order.Id}) is Bill.");
            var json = OrderHelper.GetOrderPackageJson(order);
            HttpSender.AddTask(new HttpSenderTask { Url = HttpSender.PostChequeUrl, Json = json, Method = HttpMethod.Post });
        }*/

        private void OnOrderChange(IOrder order)
        {
            PluginContext.Log.Info($"Order {order.Number} ({order.Id}) is Changed.");
            string json = null;
            try
            {
                json = OrderHelper.GetOrderPackageJson(order);
            }
            catch (Exception e)
            {
                PluginContext.Log.Error($"get order error by {order.Id}", e);
            }
            
            if (json != null)
            {
                try
                {
                    HttpSender.AddTask(new HttpSenderTask { Url = HttpSender.PostChequeUrl, Json = json, Method = HttpMethod.Post });
                }
                catch (Exception e)
                {
                    PluginContext.Log.Error($"add task to HttpSender error", e);
                }
            }
            
        }
    }
}
