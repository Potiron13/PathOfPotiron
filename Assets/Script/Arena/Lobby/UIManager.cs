using Potiron.Core.Singletons;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    private Button startHostButton;
    [SerializeField]
    private Button startClientButton;
    [SerializeField]
    private TextMeshProUGUI playersInGameText;
    [SerializeField]
    private TMP_InputField joinCodeInput;
    [SerializeField]
    private TextMeshProUGUI joinCodeDisplay;    
    [SerializeField]
    private GameObject hostCard;
    [SerializeField]
    private GameObject joinCard;    
    [SerializeField]
    private GameObject joinUI;

    private bool hasServerStarted;

    private void Awake()
    {
        Cursor.visible = true;
        NetworkManager.Singleton.OnServerStarted += () =>
         {
             NetworkObjectPool.Instance.InitializePool();
         };
    }

    void Update()
    {
        playersInGameText.text = $"Players in game: {PlayersManager.Instance.PlayersInGame}";
    }

    void Start()
    {
        // START HOST
        startHostButton?.onClick.AddListener(async () =>
        {
            // this allows the UnityMultiplayer and UnityMultiplayerRelay scene to work with and without
            // relay features - if the Unity transport is found and is relay protocol then we redirect all the 
            // traffic through the relay, else it just uses a LAN type (UNET) communication.
            if (RelayManager.Instance.IsRelayEnabled)
            {
                RelayHostData relayHostData = await RelayManager.Instance.SetupRelay();
                joinCodeDisplay.text = $"Share this code : {relayHostData.JoinCode}";
                joinUI.SetActive(false);
            }
            if (NetworkManager.Singleton.StartHost())
            {
                Logger.Instance.LogInfo("Host started...");                
            } else
            {
                Logger.Instance.LogInfo("Unable to start host...");
            }                
        });

        // START CLIENT
        startClientButton?.onClick.AddListener(async () =>
        {
            if (RelayManager.Instance.IsRelayEnabled && !string.IsNullOrEmpty(joinCodeInput.text))
            {
                await RelayManager.Instance.JoinRelay(joinCodeInput.text);
                joinUI.SetActive(false);
            }

            if (NetworkManager.Singleton.StartClient())
                Logger.Instance.LogInfo("Client started...");
            else
                Logger.Instance.LogInfo("Unable to start client...");
        });

        // STATUS TYPE CALLBACKS
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            Logger.Instance.LogInfo($"{id} just connected...");
        };

        NetworkManager.Singleton.OnServerStarted += () =>
        {
            hasServerStarted = true;
        };
    }
}