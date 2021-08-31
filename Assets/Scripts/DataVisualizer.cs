using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataVisualizer : MonoBehaviour
{
    [SerializeField]
    private GameObject Sphere;
    [SerializeField]
    private List<GameObject> ObjectPool;

    private RequestController controller;
    private Vector3 initPos;
    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<RequestController>();
        initPos = Camera.main.transform.position;
        var listOfPoints = controller.Points.Features;

        //init the object pool by the points number
        ObjectPool = new List<GameObject>(listOfPoints.Count);
        IncrementPool(listOfPoints.Count);
        VisualizeDataPoints();
    }
    // Update is called once per frame
    void Update()
    {
        //monitor camera position
        Vector3 cameraPos = Camera.main.transform.position;
        //we don't account Y direction, since it doesn't influence the distance in X-Z 2D plane
        Vector2 vectorPos = new Vector2(cameraPos.x-initPos.x,cameraPos.z-initPos.z);
        if (vectorPos.magnitude >= 300)
        {
            initPos = cameraPos;
            controller.InitX = controller.InitX+vectorPos.x;
            controller.InitY = controller.InitY+vectorPos.y;
            Debug.Log($"InitPos updated: {controller.InitX}, {controller.InitY}");
            controller.CallForRequest();
            VisualizeDataPoints();
        }
    }
    void VisualizeDataPoints()
    {
        
        var listOfPoints = controller.computeRelativePos();
        double maxX = listOfPoints[0].Point.X + initPos.x, maxZ = listOfPoints[0].Point.Y + initPos.z, minX = listOfPoints[0].Point.X + initPos.x, minZ = listOfPoints[0].Point.Y + initPos.z;
        if (listOfPoints.Count > ObjectPool.Count)
        {
            IncrementPool(listOfPoints.Count - ObjectPool.Count);
        }
        //visualize data
        for(int i = 0; i < listOfPoints.Count; i++)
        {
            ObjectPool[i].SetActive(true);
            float newX = (float)listOfPoints[i].Point.X + initPos.x;
            float newZ = (float)listOfPoints[i].Point.Y + initPos.z;
            ObjectPool[i].GetComponent<Transform>().position = new Vector3(newX, 0.0f, newZ);
            if (newX > maxX) { maxX = newX; }
            if (newX < minX) { minX = newX; }
            if (newZ > maxZ) { maxZ = newZ; }
            if (newZ < minZ) { minZ = newZ; }
        }
        for(int j = listOfPoints.Count; j < ObjectPool.Count; j++)
        {
            ObjectPool[j].SetActive(false);
        }
        Debug.Log($"sphere No: {listOfPoints.Count}; Max_X: {maxX}; Max_Z: {maxZ}; Min_X: {minX}; Min_Z: {minZ};\n Range_X: {maxX-minX}; Range_Z: {maxZ-minZ}");
    }
    void IncrementPool(int n)
    {
        for(int i = 0; i < n; i++)
        {
            var sphereObj = Instantiate(Sphere, Vector3.zero, Quaternion.identity);
            sphereObj.SetActive(false);
            ObjectPool.Add(sphereObj);
        }
    }
   
}
