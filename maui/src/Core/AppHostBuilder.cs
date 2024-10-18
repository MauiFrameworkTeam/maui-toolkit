using Microsoft.Maui;
#if __IOS__ 
using Microsoft.Maui.Controls.Compatibility.Hosting;
#endif
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
using Syncfusion.Maui.Toolkit;
using Syncfusion.Maui.Toolkit.Carousel;
using Syncfusion.Maui.Toolkit.Internals;
using Syncfusion.Maui.Toolkit.Graphics.Internals;
#if WINDOWS
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Dispatching;
#endif

namespace Syncfusion.Maui.Toolkit.Hosting
{
    /// <summary>
    /// Represents application host extension, that used to configure handlers defined in Syncfusion maui core.
    /// </summary>
    public static class AppHostBuilderExtensions
    {
        /// <summary>
        /// Configures the implemented handlers in Syncfusion.Maui.Toolkit.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static MauiAppBuilder ConfigureSyncfusionToolkit(this MauiAppBuilder builder)
        {
#if __IOS__ 
            builder.UseMauiCompatibility();
#endif
            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler(typeof(IDrawableView), typeof(SfDrawableViewHandler));
                handlers.AddHandler(typeof(IDrawableLayout), typeof(SfViewHandler));
                handlers.AddHandler(typeof(ICarousel), typeof(CarouselHandler));
            });

#if WINDOWS
            builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IMauiInitializeService, MauiControlsInitializer>());
#endif

            return builder;
        }

#if WINDOWS
        class MauiControlsInitializer : IMauiInitializeService
        {
            public void Initialize(IServiceProvider services)
            {
               var dispatcher = services.GetRequiredService<IDispatcher>();
				if (dispatcher.IsDispatchRequired)
					dispatcher.Dispatch(() => SetupResources());
				else
					SetupResources();

				static void SetupResources()
				{
					if (Microsoft.UI.Xaml.Application.Current?.Resources is not Microsoft.UI.Xaml.ResourceDictionary resources)
						return;

					AppHostBuilderExtensions.AddLibraryResources(resources, "SyncfusionResourcesIncluded", "ms-appx:///Syncfusion.Maui.Toolkit/Carousel/Platform/Windows/Styles/Resources.xaml");

				}
            }
        }   

        /// <summary>
        /// Method to add the 
        /// </summary>
        internal static void AddLibraryResources(this Microsoft.UI.Xaml.ResourceDictionary? resources, string key, string uri)
        {
            if (resources == null)
                return;

            var dictionaries = resources.MergedDictionaries;
            if (dictionaries == null)
                return;

            if (!resources.ContainsKey(key))
            {
                dictionaries.Add(new Microsoft.UI.Xaml.ResourceDictionary
                {
                    Source = new Uri(uri)
                });
            }
        }
#endif
    }
}
