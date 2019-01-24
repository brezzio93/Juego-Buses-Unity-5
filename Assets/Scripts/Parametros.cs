using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(InputField))]


public class Parametros : MonoBehaviour {

    [SerializeField]
    private InputField nombre;
    [SerializeField]
    private InputField monto;
    [SerializeField]
    private InputField precio;
    [SerializeField]
    private InputField ganancia;
    [SerializeField]
    private InputField cantidad;
    [SerializeField]
    private Text txt;

    public struct Params
    {
        public string nombre, monto, precio, ganancia, cantidad;
        //public byte cantidad;

        public Params(string p1, string p2, string p3, string p4, string p5)
        {
            nombre = p1;
            monto = p2;
            precio = p3;
            ganancia =p4;
            cantidad = p5;
        }
    }

    static public Params param;



    // Use this for initialization
    void Awake () {
        DontDestroyOnLoad(this);
    }
    
    // Update is called once per frame
    void Update () {
        		
	}

    public void setGetInputs()
    {
        param.nombre = PhotonNetwork.LocalPlayer.NickName;
        param.monto = monto.text;
        param.precio = precio.text;
        param.ganancia = ganancia.text;
        param.cantidad = cantidad.text;

        int cant = System.Convert.ToInt32(Parametros.param.cantidad);
        if (cant > 20)
            txt.text = "Advertencia: La Sala no puede tener más de 20 Jugadores";

    }
}
