using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketballController : MonoBehaviour {
//Velocidad de movimiento del jugador
    public float MoveSpeed = 10;
    //Pelota
        public Transform Ball;
        //Posicion de la pelota al driblear
    public Transform PosDribble;
    //Posicion de la pelota encima de la cabeza
    public Transform PosOverHead;
    //referencia a los brazos del jugador
    public Transform Arms;
    //Hacia donde se lanzala pelota
    public Transform Target;

    // Si el jugador tiene la pelota
    private bool IsBallInHands = true;
    //El jugador tiene la pelota en las manos
    private bool IsBallFlying = false;
    //tiempo transcurrido desde que se lanzo la pelota
    private float T = 0;

    // Actualiza
    void Update() {

        // Movimiento del jugador 
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        transform.position += direction * MoveSpeed * Time.deltaTime;
        //Actualiza la posicion del jugador
        transform.LookAt(transform.position + direction);

        // El jugador tiene la pelota en las manos
        if (IsBallInHands) {

            // Sostener la pelota por encima de la cabeza
            if (Input.GetKey(KeyCode.Space)) {
                Ball.position = PosOverHead.position;
                //Mueve la pelota a la posición sobre la cabeza
                Arms.localEulerAngles = Vector3.right * 180;

                //  Mirar hacia el objetivo
                transform.LookAt(Target.parent.position);
            }

            // Driblar la pelota
            else {
                Ball.position = PosDribble.position + Vector3.up * Mathf.Abs(Mathf.Sin(Time.time * 5));
                Arms.localEulerAngles = Vector3.right * 0;
            }

            //Lanzar la pelota con la tecla espacio
            if (Input.GetKeyUp(KeyCode.Space)) {
            //El jugador ya no tiene la pelota en las manos
                IsBallInHands = false;
            //La pelota ahora está volando
                IsBallFlying = true;
                //Reinicia el tiempo transcurrido
                T = 0;
            }
        }

        //Si la pelota está en el aire
        if (IsBallFlying) {
            // Incrementa el tiempo transcurrido
            T += Time.deltaTime;
            float duration = 0.66f;
            // Normaliza el tiempo entre 0 y 1
            float t01 = T / duration;

            //Movimiento de la pelota hacia el objetivo
            Vector3 A = PosOverHead.position;
            Vector3 B = Target.position;
            Vector3 pos = Vector3.Lerp(A, B, t01);

            //Movimiento en arco para simular un lanzamiento
            Vector3 arc = Vector3.up * 5 * Mathf.Sin(t01 * 3.14f);

            Ball.position = pos + arc;

            // Verifica si la pelota ha llegado al objetivo
            if (t01 >= 1) {
                IsBallFlying = false;
                Ball.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }
    // Habilita la física de la pelota al aterrizar
    private void OnTriggerEnter(Collider other) {

        // El jugador recoge la pelota nuevamente
        if (!IsBallInHands && !IsBallFlying) {

            IsBallInHands = true;
            // Desactiva la física de la pelota
            Ball.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
