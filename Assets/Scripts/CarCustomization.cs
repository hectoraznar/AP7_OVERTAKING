using UnityEngine;
using UnityEngine.UI;
public class CarCustomization : MonoBehaviour
{
    public GameObject [] carsGM;
    void Start()
    {
  
        carsGM[0].SetActive(true);
        carsGM[1].SetActive(false);
    }

   public void ChangeCarModel(int carIndex)
    {
      if (carIndex == 1)
        {
            carsGM[0].SetActive(false);
            carsGM[carIndex].SetActive(true);
        }
      if (carIndex == 0)
        {
            carsGM[1].SetActive(false);
            carsGM[carIndex].SetActive(true);
        }
        
    }
}
