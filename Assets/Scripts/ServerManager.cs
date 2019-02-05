using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.MyCompany.MyGame
{
    public class ServerManager : MonoBehaviourPunCallbacks
    {
        private List<string> roomName = new List<string>();
        private Dictionary<string, RoomInfo> cachedRoomList;
        private Scene currentScene;
        private string SceneName;

        [SerializeField]
        private GameObject buttonTemplate;

        private List<GameObject> buttons;
        public static string roomSelected;
        public static bool createRoom;

        // Use this for initialization
        public void Awake()
        {
            cachedRoomList = new Dictionary<string, RoomInfo>();
        }

        private void Start()
        {
            currentScene = SceneManager.GetActiveScene();
            SceneName = currentScene.name;
        }

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

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            UpdateCachedRoomList(roomList);
        }

        #endregion Photon Callbacks

        public void SwitchScenes(int idScene)
        {
            SceneManager.LoadScene(idScene);
        }

        /// <summary>
        /// Función utilizada para saber si el jugador creará o se unirá a una sala
        /// </summary>
        public void CreateOrJoin()
        {
            Debug.Log(PlayerParameters.ChosenName);
            
            if (PlayerParameters.ChosenName != null)
            {
                if (PhotonNetwork.IsConnected)
                {
                    PhotonNetwork.JoinLobby();
                    Debug.Log("CreateOrJoin " + createRoom);
                    if (createRoom == true) SwitchScenes(4);
                    else SwitchScenes(3);
                }
            }
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

        public void GetRoomName(string textString)
        {
            roomSelected = textString;
            Debug.Log(roomSelected);
        }

        public void UpdateCachedRoomList(List<RoomInfo> roomList)
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
            Debug.Log("ListarSalas()");
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
    }
}