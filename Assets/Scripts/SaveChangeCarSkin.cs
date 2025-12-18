using UnityEngine;
using UnityEngine.SceneManagement;
public class SaveChangeCarSkin : MonoBehaviour
{
    private GameObject carModel;
    private Color carColor; 
     [Header("ScriptCarManager")]
     public CarManager mCarManager; 

    //public CarChanger [] mChanger; 
     
     [Header("ScriptGarageFunc...")]
     public GarageFuncionality mGarageF;
     public Mesh mCarMesh;

    void Start()
    {
       /* mChanger[0] = GameObject.Find("btnCar1").GetComponent<CarChanger>();
        mChanger[1] = GameObject.Find("btnCar2").GetComponent<CarChanger>();*/
    }
    public void getColorAndCarModel()
    {
        carColor = mGarageF.GetCurrentColor();
        Debug.Log(carColor.ToString());   
        //mCarManager.SetColor(carColor);
        carModel = mGarageF.GetCar();
        Debug.Log(carModel.ToString()); 
        mCarMesh = carModel.GetComponentInChildren<MeshFilter>().mesh;
     
        //mCarManager.SetColor(carColor);
        //mCarManager.SetModelo(carModel);
        carData.SetCarSkin( carModel, carColor, mCarMesh);
        //mCarManager.SetModelo(carModel);
        SaveAndChangeScene();
    }
   
   public GameObject GetModel()
    {
        return carModel;
    }
    public Color GetColor()
    {
        return carColor;
    }

    public void SaveAndChangeScene()
    {
        SceneManager.LoadScene("Autopista");
    }
  /*  public void  getMeshByID( int index)
    {
         if(index == 1)
        {
             carModel = mChanger[1].getCarMesh();
        }
        else
        {
              carModel = mChanger[0].getCarMesh();
        }
    }*/
   
}