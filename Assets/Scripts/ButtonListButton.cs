using UnityEngine;

using UnityEngine.UI;

namespace Com.MyCompany.MyGame
{
    public class ButtonListButton : MonoBehaviour
    {
        [SerializeField]
        private Text myText;

        [SerializeField]
        private ServerManager server;

        private string myTextString;

        public void SetText(string str)
        {
            myTextString = str;
            myText.text = str;
        }

        public void GetRoomName()
        {
            server.GetRoomName(myTextString);            
        }

        public void JoinRoom()
        {
            Debug.Log("Joining Room "+myTextString);
            //buttonControl.JoinSelectedRoom(myTextString);
        }
    }
}