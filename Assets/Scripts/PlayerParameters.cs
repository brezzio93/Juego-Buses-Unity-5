using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Com.MyCompany.MyGame
{
    public class PlayerParameters : MonoBehaviour
    {
        public Image Avatar;        
        public Image Chosen;
        public static string ChosenName;

        private ExitGames.Client.Photon.Hashtable CustomProps = new ExitGames.Client.Photon.Hashtable();

        // Use this for initialization
        private void Start()
        {
        }
        private void Update()
        {
            
        }

        public void SetPlayerProperties()
        {
            Debug.Log(ChosenName);
            ExitGames.Client.Photon.Hashtable PlayerCustomProps = new ExitGames.Client.Photon.Hashtable();
            CustomProps["paga"] = new bool[10];
            CustomProps["llega"] = new bool[10];
            CustomProps["Avatar"] = Chosen;
            PhotonNetwork.SetPlayerCustomProperties(CustomProps);
        }

        public void SetPlayerAvatar()
        {
            ChosenName = Avatar.sprite.name;
            Chosen.sprite = Avatar.sprite;
            Chosen.gameObject.SetActive(true);
        }


    }
}