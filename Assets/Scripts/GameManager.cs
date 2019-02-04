using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Com.MyCompany.MyGame
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        private ServerManager server = new ServerManager();
        private RoomParameters parameters = new RoomParameters();

        [SerializeField]
        private Text txt;

        [SerializeField]
        private Text nombreSala;

        private Scene currentScene;
        private string SceneName;

        private static List<string> RoomList = new List<string>();
        private ExitGames.Client.Photon.Hashtable CustomProps = new ExitGames.Client.Photon.Hashtable();

        private static bool createRoom;

        private List<string> roomName = new List<string>();
        private Dictionary<string, RoomInfo> cachedRoomList;

        private static GameManager gameManager;
        public static GameManager instance;

        [SerializeField]
        private GameObject buttonTemplate;

        private List<GameObject> buttons;

        #region Photon Callbacks

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                //LoadArena();
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                //LoadArena();
            }
        }

        

        #endregion Photon Callbacks

        #region MonoBehaviour Callbacks

        // Use this for initialization
        public void Awake()
        {
            DontDestroyOnLoad(this);
            cachedRoomList = new Dictionary<string, RoomInfo>();
        }

        private void Start()
        {
            currentScene = SceneManager.GetActiveScene();
            SceneName = currentScene.name;
        }

        private void Update()
        {
            if (SceneName == "03 Lobby")
            {
                if (PhotonNetwork.IsConnected)
                {
                    //ListarSalas();
                }
            }

            if (SceneName == "05 Espera")
            {
                if (PhotonNetwork.InRoom)
                {
                    if (PhotonNetwork.IsMasterClient) PlayersInRoom();
                    else txt.text = "Esperando Jugadores... " + PhotonNetwork.CurrentRoom.PlayerCount + " de " + PhotonNetwork.CurrentRoom.MaxPlayers;
                    nombreSala.text = "Sala: " + PhotonNetwork.CurrentRoom.Name;
                }
            }
        }

        #endregion MonoBehaviour Callbacks

        #region Public Methods

        public void LeaveRoom()
        {
            if (PhotonNetwork.InRoom)
                PhotonNetwork.LeaveRoom();
        }

        public void SwitchScenes(int idScene)
        {
            SceneManager.LoadScene(idScene);
        }

        /// <summary>
        /// Se crea la sala con los parametros ingresados por el Host
        ///  </summary>
        public void CrearSala()
        {
            Debug.Log("CrearSala()");
            int cantidad = System.Convert.ToInt32(RoomParameters.param.cantidad);
            if (cantidad <= 20)
            {
                PhotonNetwork.CreateRoom(PhotonNetwork.LocalPlayer.NickName, new RoomOptions
                {
                    MaxPlayers = System.Convert.ToByte(cantidad),
                    IsVisible = true,
                });
                SwitchScenes(5);
            }
        }

        public void JoinRandomRoom()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        public void JoinSelectedRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);
        }

        /// <summary>
        /// Redirige al registro del jugador y guarda la selección de crear o unirse a sala
        /// </summary>
        /// <param name="create">
        /// Parametro booleano que indica si se creará o no una sala
        /// </param>
        public void GoLogin(bool create)
        {
            ServerManager.createRoom = create;

            SwitchScenes(1);
        }

        /// <summary>
        /// Función utilizada para saber si el jugador creará o se unirá a una sala
        /// </summary>
        public void CreateOrJoin()
        {
            server.CreateOrJoin();
        }

        /// <summary>
        /// El host actualiza los parametros de la sala y comienza el juego
        /// </summary>
        public void ComenzarJuego()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                parameters.SetRoomProperties();
                Debug.Log("Monto (Sala): " + PhotonNetwork.CurrentRoom.CustomProperties["monto"]);
                LoadArena();
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Player on the Room : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            //PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("06 Sala");
        }

        /// <summary>
        /// Se lista a los jugadores dentro de la sala actual
        /// </summary>
        private void PlayersInRoom()
        {
            int i = System.Convert.ToInt32(PhotonNetwork.CurrentRoom.PlayerCount);
            txt.text = "Jugadores Conectados:\n" + System.Convert.ToString(PhotonNetwork.CurrentRoom.PlayerCount)
                                + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
            while (i != 0)
            {
                i--;
                Debug.Log("Players in Room: " + PhotonNetwork.PlayerList[i].NickName);
            }
        }

        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            Debug.Log("UpdateCachedRoomList()");
            foreach (RoomInfo info in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (cachedRoomList.ContainsKey(info.Name))
                    {
                        cachedRoomList.Remove(info.Name);
                        roomName.Remove(info.Name);
                    }
                    continue;
                }

                // Update cached room info
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                // Add new room info to cache
                else
                {
                    cachedRoomList.Add(info.Name, info);
                    roomName.Add(info.Name);
                }
            }
            if (SceneName == "03 Lobby")
            {
                ListarSalas(roomName);
            }
        }

        /// <summary>
        /// Se obtiene una lista de todas las salas existentes
        /// </summary>
        public void ListarSalas(List<string> roomList)
        {
            string[] room = roomList.ToArray();

            buttons = new List<GameObject>();
            foreach (string info in room)
            {
                Debug.Log("Sala " + info);
            }

            if (buttons.Count > 0)
            {
                foreach (GameObject button in buttons)
                    Destroy(button.gameObject);
            }
            buttons.Clear();

            for (int i = 0; i < room.Length; i++)
            {
                GameObject button = Instantiate(buttonTemplate) as GameObject;
                button.SetActive(true);
                button.GetComponent<ButtonListButton>().SetText(room[i]);
                button.transform.SetParent(buttonTemplate.transform.parent, false);
            }
        }

        public void ButtonClicked(string textString)
        {
            Debug.Log(textString);
        }

        public void SaveAvatar()
        {
            
        }

        #endregion Private Methods
    }
}