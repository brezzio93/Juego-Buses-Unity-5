using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollTest : MonoBehaviour
{
    [SerializeField]
    private Text txt;

    private static List<string> RoomList = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++)
            RoomList.Add(System.Convert.ToString(i));
        foreach(string sala in RoomList)
            txt.text = sala;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
