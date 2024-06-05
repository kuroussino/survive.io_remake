using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StartButtonUI : MonoBehaviour
{
    #region var
    Button startButton;
    #endregion
    #region Mono
    private void Awake()
    {
        startButton = GetComponent<Button>();
        startButton.onClick.AddListener(OnStartButtonClick);
    }
    #endregion
    #region Methods
    private void OnStartButtonClick()
    {
        if (NetworkManager.Singleton!=null)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                EventsManager.OnHostStartGame?.Invoke();
            }
        }
        else
        {
            Debug.Log("Network Manager is null find another way to get the host of the game");
        }

    }
    #endregion
}
