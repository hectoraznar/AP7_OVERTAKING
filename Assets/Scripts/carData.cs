using UnityEngine;

public static class carData
{
    public static GameObject carModel;
    public static Color carColor;
    public static Mesh carMeshFilter;
    public static Mesh [] bugattiMesh;

    public static void  SetCarSkin(GameObject car, Color color , Mesh meshFilter)
    {
        carModel = car;
        carMeshFilter = meshFilter;
        carColor = color;

       // Object.DontDestroyOnLoad(car);

    }
    public static GameObject GetCarModel()
    {
        return carModel;
    }
    public static Color GetCarColor()
    {
        return carColor;
    }
    public static Mesh GetCarMesh()
    {
        return carMeshFilter;
    }

     public static void  SetBugattiSkin(GameObject car, Color color , Mesh [] mesh)
    {
        carModel = car;
        bugattiMesh = mesh;
        carColor = color;

       // Object.DontDestroyOnLoad(car);

    }
    public static Mesh [] GetBugattiMesh()
    {
        return bugattiMesh;
    }
}