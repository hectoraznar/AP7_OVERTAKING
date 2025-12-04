using UnityEngine;

public class SaveChangeCarSkin : MonoBehaviour
{
    private Mesh carModel;
    private Color carColor; 
     [Header("ScriptCarManager")]
     public CarManager mCarManager; 

    //public CarChanger [] mChanger; 
     public CarChanger mChanger;
     [Header("ScriptGarageFunc...")]
     public GarageFuncionality mGarageF;

    void Start()
    {
       /* mChanger[0] = GameObject.Find("btnCar1").GetComponent<CarChanger>();
        mChanger[1] = GameObject.Find("btnCar2").GetComponent<CarChanger>();*/
    }
    public void getColorAndCarModel()
    {
        carColor = mGarageF.getCurrentColor();   
        mCarManager.SetColor(carColor);
        carModel = mChanger.getCarMesh(); 
        mCarManager.SetModelo(carModel);

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
