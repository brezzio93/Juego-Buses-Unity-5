using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Com.MyCompany.MyGame
{
    public class PlayerParameters : MonoBehaviour
    {
        [SerializeField]
        private Image Avatar;

        private ExitGames.Client.Photon.Hashtable CustomProps = new ExitGames.Client.Photon.Hashtable();

        // Use this for initialization
        private void Start()
        {
        }

        public void SetPlayerProperties()
        {
            ExitGames.Client.Photon.Hashtable PlayerCustomProps = new ExitGames.Client.Photon.Hashtable();
            CustomProps["paga"] = new bool[10];
            CustomProps["llega"] = new bool[10];
            PhotonNetwork.SetPlayerCustomProperties(CustomProps);
        }

        public void SetPlayerAvatar()
        {

        }
    }
}