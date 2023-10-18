using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Cuando el láser del jugador se encuentra con el del ovni se destruyen ambos disparos.
 * Sólo se añade este script al láser del jugador para reducir la complejidad, basta con que
 * un tipo de disparo (el del jugador) se "de cuenta" de que ha chocado con un disparo enemigo.
 */
public class LaserShock : MonoBehaviour {
    public GameObject explosion;

    void OnCollisionEnter(Collision collisionInfo) {

        GameObject other = collisionInfo.collider.gameObject;

        if (other.tag == "BoltOvni") {
            // Creamos una explosión en el lugar del choque de los láser
            Instantiate(explosion, transform.position, transform.rotation);
            Destroy(other); // Eliminamos el disparo del ovni
            Destroy(gameObject); // destruimos el disparo del jugador
        }

    }

}
