using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Boundary:	Límite que creamos con Colliders (colisionadores), a los cuales marcamos como trigger (disparadores). 
 *				Detectarán el paso de objetos gracias a la detección de las colisiones que nos proporciona Unity a través 
 *				de los Colliders.
 *				
 * Colocamos un Boundary fuera de los límites del área de juego (lo que ve el jugador).
 * Cuando se superan esos límites ligeramente, el Boundary detecta la colisión y toma medidas al respecto, ¿donde?...en los métodos
 * que Unity nos habilita para ello: 
 *		Collider.OnTriggerXXX si el Collider está marcado como trigger o bien Collider.OnCollisionXXX si no lo está.
 *		
 * Queremos teletransportar nuestra nave de un lado de la pantalla al contrario, es decir, si la nave alcanza el Boundary superior
 * teletransportamos a la nave al Boundary inferior y viceversa. Y haremos lo mismo si la nave toca uno de los lados, teletransportándola 
 * al lado contrario.
 * 
 * PROBLEMA: No podemos teletransportar de un Boundary a su opuesto tal cual, ya que si la nave entra con un ángulo 0º, es decir, está alineada con el Boundary
 * se quedará "enganchada" y no podrá salir.
 * 
 * SOLUCIÓN: Para evitar que la nave se quede "enganchada" la teletransportaremos a una posición ligeramente anterior a la que le correspondería. Este valor,
 * ligeramente anterior, es el que definimos como offset en el método auxiliar AplicarOffset.
 *
 */
public class ScreenWrapping : MonoBehaviour {
	private float boundaryPosX; //  22;
	private float boundaryPosZ; //  17;

    /**
     * Inicializamos las coordenadas del objeto hijo de BoundaryScreenWrapper (Up, Down, Left, Right) que lleva asociado
     * este script. Sólo nos interesa la coordenada X (eje Horizontal) y la coordenada Z (eje Vertical), ya que realizaremos
     * el teletransporte de izquierda a derecha y de arriba a abajo, o viceversa.
     */
    void Start() {
		boundaryPosX = gameObject.transform.position.x;
		boundaryPosZ = gameObject.transform.position.z;
	}

    /** 
     * Los Colliders deben entenderse como un volumen que delimitamos, generalmente alrededor de un objeto.
     * Si queremos controlar un espacio de la zona de juego entonces marcaremos como isTrigger el Collider.
     * Los hijos de BoundaryScreenWrapper (Up, Down, Left, Right) actuan así como disparadores y Unity nos 
     * informará cuando otro objeto contacte con ellos.
     * 
     * Sólo nos interesa detectar cuando la nave del jugador entra en contacto con alguno de los hijos de
     * BoundaryScreenWrapper, y cuando esto suceda, teletransportamos la nave hasta el otro extremo de la pantalla.
     */
    void OnTriggerEnter(Collider other) {

		if (other.tag == "Player") {
			GameObject player = other.gameObject;
			player.transform.position = Teletransportation(player.transform.position);		
		}
	}

    /**
     * Este método recive como parámetro de entrada las coordenadas de posición de la nave del jugador y
     * devuelve unas nuevas coordenadas de posición, es decir, provocamos un salto en el espacio de la nave.
     * 
     * Nota: Fíjate en que las coordenadas de los hijos de BoundaryScreenWrapper son siempre del tipo:
     *     Position  X = 0, Y = 0, Z = valor
     * o bien:
     *     Position  X = valor, Y = 0, Z = 0
     *     
     *     Por eso lo primero que hacemos en el método es comprobar el valor X ó Z de la coordenada del
     *     Boundary (es decir, del hijo de BoundaryScreenWrapper), lo que nos dice en qué zona se encuentra
     *     la nave, y por lo tanto hacia donde debemos teletransportarla (arriba, abajo, izquierda ó derecha)
     * 
     */
    private Vector3 Teletransportation(Vector3 playerPos) {
		Vector3 newPlayerPos = Vector3.zero;
		float coordinate = 0.0f;

        // El Boundary es Up o Down
		if (boundaryPosX != 0) {
            coordinate = playerPos.x;
            newPlayerPos = new Vector3(getNewCoordinate(coordinate), 0.0f, playerPos.z);
		}

        // El Boundary es Left o Right
		if (boundaryPosZ != 0) {
            coordinate = playerPos.z;
            newPlayerPos = new Vector3(playerPos.x, 0.0f, getNewCoordinate(coordinate));
		}

		return newPlayerPos;
	}

    /**
     * Método auxiliar para reducir y simplificar el método Teletransportation.
     */
    private float getNewCoordinate(float coordinate)
    {
        int offset = 1;

        // 1º) Aplicamos el offset
        if (coordinate < 0)
        {
            coordinate = coordinate + offset;
        }
        else {
            coordinate = coordinate - offset;
        }

        // 2º) Invertimos el signo de la coordenada, para enviarlo al otro extremo de la pantalla
        coordinate = -1 * coordinate;

        return coordinate;
    }

}
