using System.Collections;
using System;
using UnityEngine;
using TMPro;

public class LocationFinder : MonoBehaviour
{
    public TMP_Text curlatText;
    public TMP_Text curlongText;
    public TMP_Text curaltText;
    public TMP_Text distText;

    private float savefirstLat;
    private float savefirstLong;
    
    private float savesecondLat;
    private float savesecondLong;

    public GameObject newObj;
    public Camera camera;

    IEnumerator Start()
    {
        if (!Input.location.isEnabledByUser)
            Debug.Log("Location not enabled on device or app does not have permission to access location");

        Input.location.Start();

        int maxWait = 15;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location");
            yield break;
        }
        else
        {
            Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

        curlatText.text = Input.location.lastData.latitude.ToString();
        curlongText.text = Input.location.lastData.longitude.ToString();
        curaltText.text = Input.location.lastData.altitude.ToString();

        Input.location.Stop();
    }

    public void StoreCoordinates()
    {
        savefirstLat = Input.location.lastData.latitude;
        savefirstLong = Input.location.lastData.longitude;
    }

    public void RetrieveCoordinates()
    {
        savesecondLat = Input.location.lastData.latitude;
        savesecondLong = Input.location.lastData.longitude;

        CalculateDistance();
    }

    public void CalculateDistance()
    {
        double latOne = ToRadians(savefirstLat);
        double latTwo = ToRadians(savesecondLat);
        double longOne = ToRadians(savefirstLong);
        double longTwo = ToRadians(savesecondLong);

        double deltaLat = latTwo - latOne;
        double deltaLong = longTwo - longOne;

        double a = Math.Sin(deltaLat / 2 ) * Math.Sin(deltaLat / 2) + Math.Cos(latOne) * Math.Cos(latTwo) * Math.Sin(deltaLong / 2) * Math.Sin(deltaLong / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // Earth's radius in Meters.
        double radius = 6371000;

        double distance = radius * c;

        distText.text = "Distance: " + distance.ToString();
    }

    private double ToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }

    public void InstatiateObject()
    {
        Vector3 spawnPosition = camera.transform.position + camera.transform.forward * 1.0f;

        Instantiate(newObj, spawnPosition, Quaternion.identity);
    }
}
