using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using Resto.Front.Api.Attributes;
using Resto.Front.Api.Attributes.JetBrains;
using System.Configuration;

namespace Resto.Front.Api.CheckBookPlugin
{
    /// <summary>
    /// Тестовый плагин для демонстрации возможностей Api.
    /// Автоматически не публикуется, для использования скопировать Resto.Front.Api.SamplePlugin.dll в Resto.Front.Main\bin\Debug\Plugins\Resto.Front.Api.SamplePlugin\
    /// </summary>
    [UsedImplicitly]
    [PluginLicenseModuleId(21015808)]
    //[PluginLicenseModuleId(21005108)]
    public sealed class CheckBookPlugin : IFrontPlugin
    {
        private readonly Stack<IDisposable> subscriptions = new Stack<IDisposable>();

        public CheckBookPlugin()
        {
            PluginContext.Log.Info("Initializing CheckBookPlugin");
            var classesstr = ConfigurationManager.AppSettings["ListOfPluginClassForLoading"];
            var classesNames = classesstr.Split(',').Select(_ => _.Trim());
            var httpSender = new HttpSender();
            subscriptions.Push(httpSender);
            httpSender.Run();
            foreach (var className in classesNames)
            {
                subscriptions.Push(Activator.CreateInstance("Resto.Front.Api.CheckBookPlugin", className) as IDisposable);
                PluginContext.Log.Info($"class {className} was added to subsribe");
            }
            PluginContext.Log.Info("CheckBookPlugin started");
        }

        public void Dispose()
        {
            while (subscriptions.Any())
            {
                var subscription = subscriptions.Pop();
                try
                {
                    if (subscription != null)
                        subscription.Dispose();
                }
                catch (RemotingException re)
                {
                    // nothing to do with the lost connection
                    PluginContext.Log.Error(re.ToString());
                }
                catch (Exception e)
                {
                    // nothing to do with the lost connection
                    PluginContext.Log.Error(e.ToString());
                }
            }

            PluginContext.Log.Info("SamplePlugin stopped");
        }
        
    }
}
