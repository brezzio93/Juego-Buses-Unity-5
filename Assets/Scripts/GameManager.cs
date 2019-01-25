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

        #endregion





        #region MonoBehaviour Callbacks
        // Use this for initialization
        void Awake()
        {
            //DontDestroyOnLoad(this);
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
                    //CustomRoomProperties =  {
                    //    {"monto",Parametros.param.monto },
                    //    {"precio",Parametros.param.precio },
                    //    {"ganancia",Parametros.param.ganancia },
                    //},            
                    MaxPlayers = System.Convert.ToByte(cantidad),
                    IsVisible = true,
                });

                SaveRoom();
                //SetPlayerProperties();            
                SwitchScenes(5);
            }
        }

        public void SetPlayerProperties()
        {
            ExitGames.Client.Photon.Hashtable PlayerCustomProps = new ExitGames.Client.Photon.Hashtable();
            CustomProps["monto"] = Parametros.param.monto;
            CustomProps["precio"] = Parametros.param.precio;
            CustomProps["ganancia"] = Parametros.param.ganancia;
            PhotonNetwork.SetPlayerCustomProperties(CustomProps);
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
        /// Función utilizada para saber si el jugador creará o se unirá a una sala
        /// </summary>
        /// <param name="create">
        /// Parametro booleano que indica si se está 
        /// </param>
        public void GoLogin(bool create)
        {
            createRoom = create;
            SwitchScenes(1);
        }

        public void CreateOrJoin()
        {
            if (PhotonNetwork.IsConnected)
            { 
                PhotonNetwork.JoinLobby();
                Debug.Log("CreateOrJoin " + createRoom);
                if (createRoom == true) SwitchScenes(4);
                else //SwitchScenes(3);
                    PhotonNetwork.JoinRandomRoom();
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

        public void ListarSalas()
        {
            if (PhotonNetwork.CurrentLobby.IsDefault)
            {
                Debug.Log("Lobby is Default");
                string sqlLobbyFilter = "C0 = 0";
                PhotonNetwork.GetCustomRoomList(PhotonNetwork.CurrentLobby, sqlLobbyFilter);
                Debug.Log(PhotonNetwork.CountOfRooms);
                //PhotonNetwork.room
            }
            //LoadRoom();
            //foreach (string str in RoomList)
            //{
            //    Debug.Log(str);
            //    txt.text = str;
            //}
                                                

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

        #endregion
    }
}

