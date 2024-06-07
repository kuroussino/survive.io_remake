using CI.PowerConsole;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

[RequireComponent(typeof(LobbyManager))]
public class LobbyUIManager : MonoBehaviour
{
    #region Variables

    [Space(10)]
    [Header("UI login var")]
    [SerializeField] TMP_InputField playerNameField;
    [SerializeField] GameObject loginUI;
    [SerializeField] TextMeshProUGUI missingPlayerNameText;


    [Space(10)]
    [Header("UI lobby searching var")]
    [SerializeField] TMP_InputField lobbyNameInputField;
    [SerializeField] GameObject lobbyUI;
    [SerializeField] GameObject PageUI;
    [SerializeField] GameObject PageContainerUI;
    [SerializeField] GameObject LobbySearchOrCreateUI;
    [SerializeField] TextMeshProUGUI lobbyMissingtext;
    [SerializeField] TextMeshProUGUI pagesCountText;
    [SerializeField] TextMeshProUGUI lobbyNameMissingText;

    [Space(10)]
    [Header("UI lobby hub var")]
    [SerializeField] GameObject lobbyHubUI;
    [SerializeField] GameObject playerLobbyUI;
    [SerializeField] GameObject verticalLobbyLayoutUI;
    [SerializeField] GameObject startGameButton;
    [SerializeField] TextMeshProUGUI counterPlayers;
    [SerializeField] TextMeshProUGUI lobbyNamePreLobby;
    [SerializeField] GameObject preGameStart;



    [Space(10)]
    [Header("Lobby Manager")]
    LobbyManager lobbyManager;

    [Space(10)]
    [Header("Variables")]
    List<Lobby> lobbyList= new List<Lobby>();
    List<GameObject> pages = new List<GameObject>();
    List<GameObject> pagesToDestroy = new List<GameObject>();
    List<LobbyPlayerUI> playerShowedInUIList=new List<LobbyPlayerUI>();
    GameObject currentPage;
    GameObject currentLobbyUI;
    int currentPageNumber = 0;
    string lobbyName;
    bool isWaiting;
    #endregion

    #region Mono

    private void OnEnable()
    {
        EventsManager.LobbyReady += LobbyReadyEvent;
        EventsManager.OnShowLobby += OnShowingLobby;
        EventsManager.OnQuittedLobby += OnQuitPlayer;
        EventsManager.OnJoinedLobby += OnJoinPlayer;
        EventsManager.GameStarting += StartGameEvent;
    }

    private void OnDisable()
    {
        EventsManager.LobbyReady -= LobbyReadyEvent;
        EventsManager.OnShowLobby -= OnShowingLobby;
        EventsManager.OnQuittedLobby -= OnQuitPlayer;
        EventsManager.OnJoinedLobby -= OnJoinPlayer;
        EventsManager.GameStarting -= StartGameEvent;
    }



    private void Awake()
    {
        lobbyManager=GetComponent<LobbyManager>();
    }
    private void Start()
    {
        LobbySearchOrCreateUI.SetActive(false);
        lobbyHubUI.SetActive(false);
        loginUI.SetActive(true);

    }
    #endregion

    #region Login
    public void OnLoginButton()
    {
        if(playerNameField.text==""|| playerNameField.text == null)
        {
            StartCoroutine(HandleMissingNameText(missingPlayerNameText));
        }
        else
        {
            GoOnLobbySearchUI();
            EventsManager.OnPlayerNameSet?.Invoke(playerNameField.text);
        }
    }
    private void GoOnLobbySearchUI()
    {
        loginUI.SetActive(false);
        LobbySearchOrCreateUI.SetActive(true);
    }

    #endregion

    #region LobbyList
    private void LobbyReadyEvent()
    {
        RefreshLobbyList();
    }
    public void RefreshButton()
    {
        RefreshLobbyList();
    }

