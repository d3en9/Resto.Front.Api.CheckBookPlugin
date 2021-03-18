using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Resto.Front.Api.Data.Cheques;
using Resto.Front.Api.Extensions;
using Resto.Front.Api.Attributes.JetBrains;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Net.Http;

namespace Resto.Front.Api.CheckBookPlugin
{
    /// <summary>
    /// Обрабатываем печать пречека.
    /// </summary>
    public class BillChequePrintingExtender : IDisposable
    {
        [NotNull]
        private readonly IDisposable subscription;
        

        public BillChequePrintingExtender()
        {
            subscription = PluginContext.Notifications.SubscribeOnBillChequePrinting(OnBillChequePrinting);

        }

        /// <summary>
        /// callback печати пречека.
        /// </summary>        
        [NotNull]
        private BillCheque OnBillChequePrinting(Guid orderId)
        {
            PluginContext.Log.Info("Printing precheck");
            var order = PluginContext.Operations.GetOrderById(orderId);
            var json = OrderHelper.GetOrderPackageJson(order);
            PluginContext.Log.Info(json);
            HttpSender.AddTask(new HttpSenderTask { Url = HttpSender.PostChequeUrl, Json = json, Method = HttpMethod.Post });
            return new BillCheque();
        }

        
        public void Dispose()
        {
            subscription.Dispose();
        }
    }
}
