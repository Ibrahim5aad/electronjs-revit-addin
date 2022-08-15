using System;
using System.Collections.Generic;
using System.Linq;


namespace ElectronJsRevitAddin.DI
{
	/// <summary>
	/// An implementation of an inversion of control (IoC) container.
	/// </summary>
	public class IoC : IDisposable
	{

		#region Fields

		private readonly HashSet<ServiceDescriptor> _serviceDescriptors
				= new HashSet<ServiceDescriptor>(new ServiceDescriptorComparer());
		private static IoC _defaultInstance;
		private static object _lockObject = new object();

		#endregion

		#region Singleton Instance

		/// <summary>
		/// Intializes a new instance of the IoC container,
		/// </summary>
		private IoC()
		{
		}


		/// <summary>
		/// The default IoC container.
		/// </summary>
		public static IoC Default
		{
			get
			{
				lock (_lockObject)
				{
					if (_defaultInstance == null)
						_defaultInstance = new IoC();
					return _defaultInstance;
				}
			}
			private set => _defaultInstance = value;
		}

		#endregion

		#region Registering Services


		/// <summary>
		/// Register a servic of type TService as a singleton.
		/// </summary>
		/// <typeparam name="TService"> Type of service. </typeparam>
		/// <returns></returns>
		public IoC RegisterSingleton<TService>() where TService : class
		{
			_serviceDescriptors.Add(new ServiceDescriptor(typeof(TService), ServiceLifetime.Singleton));
			return this;
		}


