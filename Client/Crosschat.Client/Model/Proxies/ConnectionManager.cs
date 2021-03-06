﻿using System;
using System.Threading;
using System.Threading.Tasks;
using SharedSquawk.Client.Model.Contracts;
using SharedSquawk.Server.Application.DataTransferObjects.Messages;
using SharedSquawk.Server.Application.DataTransferObjects.Requests;
using SharedSquawk.Server.Application.DataTransferObjects.Utils;
using SharedSquawk.Server.Infrastructure.Protocol;
using SharedSquawk.Utils.Tasking;
using System.Net;

namespace SharedSquawk.Client.Model.Proxies
{
	public class ConnectionManager
    {
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);
        private readonly ITransportResource _transport;
        private readonly RequestsHandler _requestsHandler;
        private long _lastToken = 1;

        public ConnectionManager(
            ITransportResource transport,
            RequestsHandler requestsHandler)
        {
            _transport = transport;
            _requestsHandler = requestsHandler;

			_transport.DataReceived += DataReceived;

		}

		private void DataReceived(ResponseBase response)
		{
			_requestsHandler.AppendResponse (response);
		}

		public event Action ConnectionDropped = delegate { }; 

		internal Task<TResponse> SendRequestAndWaitResponse<TRequest, TResponse>(TRequest request)
			where TRequest : RequestBase
			where TResponse : ResponseBase
		{

			var endpoint = EndpointFinder.Get (request.GetType ());

			Task<TResponse> response = null;
			const int maxTries = 3;

			//We'll allow the client to try a few times to get a connection.
			for(int i = 0; i < maxTries; i++)
			{
				Interlocked.Increment(ref _lastToken);
				request.Token = _lastToken;
				try
				{
					response = _requestsHandler.WaitForResponse<TResponse>(request, () => _transport.SendData<TRequest, TResponse>(endpoint, request, request.Token));
				}
				catch(Exception)
				{
				}

				if (response.Result != null)
				{
					break;
				}
			}

			if (response == null || response.Result == null)
			{
				ConnectionDropped ();
				#if DEBUG
				System.Diagnostics.Debug.WriteLine("Response failed, connection dropped.");
				#endif
			}
			return response;
        }
    }
}
