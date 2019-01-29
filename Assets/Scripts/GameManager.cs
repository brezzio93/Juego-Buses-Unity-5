using System;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;


using Photon.Pun;
using Photon.Realtime;

using UnityEngine.UI;
using System.Collections.Generic;

namespace Com.MyCompany.MyGame
{
    public class GameManager : MonoBehaviourPunCallbacks
    {

        [SerializeField]
        private Text txt;
        [SerializeField]
        private Text nombreSala;

        private Scene currentScene;
        private string SceneName;
        
        private static List<string> RoomList = new List<string>();
        ExitGames.Client.Photon.Hashtable CustomProps = new ExitGames.Client.Photon.Hashtable();

        private static bool createRoom;

        private Dictionary<string, RoomInfo> cachedRoomList;


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

        #endregion





        #region MonoBehaviour Callbacks
        // Use this for initialization
        public void Awake()
        {
            cachedRoomList = new Dictionary<string, RoomInfo>();
        }

        void Start()
        {
            currentScene = SceneManager.GetActiveScene();
            SceneName = currentScene.name;                                        
        }

        void Update()
        {

            if(SceneName == "02 Lobby")
            {
                if (PhotonNetwork.IsConnected) ListarSalas();

            }
            
            if (SceneName == "05 Espera")
            {                
                                
                if (PhotonNetwork.IsMasterClient) PlayersInRoom();
                else txt.text = "Esperando Jugadores... "+ PhotonNetwork.CurrentRoom.PlayerCount+" de "+ PhotonNetwork.CurrentRoom.MaxPlayers;
                nombreSala.text = "Sala: " + PhotonNetwork.CurrentRoom.Name;                         

            }
            
        }

        #endregion




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
            int cantidad = System.Convert.ToInt32(Parametros.param.cantidad);            
            if (cantidad <= 20)
            {
                PhotonNetwork.CreateRoom(PhotonNetwork.LocalPlayer.NickName, new RoomOptions
                {           
                    MaxPlayers = System.Convert.ToByte(cantidad),
                    IsVisible = true,
                });

                //SaveRoom();          
                SwitchScenes(5);
            }
        }

        public void JoinRandomRoom()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        public void JoinSelectedRoom()
        {
            string SelectedRoom = PhotonNetwork.MasterClient.NickName;
            PhotonNetwork.JoinRoom(SelectedRoom);
        }

        /// <summary>
        /// Redirige al registro del jugador y guarda la selección de crear o unirse a sala
        /// </summary>
        /// <param name="create">
        /// Parametro booleano que indica si se creará o no una sala
        /// </param>
        public void GoLogin(bool create)
        {
            createRoom = create;
            SwitchScenes(1);
        }
        /// <summary>
        /// Función utilizada para saber si el jugador creará o se unirá a una sala
        /// </summary>
        public void CreateOrJoin()
        {
            if (PhotonNetwork.IsConnected)
            { 
                PhotonNetwork.JoinLobby();
                Debug.Log("CreateOrJoin " + createRoom);
                if (createRoom == true) SwitchScenes(4);
                else SwitchScenes(3);
                    //PhotonNetwork.JoinRandomRoom();
            }
        }


        /// <summary>
        /// El host actualiza los parametros de la sala y comienza el juego
        /// </summary>
        public void ComenzarJuego()
        {                    
            if (PhotonNetwork.IsMasterClient)
            {                
                SetRoomProperties();
                Debug.Log("Monto (Sala): " + PhotonNetwork.CurrentRoom.CustomProperties["monto"]);
                LoadArena();
                

            }
        }

        /// <summary>
        /// Se añaden los parametros de la sala al objeto Room
        /// </summary>
        public void SetRoomProperties()
        {
            CustomProps["monto"] = Parametros.param.monto;
            CustomProps["precio"] = Parametros.param.precio;
            CustomProps["ganancia"] = Parametros.param.ganancia;
            Debug.Log(CustomProps["monto"]);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomProps);
        }

        /// <summary>
        /// Se obtiene una lista de todos los jugadores Host para listar la sala que crearon
        /// </summary>
        public void ListarSalas()
        {
            List<string> roomList = new List<string>();
            int i = 0;
            if (PhotonNetwork.CurrentLobby.IsDefault)
            {
                Debug.Log("Lobby is Default");
                string sqlLobbyFilter = "C0 = 0";                 
                PhotonNetwork.GetCustomRoomList(PhotonNetwork.CurrentLobby, sqlLobbyFilter);                                

                Debug.Log("N° of Rooms: "+PhotonNetwork.CountOfRooms);
                Debug.Log("N° of Players: " + PhotonNetwork.CountOfPlayers);
                
                foreach(Player player in PhotonNetwork.PlayerList)
                {
                    Debug.Log(i);
                    if (player.IsMasterClient)
                    {
                        roomList.Add(player.NickName);
                    }
                }
            }                                                     
        }

        public void SaveRoom()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath +"/RoomList.dat");
            string nombreSala = PhotonNetwork.LocalPlayer.NickName;
            bf.Serialize(file, nombreSala);
            file.Close();
            
        }

        public void LoadRoom()
        {
            if(File.Exists(Application.persistentDataPath + "/RoomList.dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();                
                FileStream file = File.Open(Application.persistentDataPath + "/RoomList.dat", FileMode.Open);
                string nombreSala = (string)bf.Deserialize(file);
                RoomList.Add(nombreSala);
                file.Close();
            }
        }    

        #endregion





        #region Private Methods

        void LoadArena()
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
            txt.text = "Jugadores Conectados:\n"+System.Convert.ToString(PhotonNetwork.CurrentRoom.PlayerCount) 
                                + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
            while (i != 0) {
                i--;
                Debug.Log("Players in Room: "+PhotonNetwork.PlayerList[i].NickName);
                
            }
        }


        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            Debug.Log("UpdateCachedRoomList");
            foreach (RoomInfo info in roomList)
            {
                Debug.Log(info.Name);
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (cachedRoomList.ContainsKey(info.Name))
                    {
                        cachedRoomList.Remove(info.Name);
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
                }
                Debug.Log("Sala " + cachedRoomList[info.Name]);
            }
            
        }



        #endregion
    }
}

