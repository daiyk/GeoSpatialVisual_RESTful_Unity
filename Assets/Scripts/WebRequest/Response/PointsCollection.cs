namespace Vlab.WebRequest.Response
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
 

    public class PointGeometry
    {
        public PointGeometry() { }
        public PointGeometry(double x, double y) { X=x;Y = y; }
        [JsonPropertyName("x")]
        public double X { get; set; }
        [JsonPropertyName("y")]
        public double Y { get; set; }
    }
    public class Attributes
    {
        [JsonPropertyName("NO_PLAN")]
        public string NumOfPlanes { get; set; }
    }
    public class Points
    {
        public Points() { }
        public Points(double x, double y) { Point = new PointGeometry(x,y); }
        //[JsonPropertyName("attributes")]
        //public Attributes Attribute { get; set; }
        [JsonPropertyName("geometry")]
        public PointGeometry Point { get; set; }
    }
    public class FieldAliases
    {
        public string No_PLAN { get; set; }
    }
    public class SpatialReference
    {
        public int wkid { get; set; }
        public int latestWkid { get; set; }
    }
    public class FieldValues
    {
        public string name { get; set; }
        public string type { get; set; }
        public string alias { get; set; }
        public int length { get; set; }
    }
    public class PointsCollection
    {

        //public FieldAliases fieldAliases { get; set; }
        //public string displayFieldName { get; set; }
        //public SpatialReference spatialReference { get; set; }
        //public string geometryType { get; set; }
        //public List<FieldValues> fields { get; set; }

        [JsonPropertyName("features")]
        public List<Points> Features { get; set; }
    }
}