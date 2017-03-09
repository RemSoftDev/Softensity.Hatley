using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CookComputing.XmlRpc;
using InfusionSoft.Definition;
using InfusionSoft.Tables;

namespace InfusionSoft.Custom
{
	public class InfusionsoftProxyCustom<TService> where TService : IServiceDefinition
	{
		private readonly Uri _uri;

		public InfusionsoftProxyCustom(string url)
		{
			_uri = new Uri(url);
		}

		public TResponse Invoke<TResponse>(Func<TService, TResponse> method)
		{
			using (ProxyManager<TService> manager = CreateProxyManager())
			{
				return method(manager);
			}
		}

		public TResponse Invoke<TDefinitionResponse, TResponse>(Func<TService, TDefinitionResponse> method)
		{
			TResponse response = default(TResponse);

			XmlRpcResponseEventHandler responseHandler = (sender, args) =>
			{
				var deserializer = new XmlRpcResponseDeserializer();
				XmlRpcResponse res = deserializer.DeserializeResponse(args.ResponseStream, typeof(TResponse));
				response = (TResponse)res.retVal;
			};

			using (ProxyManager<TService> manager = CreateProxyManager(responseHandler))
			{
				method(manager);
			}

			return response;
		}

		private ProxyManager<TService> CreateProxyManager(XmlRpcResponseEventHandler responseHandler = null)
		{
			var definition = XmlRpcProxyGen.Create<TService>();

			// ReSharper disable PossibleInvalidCastException
			var proxy = (IXmlRpcProxy)definition;
			// ReSharper restore PossibleInvalidCastException

			//proxy.AttachLogger(new MethodListenerXmlRpcLogger(_listenerProvider.GetListener()));

			proxy.Url = _uri.AbsoluteUri;
			//proxy.UserAgent = UserAgent;

			return new ProxyManager<TService>(definition, responseHandler);
		}

		private class ProxyManager<T> : IDisposable
		{
			private readonly IXmlRpcProxy _proxy;
			private readonly XmlRpcResponseEventHandler _responseHandler;

			public ProxyManager(T proxy, XmlRpcResponseEventHandler responseHandler)
			{
				_proxy = (IXmlRpcProxy)proxy;
				_responseHandler = responseHandler;

				if (_responseHandler != null)
					_proxy.ResponseEvent += _responseHandler;
			}

			#region IDisposable Members

			public void Dispose()
			{
				if (_responseHandler != null)
					_proxy.ResponseEvent -= _responseHandler;
			}

			#endregion

			public static implicit operator T(ProxyManager<T> wrapper)
			{
				return (T)wrapper._proxy;
			}
		}
	}
}
