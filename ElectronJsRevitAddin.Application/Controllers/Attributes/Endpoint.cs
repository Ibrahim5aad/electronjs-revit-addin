using System;

namespace ElectronJsRevitAddin.Controllers.Attributes
{
	public class Endpoint : Attribute
	{
		public Endpoint(string path)
		{
			Path = path;
		}
		public string Path { get; set; }
	}
}
