using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Este script sirve para dar a un objeto un movimiento linear, en una dirección.
 * Se podría utilizar (el script) para cualquier objeto al cual quisiéramos aplicarle
 * este comportamiento.
 * 
 * La variable pública speed es la que determina la velocidad a la que se desplazará
 * el objeto. Debido a que podemos modificarla desde el editor de Unity, su valor podrá ser
 * diferente para cada uno de los GameObject a los que asociemos este script.
 * 
 */ 
public class Mover : MonoBehaviour {

    public float speed;

    private Rigidbody rb;

    /**
     * En cuanto el objeto es instanciado se le aplica este comportamiento (script).
     * Observa que la dirección en la que se desplaza el objeto es forward (hacia adelante)
     * siguiendo el eje Z (azul)
     */ 
	void Awake() {
		rb = GetComponent<Rigidbody>();

        rb.velocity = transform.forward * speed;
	}

}
