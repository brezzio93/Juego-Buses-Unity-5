using UnityEngine;

using UnityEngine.UI;

namespace Com.MyCompany.MyGame
{
    public class ButtonListButton : MonoBehaviour
    {
        [SerializeField]
        private Text myText;

        [SerializeField]
        private ServerManager buttonControl;

        private string myTextString;

        public void SetText(string str)
        {
            myTextString = str;
            myText.text = str;
        }

        public void GetRoomName()
        {
            buttonControl.ButtonClicked(myTextString);
        }

        public void JoinRoom()
        {
            Debug.Log("Joining Room "+myTextString);
            //buttonControl.JoinSelectedRoom(myTextString);
        }
    }
}