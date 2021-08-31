namespace Vlab.WebRequest
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using RestSharp;
    using System;
    using System.Threading.Tasks;

    public class RestfulRequest
    {
        private RestClient client;
        public RestfulRequest() { }
        public RestfulRequest(string baseURL)
        {
            CreateClient(baseURL);
        }
        public void CreateClient(string baseURL)
        {
            if (client == null)
            {
                client = new RestClient(baseURL);
                RestSharp.Serializers.SystemTextJson.RestClientExtensions.UseSystemTextJson(client);
            }
        }
        public IRestResponse Execute(RestRequest request)
        {
            if (client == null)
            {
                Debug.Log($"{this.GetType().Namespace}: Request Client is null.");
                return null;
            }
            var response = client.Execute(request);
            if (response.ErrorException != null)
            {
                var requestException = new Exception($"{this.GetType().Namespace}: Exception happens in request execute.", response.ErrorException);
                throw requestException;
            }
            return response;
        }
        public T Execute<T>(RestRequest request) where T:new()
        {
            if (client == null)
            {
                Debug.Log($"{this.GetType().Namespace}: Request Client is null.");
                return default(T);
            }
            var response = client.Execute<T>(request);
            if (response.ErrorException != null)
            {
                var requestException = new Exception($"{this.GetType().Namespace}: Exception happens in request execute.", response.ErrorException);
                throw requestException;
            }
            return response.Data;

        }
        public async Task<T> ExecuteAsync<T>(RestRequest request) where T : new()
        {
            if (client == null)
            {
                Debug.Log($"{this.GetType().Namespace}: Request Client is null.");
                return default(T);
            }
            var response = await client.ExecuteAsync<T>(request);
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                var requestException = new Exception($"{this.GetType().Namespace}: Exception happens in request execute.", response.ErrorException);
                throw requestException;
            }
            return response.Data;

        }
        public static RestRequest CreateGetRequest(string resource)
        {
            string queryRequestResource = resource + $"/query";
            return new RestRequest(queryRequestResource, RestSharp.Method.GET, DataFormat.Json);
        }
        public static void AddParameter(RestRequest request, string name, object value)
        {
            request.AddOrUpdateParameter(name, value);
        }

        public Uri GetFullURI(RestRequest request)
        {
            return client.BuildUri(request);
        }
    }
}
