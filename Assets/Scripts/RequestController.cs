using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RestSharp;
using Vlab.WebRequest;
using Vlab.WebRequest.Response;
using System;

public class RequestController : MonoBehaviour
{
    // Start is called before the first frame update
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

    public double InitX { get { return x; } set { x = value; } }
    public double InitY { get { return y; } set { y = value; } }
    public PointsCollection Points
    {
        get { return points; }
    }
    private RestfulRequest restClient;
    private RestRequest request;
    private PointsCollection points;
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
    // Update is called once per frame
    void Update()
    {

    }
    private List<double> computeRange(double x, double y, float squareLength)
    {
        List<double> geoParameters = new List<double>();
        geoParameters.Add(x - squareLength / 2.0f);// xmin
        geoParameters.Add(y - squareLength / 2.0f);// ymin
        geoParameters.Add(x + squareLength / 2.0f);// xmax
        geoParameters.Add(y + squareLength / 2.0f);// ymax
        return geoParameters;
    }
    public void CallForRequest()
    {
        var listOfGeo = computeRange(x, y, SquareLength);
        RestfulRequest.AddParameter(request, "geometryType", "esriGeometryEnvelope");
        RestfulRequest.AddParameter(request, "geometry", $"{listOfGeo[0].ToString()},{listOfGeo[1].ToString()},{listOfGeo[2].ToString()},{listOfGeo[3].ToString()}");
        RestfulRequest.AddParameter(request, "f", "json");
        Debug.Log($"Build Full uri path: {restClient.GetFullURI(request)}");
        points = restClient.Execute<PointsCollection>(request);
    }
    
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