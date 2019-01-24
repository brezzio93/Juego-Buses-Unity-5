using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;


public class Jugador : MonoBehaviourPunCallbacks
{
    private Image avatar;
    private string nombre;
    private bool[] pago = new bool[10];
    private bool[] llega = new bool[10];
    private int billetera;
    [SerializeField]
    private Text t_dias;

    private int dias = 0;
    float probabilidad;

    private int evasores = 0;



    // Start is called before the first frame update
    void Start()
    {
        billetera = System.Convert.ToInt32(PhotonNetwork.CurrentRoom.CustomProperties["monto"]);                
        t_dias.text = "Día "+ System.Convert.ToString(dias+1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Se añade la selección del jugador a su historial de pago y se descuenta saldo de la billetera del jugador 
    /// </summary>
    public void Pagar(bool button)
    {
        if(button) Debug.Log("Se pagó hoy");
        else        Debug.Log("No se pagó hoy");
        pago[dias] = button;
        if (pago[dias])
            billetera = billetera-System.Convert.ToInt32(PhotonNetwork.CurrentRoom.CustomProperties["precio"]);
        

    }

    public void Llegar()
    {
        llega[dias]=CalcularViaje();
        if (llega[dias])
            billetera = billetera+ System.Convert.ToInt32(PhotonNetwork.CurrentRoom.CustomProperties["ganancia"]);        
    }


    /// <summary>
    /// Calcula la probabilidad de llegar al destino en base a la cantidad de evasores totales
    /// </summary>
    public bool CalcularViaje()
    {
        probabilidad=1f;
        int i = 0, evasores = 0;
        float x = 0f;
        double uno = 1;
        while (i != dias)
        {
            if (pago[i] == false)
                evasores++;
            i++;
        }
        //probabilidad = 1 - 1 / (1 + uno ^ (13 * (x - 0.5)));
        //probabilidad=1/(1 ^ ( 13 *( x - 0.5 ) ) );
        return true;

    }

    /// <summary>
    /// Revisa que el jugador pague su pasaje y en base a eso calcula la probabilidad de llegar junto con los 
    /// calculos del pasaje de bus
    /// </summary>
    /// <param name="button"></param>
    public void OtroDia(bool button)
    {        
        if (dias < 9) {
            Pagar(button);
            Llegar();
            dias++;
            Debug.Log("Comienza Día " + (dias+1));
            Debug.Log("Saldo "+PhotonNetwork.LocalPlayer.NickName+": "+ billetera);
            t_dias.text = "Día " + System.Convert.ToString(dias+1);
        }
        else
        {
            Debug.Log("Finalizado");
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(0);
        }
    }
}