    /// <summary>
    /// Refresh the lobby list using the filters given in the lobbyManager, in order to update the UI and let the player join the lobby 
    /// </summary>
    private async void RefreshLobbyList()
    {
        //Prendere la lista di lobby, riempire gli slot di ogni pagina e creare una nuova pagina 
        DestroyOldPages();
        Debug.Log($"Pages after refreshing {pages.Count} ");
        //fakeLobbyList = GetFakeLobbyList();
        await GetLobbies();
        if (lobbyList != null)
        {
            Debug.Log(lobbyList.Count);
            if (lobbyList.Count > 0)
            {
                lobbyMissingtext.gameObject.SetActive(false);
                for (int i = 0; i < lobbyList.Count; i++)
                {
                    if (i % 5 == 0)
                    {
                        Debug.Log(i);
                        currentPage = Instantiate(PageUI, PageContainerUI.transform);
                        pages.Add(currentPage);
                        if (i == 0)
                        {
                            currentPage.SetActive(true);
                            currentPageNumber = 0;
                        }
                        //Se le lobby sono 5 passa alla pagina successiva, quindi crea una nuova pagina disabilitata e aggiungi le lobby nei nuovi slot.
                    }
                    currentLobbyUI = Instantiate(lobbyUI, currentPage.transform);
                    currentLobbyUI.GetComponent<LobbyHandleUI>().UpdateUI(lobbyList[i]);
                    //Ogni UI si controlla e aggiorna da sola, il tasto join viene controllato dalla lobby UI.
                }

                Debug.Log($"Pages: {pages.Count}");
            }
            else
            {
                lobbyMissingtext.gameObject.SetActive(true);
                Debug.Log("No lobby Found");
            }
        }
        UpdatePageUI();
    }
    private async Task GetLobbies()
    {
        try
        {
            lobbyList = await lobbyManager.SearchLobbies();
        }
        catch (LobbyServiceException err)
        {
            print("error getting lobby list: " + err);
        }
    }

    /// <summary>
    /// Destroy the old pages of the Lobby List UI in order to create new ones.
    /// </summary>
    private void DestroyOldPages()
    {
        pagesToDestroy = pages;
        Debug.Log("Pages to destryo count "+pagesToDestroy.Count);
        foreach (GameObject page in pagesToDestroy)
        {
            if (page != null)
            {
                Debug.Log($"Destroying page {page.name}");
                page.SetActive(false);
                Destroy(page);
            }
        }
        pagesToDestroy.Clear();
        pages.Clear();
        Debug.Log($"Destroyed old pages, now having {pages.Count} pages");
    }

    /// <summary>
    /// Changes the page on the arrow button click
    /// </summary>
    public void ChangePagePlus()
    {
        if (currentPageNumber+1<pages.Count)
        {
            pages[currentPageNumber].SetActive(false);
            currentPageNumber++;
            pages[currentPageNumber].SetActive(true);
        }
        UpdatePageUI();
    }

    /// <summary>
    /// Changes the page on the arrow button click
    /// </summary>
    public void ChangePageMinus()
    {
        if (currentPageNumber-1>=0)
        {
            pages[currentPageNumber].SetActive(false);
            currentPageNumber--;
            pages[currentPageNumber].SetActive(true);
        }
        UpdatePageUI();
    }
    private void UpdatePageUI()
    {
        if (pages.Count > 0)
        {
            pagesCountText.text = $"{currentPageNumber+1} / {pages.Count}";
        }
        else
        {
            pagesCountText.text = $"{currentPageNumber} / {pages.Count}";
        }
        
    }
    #endregion

    #region OnHost

    /// <summary>
    /// Method Called when hosting a game from the UI 
    /// </summary>
    public void OnHost()
    {
        if (lobbyNameInputField.text != "")
        {
            lobbyName = lobbyNameInputField.text;
            EventsManager.OnHostCreateLobbyWithName?.Invoke(lobbyName);
        }
        else
        {
            StartCoroutine(HandleMissingNameText(lobbyNameMissingText));
        }
    }

    /// <summary>
    /// Missing Text appearing for a shor amount of time and disappearing again only when the name of the lobby is equal to ""
    /// </summary>
    /// <returns></returns>
    private IEnumerator HandleMissingNameText(TextMeshProUGUI text)
    {
        if (!isWaiting)
        {
            text.text = "Name is missing or invalid";
            isWaiting = true;
            text.gameObject.SetActive(true);
            yield return new WaitForSeconds(2f);
            text.gameObject.SetActive(false);
            isWaiting = false;
        }

    }
    #endregion

