using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Resto.Front.Api.Data.Cheques;
using Resto.Front.Api.Extensions;
using Resto.Front.Api.Attributes.JetBrains;
using System.Diagnostics;
using Resto.Front.Api.Data.Orders;
using Resto.Front.Api.UI;
using Newtonsoft.Json;

namespace Resto.Front.Api.CheckBookPlugin
{
    /// <summary>
    /// Обрабатываем печать пречека.
    /// </summary>
    public class BillChequePrintedExtender : IDisposable
    {
        [NotNull]
        private readonly IDisposable subscription;
        

        public BillChequePrintedExtender()
        {
            subscription = PluginContext.Notifications.SubscribeOnServiceChequePrinted(OnServiceChequePrinted);
        }

        /// <summary>
        /// callback печати пречека.
        /// </summary>        
        [NotNull]
        private void OnServiceChequePrinted(IOrder order, IOperationService service, IViewManager manager)
        {
            PluginContext.Log.Info("Printed precheck");
            var json = OrderHelper.GetOrderPackageJson(order);
            HttpSender.AddTask(new HttpSenderTask { Url = HttpSender.PostChequeUrl, Json = json });
            PluginContext.Log.Info(json);
        }

        
        public void Dispose()
        {
            subscription.Dispose();
        }
    }
}
