using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CookComputing.XmlRpc;
using InfusionSoft.Definition;

namespace InfusionSoft.Custom
{
	public class ServiceBaseCustom<TServiceDefinition> where TServiceDefinition : IServiceDefinition
	{
		private readonly InfusionsoftProxyCustom<TServiceDefinition> _proxy;

		protected ServiceBaseCustom(string url)
		{
			_proxy = new InfusionsoftProxyCustom<TServiceDefinition>(url);
		}

		public IInfusionSoftConfiguration Configuration { get; set; }
		public string ApiKey { get; private set; }

		protected T Invoke<T>(Func<TServiceDefinition, T> method)
		{
			return TryInvoke(() => _proxy.Invoke(method));
		}

		/// <summary>
		///     This should only be used on dynamic return types as it is a lot slower.
		///     Typically only used in the Data Service because you can return any entity from any table.
		/// </summary>
		/// <typeparam name="TDefinitionResponse"></typeparam>
		/// <typeparam name="TResponse"></typeparam>
		/// <param name="method"></param>
		/// <returns></returns>
		protected internal TResponse Invoke<TDefinitionResponse, TResponse>(Func<TServiceDefinition, TDefinitionResponse> method)
		{
			return TryInvoke(() => _proxy.Invoke<TDefinitionResponse, TResponse>(method));
		}

		private static T TryInvoke<T>(Func<T> func)
		{
			try
			{
				return func();
			}
			catch (XmlRpcFaultException e)
			{
				throw new InfusionSoftException(e.Message);
			}
		}

		public IMethodListenerProvider MethodListenerProvider { get; private set; }
	}
}
