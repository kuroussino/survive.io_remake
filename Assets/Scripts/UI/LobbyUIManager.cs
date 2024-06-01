using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    #region Variables

    [Header("UI lobby var")]
    [SerializeField] InputField lobbyNameInputField;
    [SerializeField] GameObject lobbyUI;
    [SerializeField] GameObject PageUI;
    [SerializeField] GameObject PageContainerUI;
    [SerializeField] TextMeshProUGUI lobbyMissingtext;

    [Space(10)]
    [Header("Lobby Manager")]
    LobbyManager lobbyManager;

    [Space(10)]
    [Header("Variables")]
    QueryResponse lobbyList;
    List<FakeLobbies> fakeLobbyList=new List<FakeLobbies>();
    List<GameObject> pages = new List<GameObject>();
    GameObject currentPage;
    GameObject currentLobbyUI;
    #endregion

    #region Mono
    private void Awake()
    {
        lobbyManager=GetComponent<LobbyManager>();
    }
    private void Start()
    {
        RefreshLobbyList();
    }
    #endregion
    private void RefreshLobbyList()
    {
        //Prendere la lista di lobby, riempire gli slot di ogni pagina e creare una nuova pagina 
        DestroyOldPages();
        fakeLobbyList = GetFakeLobbyList();
        if (fakeLobbyList.Count > 0)
        {
            for (int i = 0; i < fakeLobbyList.Count; i++)
            {
                if (i % 5 == 0)
                {
                    Debug.Log(i);
                    currentPage = Instantiate(PageUI,PageContainerUI.transform);
                    pages.Add(currentPage);
                    if (i == 0)
                    {
                        currentPage.SetActive(true);
                    }
                    //Se le lobby sono 5 passa alla pagina successiva, quindi crea una nuova pagina disabilitata e aggiungi le lobby nei nuovi slot.
                }
                currentLobbyUI=Instantiate(lobbyUI, currentPage.transform);
                currentLobbyUI.GetComponent<LobbyHandleUI>().UpdateUI(fakeLobbyList[i]);
                //Ogni UI si controlla e aggiorna da sola, il tasto join viene controllato dalla lobby UI.
            }
            Debug.Log($"Pages: {pages.Count}");
        }
        else
        {
            Debug.Log("No lobby Found");
        }
       

    }

   /// <summary>
   /// Get a list of fake lobbies containing name, max players and current players. Method needed as testing purpose only
   /// </summary>
   /// <returns></returns>
    private List<FakeLobbies> GetFakeLobbyList()
    {
        List<FakeLobbies> lobbyList = new List<FakeLobbies>(); 
        for(int i = 1; i <= 10; i++)
        {
            FakeLobbies lobby = new FakeLobbies($"Lobby {i}", 6, Random.Range(0,7)); // Example: each lobby has max 10 players and starts with 0 current players
            lobbyList.Add(lobby);
        }
        return lobbyList;
    }

    /// <summary>
    /// Destroy the old pages of the Lobby List UI in order to create new ones.
    /// </summary>
    private void DestroyOldPages()
    {
        if (pages.Count > 0)
        {
            foreach (GameObject obj in pages)
            {
                pages.Remove(obj);
                Destroy(obj);
            }
        }
        Debug.Log($"Destroyed old pages, now having {pages.Count} pages");
    }
}


    #region struct lobbies
    /// <summary>
    /// Fake lobbies struct, needed for testing purpose only.
    /// </summary>
    /// 
    public struct FakeLobbies
    {
       public string LobbyName;
       public int maxPlayer;
       public int currentPlayer;
       public FakeLobbies(string Name,int maxPlayer,int currentPlayer)
      {
          LobbyName = Name;
          this.maxPlayer = maxPlayer;
          this.currentPlayer = currentPlayer;
      }
    }
    #endregion
