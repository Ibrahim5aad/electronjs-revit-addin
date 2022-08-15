using ElectronJsRevitAddin.Controllers;
using ElectronJsRevitAddin.DI;

namespace ElectronJsRevitAddin
{
	/// <summary>
	/// Class StartUp.
	/// </summary>
	public class StartUp
	{

		/// <summary>
		/// Configures the IoC service registrations.
		/// </summary>
		public void ConfigureServices()
		{
			IoC.Default
				.RegisterControllers();
		}
	}
}
