using ElectronJsRevitAddin.Controllers.Attributes;
using ElectronJsRevitAddin.DI;
using System;
using System.Linq;
using System.Reflection;

namespace ElectronJsRevitAddin.Controllers
{
	internal static class ControllerRegisteration
	{


		/// <summary>
		/// Registers the view models to the services container.
		/// </summary> 
		public static IoC RegisterControllers(this IoC ioc)
		{
			Assembly.GetAssembly(typeof(ControllerBase))
						   .GetTypes()
						   .Where(type => !type.IsInterface && type.GetCustomAttribute(typeof(Controller)) != null)
						   .ToList()
						   .ForEach(vm =>
						   {
							   var name = (vm.GetCustomAttribute(typeof(Controller)) as Controller).Name;
							   ioc.RegisterSingleton(vm.GetConstructor(new Type[0]).Invoke(null), name);
						   });
			return ioc;

		}


		/// <summary>
		/// Unregisters the view models from the services container.
		/// </summary> 
		public static void UnregistrViewModels(this IoC ioc)
		{
			Assembly.GetAssembly(typeof(ControllerBase))
						.GetTypes()
						.Where(type => !type.IsInterface && type.GetCustomAttribute(typeof(Controller)) != null)
						.ToList()
						.ForEach(vm =>
						{
							ioc.Unregister(vm);
						});
		}

	}
}
