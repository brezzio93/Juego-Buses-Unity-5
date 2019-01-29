using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollTest : MonoBehaviour
{
    [SerializeField]
    private Text txt;


    private GameObject SalaTemplate;
    private List<GameObject> RoomList;

    // Start is called before the first frame update
    void Start()
    {
        RoomList = new List<GameObject>();
        IniciarLista();        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IniciarLista()
    {
        //int count = PhotonNetwork.CountOfRooms;        
        int count = 5;

        for (int i = 0; i < count; i++)
        {
            GameObject nuevaSala = Instantiate(SalaTemplate) as GameObject;
            nuevaSala.SetActive(true);


            RoomList.Add(nuevaSala.gameObject);
        }
            
        foreach (GameObject sala in RoomList)
        {
            Debug.Log(sala);
            //txt.text = sala;
        }
            

    }
}