		/// <summary>
		/// Register a servic of type TService and Implementation of 
		/// type TImplementation as a singleton.
		/// </summary>
		/// <typeparam name="TService"> Type of service. </typeparam>
		/// <typeparam name="TImplementation"> Type of implementation. </typeparam>
		/// <returns></returns>
		public IoC RegisterSingleton<TService, TImplementation>() where TImplementation : class, TService
		{
			_serviceDescriptors.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton));
			return this;
		}



		/// <summary>
		/// Register a servic of type TService as a singleton.
		/// </summary>
		/// <typeparam name="TService"> Type of service. </typeparam>
		/// <returns></returns>
		public IoC RegisterSingleton<TService>(TService service) where TService : class
		{
			_serviceDescriptors.Add(new ServiceDescriptor(service, ServiceLifetime.Singleton)
			{
				ServiceName = service.GetType().Name,
			});
			return this;
		}


		/// <summary>
		/// Registers the singleton.
		/// </summary>
		/// <typeparam name="TService">The type of the service.</typeparam>
		/// <param name="service">The service.</param>
		/// <param name="serviceName">Name of the service.</param>
		/// <returns></returns>
		public IoC RegisterSingleton<TService>(TService service, string serviceName) where TService : class
		{
			_serviceDescriptors.Add(new ServiceDescriptor(service, ServiceLifetime.Singleton)
			{
				ServiceName = serviceName.ToLower(),
			});
			return this;
		}


		/// <summary>
		/// Register a servic of type TService and Implementation of 
		/// type TImplementation as a singleton.
		/// </summary>
		/// <typeparam name="TService"> Type of service. </typeparam>
		/// <typeparam name="TImplementation"> Type of implementation. </typeparam>
		/// <returns></returns>
		public IoC RegisterSingleton<TService, TImplementation>(TService service, TImplementation implementation) where TImplementation : class, TService
		{
			_serviceDescriptors.Add(new ServiceDescriptor(service as Type, implementation as Type, ServiceLifetime.Singleton)
			{
				ServiceName = service.GetType().Name,
			});
			return this;
		}


		/// <summary>
		/// Register a servic of type TService as a transient.
		/// </summary>
		/// <typeparam name="TService"> Type of service. </typeparam>
		/// <returns></returns>
		public IoC RegisterTransient<TService>()
		{
			_serviceDescriptors.Add(new ServiceDescriptor(typeof(TService), ServiceLifetime.Transient));
			return this;
		}


		/// <summary>
		/// Register a servic of type TService and Implementation of 
		/// type TImplementation as a transient.
		/// </summary>
		/// <typeparam name="TService"> Type of service. </typeparam>
		/// <typeparam name="TImplementation"> Type of implementation. </typeparam>
		/// <returns></returns>
		public IoC RegisterTransient<TService, TImplementation>() where TImplementation : class, TService
		{
			var s = _serviceDescriptors.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Transient));
			return this;
		}


		/// <summary>
		/// Register a servic of type TService and Implementation of
		/// type TImplementation as a transient.
		/// </summary>
		/// <typeparam name="TService">Type of service.</typeparam>
		/// <param name="service">The service.</param>
		/// <returns>IoC.</returns>
		public IoC RegisterTransient<TService>(TService service)
		{
			_serviceDescriptors.Add(new ServiceDescriptor(service as Type, ServiceLifetime.Transient));
			return this;
		}


		/// <summary>
		/// Register a servic of type TService and Implementation of
		/// type TImplementation as a transient.
		/// </summary>
		/// <typeparam name="TService">Type of service.</typeparam>
		/// <typeparam name="TImplementation">Type of implementation.</typeparam>
		/// <param name="service">The service.</param>
		/// <param name="implementation">The implementation.</param>
		/// <returns>IoC.</returns>
		public IoC RegisterTransient<TService, TImplementation>(TService service, TImplementation implementation) where TImplementation : class, TService
		{
			var s = _serviceDescriptors.Add(new ServiceDescriptor(service as Type, implementation as Type, ServiceLifetime.Transient));
			return this;
		}

		#endregion

		#region Resolving Services


		/// <summary>
		/// Resolves the required service of type 'serviceType'
		/// </summary>
		/// <param name="serviceType"> The type of service. </param>
		/// <returns> The implementation of the required service. </returns>
		public object GetService(Type serviceType)
		{
			var desciptor = _serviceDescriptors
				.SingleOrDefault(x => x.ServiceType == serviceType);

			if (desciptor == null)
				throw new Exception($"Service of type {serviceType.Name} isn't registered");


			//return the provided implementation, if it's not null means the service
			//is registered as a singleton and has been resolved before.
			if (desciptor.Implementation != null)
				return desciptor.Implementation;


			//get the actual type of the implementation, if the property ImplementationType
			//is null means that service is registered specifying the implementation type
			//only which then can be found in the ServiceType property.
			var actualType = desciptor.ImplementationType ?? desciptor.ServiceType;


			//if the provided implementation can not be instanciated throw an exception.
			if (actualType.IsAbstract || actualType.IsInterface)
				throw new Exception("Cannot instantiate abstract classes or interfaces");

			object implementation = null;
			Exception _e = null;
			foreach (var constructorInfo in actualType.GetConstructors())
			{
				try
				{
					var parameters = constructorInfo.GetParameters()
									  .Select(x => GetService(x.ParameterType)).ToArray();

					implementation = Activator.CreateInstance(actualType, parameters);

					if (desciptor.Lifetime == ServiceLifetime.Singleton)
						desciptor.Implementation = implementation;

					if (implementation != null)
						break;
				}
				catch (Exception e)
				{
					_e = e;
				}
			}

			if (implementation == null)
				throw new Exception("Cannot resolve the required service. A problem occured when resolving one or more of its dependencies.", _e);

			return implementation;
		}


		/// <summary>
		/// Gets the service.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="serviceName">Name of the service.</param>
		/// <returns></returns>
		public object GetService(string serviceName)
		{
			var desciptor = _serviceDescriptors
				.SingleOrDefault(x => x.ServiceName == serviceName);


			if (desciptor == null)
				throw new Exception($"Service of type {serviceName} isn't registered");


			//return the provided implementation, if it's not null means the service
			//is registered as a singleton and has been resolved before.
			if (desciptor.Implementation != null)
				return desciptor.Implementation;


			//get the actual type of the implementation, if the property ImplementationType
			//is null means that service is registered specifying the implementation type
			//only which then can be found in the ServiceType property.
			var actualType = desciptor.ImplementationType ?? desciptor.ServiceType;


			//if the provided implementation can not be instanciated throw an exception.
			if (actualType.IsAbstract || actualType.IsInterface)
				throw new Exception("Cannot instantiate abstract classes or interfaces");

			object implementation = null;
			Exception _e = null;
			foreach (var constructorInfo in actualType.GetConstructors())
			{
				try
				{
					var parameters = constructorInfo.GetParameters()
									  .Select(x => GetService(x.ParameterType)).ToArray();

					implementation = Activator.CreateInstance(actualType, parameters);

					if (desciptor.Lifetime == ServiceLifetime.Singleton)
						desciptor.Implementation = implementation;

					if (implementation != null)
						break;
				}
				catch (Exception e)
				{
					_e = e;
				}
			}

			if (implementation == null)
				throw new Exception("Cannot resolve the required service. A problem occured when resolving one or more of its dependencies.", _e);

			return implementation;
		}


		/// <summary>
		/// Resolves the required service of type T.
		/// </summary> 
		/// <typeparam name="T"> The type of service. </typeparam>
		/// <returns> The implementation of the required service. </returns> 
		public T GetService<T>()
		{
			return (T)GetService(typeof(T));
		}


		/// <summary>
		/// Unregisters the specified service of type T.
		/// </summary>
		/// <typeparam name="TService"> Type of the service.</typeparam> 
		public void Unregister<TService>()
		{
			var service = _serviceDescriptors.FirstOrDefault(s => s.ServiceType == typeof(TService));
			if (service != null)
				_serviceDescriptors.Remove(service);
		}

		/// <summary>
		/// Unregisters the specified service of type T.
		/// </summary>
		/// <typeparam name="TService"> Type of the service.</typeparam> 
		public void Unregister<TService>(TService service)
		{
			var rservice = _serviceDescriptors.FirstOrDefault(s => s.ServiceType == service as Type);
			if (rservice != null)
				_serviceDescriptors.Remove(rservice);
		}


		/// <summary>
		/// Performs application-defined tasks associated with freeing, 
		/// releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			_serviceDescriptors.Clear();
			Default = null;
		}


		#endregion

		#region Internal Types

		/// <summary>
		/// Class ServiceDescriptor works as key or descriptor for 
		/// registered services to be able to retrieve backe the 
		/// registered types from the service collection.
		/// </summary>
		class ServiceDescriptor
		{
			internal string ServiceName { get; set; }

			internal Type ServiceType { get; }

			internal Type ImplementationType { get; }

			internal object Implementation { get; set; }

			internal ServiceLifetime Lifetime { get; }

			internal ServiceDescriptor(object implementation, ServiceLifetime lifetime)
			{
				ServiceType = implementation.GetType();
				Implementation = implementation;
				Lifetime = lifetime;
			}

			internal ServiceDescriptor(Type serviceType, ServiceLifetime lifetime)
			{
				ServiceType = serviceType;
				Lifetime = lifetime;
			}

			internal ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
			{
				ServiceType = serviceType;
				Lifetime = lifetime;
				ImplementationType = implementationType;
			}
		}


		/// <summary>
		/// Class ServiceDescriptorComprer.
		/// Implements the <see cref="System.Collections.Generic.IEqualityComparer`1" />
		/// </summary>
		/// <seealso cref="System.Collections.Generic.IEqualityComparer`1" />
		class ServiceDescriptorComparer : IEqualityComparer<ServiceDescriptor>
		{

			/// <summary>
			/// Equalses the specified x.
			/// </summary>
			/// <param name="x">The x.</param>
			/// <param name="y">The y.</param>
			/// <returns>bool.</returns>
			public bool Equals(ServiceDescriptor x, ServiceDescriptor y)
			{
				return x.ImplementationType == y.ImplementationType &&
					  x.ServiceType == y.ServiceType &&
					  x.Lifetime == y.Lifetime;
			}

			/// <summary>
			/// Gets the hash code that is used to locate elements in hash tables.
			/// </summary>
			/// <param name="x">The object.</param>
			/// <returns>the hashcode</returns>
			public int GetHashCode(ServiceDescriptor x)
			{
				return x.ImplementationType?.GetHashCode() ?? 0
					  + x.ServiceType?.GetHashCode() ?? 0
					  + x.Lifetime.GetHashCode();
			}
		}

		/// <summary>
		/// Service lifetime enumeration.
		/// </summary>
		enum ServiceLifetime
		{
			Singleton,
			Transient,
			Scoped
		}

		#endregion

	}
}
