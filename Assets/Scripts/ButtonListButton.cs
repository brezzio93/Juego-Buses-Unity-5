using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace Com.MyCompany.MyGame
{
    public class ButtonListButton : MonoBehaviour
    {

        [SerializeField]
        private Text myText;
        [SerializeField]
        private GameManager buttonControl;

        private string myTextString;

        public void SetText(string str)
        {
            myTextString = str;
            myText.text = str;
        }

        public void JoinRoom()
        {
            buttonControl.JoinSelectedRoom(myTextString);
        }
    }
}

