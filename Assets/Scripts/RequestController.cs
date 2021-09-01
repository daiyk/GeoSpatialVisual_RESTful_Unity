using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RestSharp;
using Vlab.WebRequest;
using Vlab.WebRequest.Response;
using System;

/// <summary>
/// A central manager class that stores the init conditions and process REST request and response:
/// <list type="bullet">
/// <item><description>Send Request and receive response</description></item>
/// <item><description>Process the response and deserialize it into c# object</description></item>
/// <item><description>Simple post processing, e.g. compute the relative pos to the init position</description></item>
/// </list>
/// </summary>
public class RequestController : MonoBehaviour
{
    /// <summary>
    /// Below are init parameters provided in the project requirements
    /// </summary>
    [Header("RESTful request info")]
    [SerializeField]
    private string BaseUrl = "";
    [SerializeField]
    private string ResourceUrl = "";
    [SerializeField]
    private int LayerID;
    [Space(10)]

    [Header("Init player position")]
    [SerializeField]
    private double x;
    [SerializeField]
    private double y;
    [Space(10)]

    [Header("Others")]
    [SerializeField]
    private float SquareLength;

    /// <value> Property <c>InitX</c> is the user's init x coordinate when program start
    /// and will be updated with user's current position if user leave 300m away</value>
    public double InitX { get { return x; } set { x = value; } }

    /// <value> Property <c>InitX</c> is the user's init x coordinate when program start
    /// and will be updated with user's current position if user leave 300m away</value>
    public double InitY { get { return y; } set { y = value; } }

    /// <value> Property <c>Points</c> is the RESTfull result that include data points
    /// and other description information
    /// </value>
    public PointsCollection Points
    {
        get { return points; }
    }

    /// <summary>Instance <c>restClient</c> is a RestfulRequest object which is wrapper for RestSharp apis</summary>
    private RestfulRequest restClient;


    /// <summary> Instance <c>request</c> is a request object which will be sent to ArcGIS RESTful server</summary>
    private RestRequest request;

    /// <summary> Instance <c>restClient</c> is a RestfulRequest object which is wrapper for RestSharp apis</summary>
    private PointsCollection points;

    /// <summary>
    /// Awake is called before Start, it build the RestfulRequest client and start the first request
    /// for initial user position
    /// </summary>
    void Awake()
    {
        if (BaseUrl != "" && ResourceUrl != "")
        {
            restClient = new RestfulRequest(BaseUrl);
            
            string ResourceToLayer = ResourceUrl + $"/{LayerID}";
            request = RestfulRequest.CreateGetRequest(ResourceToLayer);
        }
        CallForRequest();
    }

    /// <summary>
    /// Compute the range of sqaure dimension for a square, 
    /// given the center point and square length
    /// </summary>
    /// <param name="x">square center point x coordinate (m)</param>
    /// <param name="y">square center point y coordinate (m)</param>
    /// <param name="squareLength">sqaure length (m)</param>
    /// <returns>a length-4 list contains the x and y-coordinate range</returns>
    private List<double> computeRange(double x, double y, float squareLength)
    {
        List<double> geoParameters = new List<double>();
        geoParameters.Add(x - squareLength / 2.0f);// xmin
        geoParameters.Add(y - squareLength / 2.0f);// ymin
        geoParameters.Add(x + squareLength / 2.0f);// xmax
        geoParameters.Add(y + squareLength / 2.0f);// ymax
        return geoParameters;
    }
    /// <summary>
    /// This method append parameters to the rest request first, 
    /// then start the request and store the deseralized response object 
    /// </summary>
    public void CallForRequest()
    {
        var listOfGeo = computeRange(x, y, SquareLength);
        RestfulRequest.AddParameter(request, "geometryType", "esriGeometryEnvelope");
        RestfulRequest.AddParameter(request, "geometry", $"{listOfGeo[0].ToString()},{listOfGeo[1].ToString()},{listOfGeo[2].ToString()},{listOfGeo[3].ToString()}");
        RestfulRequest.AddParameter(request, "f", "json");
        Debug.Log($"Build Full uri path: {restClient.GetFullURI(request)}");
        points = restClient.Execute<PointsCollection>(request);
    }
    
    /// <summary>
    /// This method computes the retrieved data points relative position, 
    /// to the user's initial(or current) position
    /// </summary>
    /// <returns>list of data points</returns>
    public List<Points> computeRelativePos()
    {
        var listOfPoints = points.Features;
        List<Points> relativePts = new List<Points>(listOfPoints.Count);

        for(int i = 0; i < listOfPoints.Count; i++)
        {
            relativePts.Add(new Points(listOfPoints[i].Point.X-x,listOfPoints[i].Point.Y-y));
        }
        return relativePts;
    }
}