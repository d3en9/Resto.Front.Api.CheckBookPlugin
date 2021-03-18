using System;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Text;
using System.Windows;
using Resto.Front.Api.Attributes.JetBrains;
using Resto.Front.Api.Data.Cheques;
using Resto.Front.Api.Data.Common;
using Resto.Front.Api.Data.Orders;
using Resto.Front.Api.Data.PreliminaryOrders;
using Resto.Front.Api.Extensions;
using Resto.Front.Api.UI;

namespace Resto.Front.Api.SamplePlugin.NotificationHandlers
{
    public class BeforeOrderBillHandler : IDisposable
    {
        private readonly IDisposable subscription; 

        public BeforeOrderBillHandler()
        {
            //subscription = PluginContext.Notifications.SubscribeOnBeforeOrderBill(OnBeforeOrderBill);
            //subscription = PluginContext.Notifications.PreliminaryOrderChanged.Subscribe(o => onNext(o));
            //subscription = PluginContext.Notifications.SubscribeOnBillChequePrinting(onChecque);
            subscription = PluginContext.Notifications.SubscribeOnServiceChequePrinted(printed);
            //PluginContext.Operations.RegisterChequeTaskProcessor(;
        }

        private void printed(IOrder order, IOperationService service, IViewManager manager)
        {
            Debug.WriteLine(order);
        }

        private BillCheque onChecque(Guid orderId)
        {
            
            var order = PluginContext.Operations.GetOrderById(orderId);
            Debug.WriteLine(order);
            StringBuilder sb = new StringBuilder();
            foreach(var item in order.Items)
            {
                var product = item as IOrderProductItem;
                if (product != null)
                {
                    
                    sb.Append($"{product.Product.Name} Amount: {product.Amount}, Cost: {product.Cost}, ResultSum: {product.ResultSum}{Environment.NewLine}");
                    Debug.WriteLine(item);
                }
                
            }
            MessageBox.Show($"#{order.Number}, sum: {order.ResultSum} FullSum {order.FullSum}" + Environment.NewLine + sb.ToString());
            return null;
        }

        private void onNext(EntityChangedEventArgs<IPreliminaryOrder> e)
        {
            Debug.WriteLine(e);
        }

        private void OnBeforeOrderBill(IOrder order, [CanBeNull] IViewManager viewManager)
        {
            foreach(var o in PluginContext.Operations.GetOrders())
            {
                Debug.WriteLine($" #{o.Number}, guests {o.Guests.Count}, sum: {o.ResultSum} FullSum {o.FullSum} OriginName {o.OriginName} Status {o.Status}");
            }
            PluginContext.Log.Info("On before order bill subscription.");
            viewManager?.ChangeProgressBarMessage("Waiting for confirmation...");

            if (MessageBox.Show($"Allow bill operation of the order: #{order.Number}, guests {order.Guests.Count}, sum: {order.ResultSum}?{Environment.NewLine}", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
                != MessageBoxResult.Yes)
            {
                PluginContext.Log.Info($"Bill operation of order '{order.Id}' will be canceled.");
                throw new OperationCanceledException();
            }
        }

        public void Dispose()
        {
            try
            {
                subscription.Dispose();
            }
            catch (RemotingException)
            {
                // nothing to do with the lost connection
            }
        }
    }
}
