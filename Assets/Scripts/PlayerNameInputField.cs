using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Com.MyCompany.MyGame
{
    /// <summary>
    /// Player name input field. Let the user input his name, will appear above the player in the game.
    /// </summary>
    [RequireComponent(typeof(InputField))]

    public class PlayerNameInputField : MonoBehaviour
    {
        public PlayerParameters parametros = new PlayerParameters();

        #region Private Constants

        // Store the PlayerPref Key to avoid typos
        private const string playerNamePrefKey = "PlayerName";

        private ExitGames.Client.Photon.Hashtable CustomProps = new ExitGames.Client.Photon.Hashtable();

        #endregion Private Constants

        #region MonoBehaviour CallBacks

        private void Start()
        {
            string defaultName = string.Empty;
            InputField _inputField = this.GetComponent<InputField>();
            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;
        }

        #endregion MonoBehaviour CallBacks

        #region Public Methods

        /// <summary>
        /// Guarda el nombre del jugador para futuras sesiones.
        /// </summary>
        /// <param name="value">The name of the Player</param>
        public void SetPlayerName(string value)
        {
            // #Important
            if (string.IsNullOrEmpty(value))
            {
                //Debug.LogError("Player Name is null or empty");
                return;
            }
            PhotonNetwork.NickName = value;
            PlayerPrefs.SetString(playerNamePrefKey, value);            
        }

        #endregion Public Methods
    }
}