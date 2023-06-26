
using UnityEngine;

public class FPSLimit : MonoBehaviour
{

    #region Public Variables

    public int expectedFPS = -1;

    #endregion
    

    #region Methods

    public void Awake()
    {
        Application.targetFrameRate = expectedFPS;
    }
    
    #endregion


    
}
