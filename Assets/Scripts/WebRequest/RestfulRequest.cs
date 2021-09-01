namespace Vlab.WebRequest
{
    using UnityEngine;
    using RestSharp;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// RestfulRequest is a wrapper for RestSharp API and provide relative functions 
    /// specific for ArcGIS MapServer RESTful Query operation 
    /// </summary>
    public class RestfulRequest
    {
        /// <summary>
        /// RestSharp instance variable <c>clients</c> provide interface to RestSharp APIs
        /// </summary>
        private RestClient client;

        /// <summary>
        /// Default constructor for RestfulRequest
        /// </summary>
        public RestfulRequest() { }

        /// <summary>
        /// Construct a new ResrfulRequest object with the given base uri, which is not 
        /// necessary the full uri path
        /// </summary>
        /// <param name="baseURL">base uri address</param>
        public RestfulRequest(string baseURL)
        {
            CreateClient(baseURL);
        }

        /// <summary>
        /// create RestSharp client if not done, e.g. when use default constructor
        /// </summary>
        /// <param name="baseURL">base uri address</param>
        public void CreateClient(string baseURL)
        {
            if (client == null)
            {
                client = new RestClient(baseURL);
                RestSharp.Serializers.SystemTextJson.RestClientExtensions.UseSystemTextJson(client);
            }
        }

        /// <summary>
        /// Execute https request with the given request object to target RESTful server
        /// </summary>
        /// <param name="request">a RestSharp request object to be executed</param>
        /// <returns>a IRestReponse object includes raw response data</returns>
        public IRestResponse Execute(RestRequest request)
        {
            if (client == null)
            {
                Debug.Log($"{this.GetType().Namespace}: Request Client is null.");
                return null;
            }
            var response = client.Execute(request);
            //NOTE: ReshSharp doesn't throw exception, instead it stores the exception in its property
            //we need to manually check it
            if (response.ErrorException != null)
            {
                var requestException = new Exception($"{this.GetType().Namespace}: Exception happens in request execute.", response.ErrorException);
                throw requestException;
            }
            return response;
        }

        /// <summary>
        /// Generic function that execute https request with the given request object to target RESTful
        /// server and seralize the response to object with type T 
        /// </summary>
        /// <typeparam name="T"> generic type that its structure must corespond to the response's data format</typeparam>
        /// <param name="request">a RestSharp request object to be executed</param>
        /// <returns>a object with generic type T</returns>
        public T Execute<T>(RestRequest request) where T: new()
        {
            if (client == null)
            {
                Debug.Log($"{this.GetType().Namespace}: Request Client is null.");
                return default(T);
            }
            var response = client.Execute<T>(request);
            //NOTE: ReshSharp doesn't throw exception, instead it stores the exception in its property
            //we need to manually check it
            if (response.ErrorException != null)
            {
                var requestException = new Exception($"{this.GetType().Namespace}: Exception happens in request execute.", response.ErrorException);
                throw requestException;
            }
            return response.Data;

        }

        /// <summary>
        /// async version api that execute https request with the given request object to target RESTful
        /// and deseralize the response to object with type T 
        /// </summary>
        /// <typeparam name="T">generic type that its structure must corespond to the response's data format</typeparam>
        /// <param name="request">a RestSharp request object to be executed</param>
        /// <returns>asynchronous result of a object with generic type T</returns>
        public async Task<T> ExecuteAsync<T>(RestRequest request) where T : new()
        {
            if (client == null)
            {
                Debug.Log($"{this.GetType().Namespace}: Request Client is null.");
                return default(T);
            }
            var response = await client.ExecuteAsync<T>(request);
            //NOTE: ReshSharp doesn't throw exception, instead it stores the exception in its property
            //we need to manually check it
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                var requestException = new Exception($"{this.GetType().Namespace}: Exception happens in request execute.", response.ErrorException);
                throw requestException;
            }
            return response.Data;

        }

        /// <summary>
        /// create a RestSharp request object that execute QUERY operation specific 
        /// for ArcGIS MapServer RESTful apis
        /// </summary>
        /// <param name="resource">a resource address that combining with RestSharp 
        /// client base uri can form a complete endpoint </param>
        /// <returns> a RestRequest object</returns>
        public static RestRequest CreateGetRequest(string resource)
        {
            string queryRequestResource = resource + $"/query";
            return new RestRequest(queryRequestResource, RestSharp.Method.GET, DataFormat.Json);
        }

        /// <summary>
        /// Append optional parameter to the given RestSharp request, if the 
        /// parameter already exist, update the value. The result will be uri?name=value&name=...
        /// </summary>
        /// <param name="request">the request that the parameter will be appended to</param>
        /// <param name="name">the name of the parameter</param>
        /// <param name="value">the value of the parameter</param>
        public static void AddParameter(RestRequest request, string name, object value)
        {
            request.AddOrUpdateParameter(name, value);
        }

        /// <summary>
        /// Combine base uri in RestSharp client and request resource location
        /// that formulate a complete endpoint
        /// </summary>
        /// <param name="request">a fullly functional uri(endpoint)</param>
        /// <returns></returns>
        public Uri GetFullURI(RestRequest request)
        {
            return client.BuildUri(request);
        }
    }
}
