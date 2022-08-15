using System;

namespace ElectronJsRevitAddin.Controllers.Attributes
{
	public class Controller : Attribute
	{ 

		/// <summary>
		/// Initializes a new instance of the <see cref="Controller"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		public Controller(string name)
		{
			Name = name;
		}


		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; set; }
	}
}