    #region OnLobbyCreatedOrJoined

    /// <summary>
    /// Called throught event when the lobby is up and created, it will shows the UI of the lobbyHub with a list of players connected
    /// </summary>
    /// <param name="lobby"></param>
    private void OnShowingLobby(Lobby lobby, bool isHostVar)
    {
        lobbyHubUI.SetActive(true);
        LobbySearchOrCreateUI.SetActive(false);
        if(isHostVar)
        {
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
        }
        GameObject playerUI;
        GameObject[] panels = GameObject.FindGameObjectsWithTag("PlayerUI");
        foreach (GameObject p in panels)
        {
            Destroy(p);
        }
        counterPlayers.text= lobby.Players.Count.ToString()+"/"+lobby.MaxPlayers.ToString();
        lobbyNamePreLobby.text = lobby.Name;
        foreach (Unity.Services.Lobbies.Models.Player player in lobby.Players)
        {
            PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"player name is {player.Data["PlayerName"].Value}");
            playerUI = Instantiate(playerLobbyUI, verticalLobbyLayoutUI.transform);
            playerUI.GetComponent<LobbyPlayerUI>().UpdatePlayerUI(player, lobbyManager.isHost(player.Id));
            playerShowedInUIList.Add(playerUI.GetComponent<LobbyPlayerUI>());
        }

    }

    /// <summary>
    /// Called when player is quitting the lobby when the lobby is already up
    /// </summary>
    /// <param name="player"></param>
    private void OnQuitPlayer(Unity.Services.Lobbies.Models.Player player)
    {
       for(int i = 0; i < playerShowedInUIList.Count; i++)
       {
            if (playerShowedInUIList[i].playerID == player.Id)
            {
                playerShowedInUIList[i].RemovePlayer();
                return;
            }
       }
    }

    /// <summary>
    /// Called when the player is joining a lobby that is already up 
    /// </summary>
    /// <param name="player"></param>
    private void OnJoinPlayer(Unity.Services.Lobbies.Models.Player player)
    {
        GameObject playerUI;
        playerUI = Instantiate(playerLobbyUI, verticalLobbyLayoutUI.transform);
        //playerUI.GetComponent<LobbyPlayerUI>().UpdatePlayerUI(player, lobbyManager.isHost());
        playerShowedInUIList.Add(playerUI.GetComponent<LobbyPlayerUI>());
    }

    public void OnLeaveLobby() 
    {
        lobbyHubUI.SetActive(false);
        LobbySearchOrCreateUI.SetActive(true);
        lobbyNameInputField.text = "";
        EventsManager.OnLeaveLobbyButton?.Invoke();
    }

    public void OnClickStartGame()
    {

        EventsManager.OnHostStartGame?.Invoke();
    }

    private void StartGameEvent()
    {
        lobbyHubUI.SetActive(false);
        preGameStart.SetActive(true);
    }

    #endregion

    /// <summary>
    /// Get a list of fake lobbies containing name, max players and current players. Method needed as testing purpose only
    /// </summary>
    /// <returns></returns>
    //private List<FakeLobbies> GetFakeLobbyList()
    //{
    //    List<FakeLobbies> lobbyList = new List<FakeLobbies>(); 
    //    for(int i = 1; i <= numberOfFakeLobbiesWanted; i++)
    //    {
    //        FakeLobbies lobby = new FakeLobbies($"Lobby {i}", 6, Random.Range(0,7)); // Example: each lobby has max 10 players and starts with 0 current players
    //        lobbyList.Add(lobby);
    //    }
    //    return lobbyList;
    //}
}


//#region struct lobbies
///// <summary>
///// Fake lobbies struct, needed for testing purpose only.
///// </summary>
///// 
//public struct FakeLobbies
//{
//   public string LobbyName;
//   public int maxPlayer;
//   public int currentPlayer;
//   public FakeLobbies(string Name,int maxPlayer,int currentPlayer)
//  {
//      LobbyName = Name;
//      this.maxPlayer = maxPlayer;
//      this.currentPlayer = currentPlayer;
//  }
//}
//#endregion
