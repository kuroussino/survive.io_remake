using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameUiManager : MonoBehaviour
{
    #region Variables & Properties

    #region Local
    [SerializeField] Sprite crosshairIdle;
    [SerializeField] Sprite crosshairPressed;
    [SerializeField] Image crosshair;
    #endregion

    #region Public
    #endregion

    #endregion

    #region Monobehaviour
    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        crosshair.transform.position = Input.mousePosition;
        
        if (Input.GetMouseButtonDown(0))
        {
            crosshair.sprite = crosshairPressed;
        }

        if (Input.GetMouseButtonUp(0))
        {
            crosshair.sprite = crosshairIdle;
        }
    }
    #endregion

    #region Methods
    #endregion
}
