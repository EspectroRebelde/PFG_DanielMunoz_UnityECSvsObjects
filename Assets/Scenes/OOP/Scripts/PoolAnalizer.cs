
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolAnalizer : MonoBehaviour
{

    #region Public Variables

    // Reference to canvas text
    public UnityEngine.UI.Text bulletText;
    public UnityEngine.UI.Text enemyText;
    
    #endregion

    #region Private Variables

    #endregion

    #region Properties

    #endregion

    #region MonoBehaviours

    private void Start()
    {
        // Start the coroutine that will update the text
        StartCoroutine(UpdateText());
    }
    
    private IEnumerator UpdateText()
    {
        while (true)
        {
            // Check the count of enemy and bullet objects tags
            bulletText.text = GameObject.FindGameObjectsWithTag("Bullet").Length.ToString();
            enemyText.text = GameObject.FindGameObjectsWithTag("Enemy").Length.ToString();
            
            // Wait for 0.1 seconds
            yield return new WaitForSeconds(0.1f);
        }
    }

    #endregion

    #region Methods

    #endregion


    
}
