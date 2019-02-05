using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Jugador : MonoBehaviourPunCallbacks
{
    private Image avatar;
    private string nombre;
    private bool[] pago = new bool[10];
    private int billetera;

    [SerializeField]
    private Text t_dias;

    private int dias = 0;
    private float probabilidad;

    private int evasores = 0;

    // Start is called before the first frame update
    private void Start()
    {
        billetera = System.Convert.ToInt32(PhotonNetwork.CurrentRoom.CustomProperties["monto"]);
        t_dias.text = "Día " + System.Convert.ToString(dias + 1);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    /// <summary>
    /// Revisa que el jugador pague su pasaje y en base a eso calcula la probabilidad de llegar junto con los
    /// calculos del pasaje de bus
    /// </summary>
    /// <param name="button"></param>
    public void OtroDia(bool button)
    {
        if (dias < 9)
        {
            Pagar(button);
            Llegar();
            dias++;
            Debug.Log("Comienza Día " + (dias + 1));
            Debug.Log("Saldo " + PhotonNetwork.LocalPlayer.NickName + ": " + billetera);
            t_dias.text = "Día " + System.Convert.ToString(dias + 1);
        }
        else
        {
            Debug.Log("Finalizado");
            PhotonNetwork.LeaveRoom();            
            SceneManager.LoadScene(0);
        }
    }

    /// <summary>
    /// Se añade la selección del jugador a su historial de pago y se descuenta saldo de la billetera del jugador
    /// </summary>
    public void Pagar(bool button)
    {
        if (button) Debug.Log("Se pagó hoy");
        else Debug.Log("No se pagó hoy");
        //pago[dias] = button;
        PhotonNetwork.LocalPlayer.CustomProperties["paga"] = button;
        if (PhotonNetwork.LocalPlayer.CustomProperties["paga"].Equals(true))
            billetera = billetera - System.Convert.ToInt32(PhotonNetwork.CurrentRoom.CustomProperties["precio"]);
    }

    public void Llegar()
    {
        PhotonNetwork.LocalPlayer.CustomProperties["llega"] = CalcularViaje();
        if (PhotonNetwork.LocalPlayer.CustomProperties["llega"].Equals(true))
            billetera = billetera + System.Convert.ToInt32(PhotonNetwork.CurrentRoom.CustomProperties["ganancia"]);
    }

    /// <summary>
    /// Calcula la probabilidad de llegar al destino en base a la cantidad de evasores totales
    /// </summary>
    public bool CalcularViaje()
    {
        return true;
    }
}