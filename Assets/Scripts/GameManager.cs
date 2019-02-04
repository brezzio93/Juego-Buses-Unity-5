using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.MyCompany.MyGame
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        private ServerManager server = new ServerManager();
        private RoomParameters parameters = new RoomParameters();
        private Scene currentScene;
        public static string SceneName;
        private ExitGames.Client.Photon.Hashtable CustomProps = new ExitGames.Client.Photon.Hashtable();
        private static bool createRoom;
        private List<GameObject> buttons;

        public static GameManager instance = null;

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
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
            }
        }

        #endregion Photon Callbacks

        #region MonoBehaviour Callbacks

        private void Awake()
        {            
            if (instance == null) instance = this;
            else if (instance != this) Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            currentScene = SceneManager.GetActiveScene();
            SceneName = currentScene.name;
        }

        private void Update()
        {

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

        #endregion Public Methods
    }
}