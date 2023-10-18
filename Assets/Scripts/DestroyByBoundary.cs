using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Los Boundaries se definen como los límites del espacio de juego. Se sitúan fuera del
 * terreno o espacio de juego (lo que el jugador ve).
 * 
 * El GameObject BoundaryDestroyer está formado por 4 objetos hijos: Up, Down, Left y Right.
 * Estos hijos son GameObjects muy simples, ya que sólo presentan como componentes:
 *  - Transform: para definir básicamente su posición espacial y escala.
 *  - Collider: que actúa como un trigger, ya que buscamos que Unity nos informe cuando un objeto
 *    entre dentro de estos límites (o boundaries)
 *    
 * Nota: Hay que diferenciar este límite espacial (BoundaryDestroyer) de otro límite que también
 * se define en el juego, el BoundaryScreenWrapper, cuya misión es definir el espacio de juego o la
 * zona donde "suceden las cosas".
 */
public class DestroyByBoundary : MonoBehaviour {

    /**
     * Cuando un objeto intenta atravesar los límites del juego lo destruimos. De esta
     * forma eliminamos todo aquello que ya no va a poder interactuar con el jugador,
     * como por ejemplo asteroides que han salido del terreno de juego y se marchan 
     * "hacia el espacio exterior".
     * 
     * Es una forma de liberar memoria y de limpiar aquello que ya no nos es "útil" para
     * nuestro juego.
     */ 
    void OnTriggerEnter(Collider other) {
		Destroy(other.gameObject);
    }
}
