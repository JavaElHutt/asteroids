using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Este script es responsable de la rotación que presentan los asteroides. 
 * Un Rigidbody (3D) tiene asociada una velocidad angular, la cual viene definida por un vector,
 * y es una medida de la velocidad de rotación.
 */
public class RandomRotator : MonoBehaviour {
	private Rigidbody rb;

    // Al utilizar una variable pública (tumble) podemos definir su valor desde el editor e ir probando
    // valores hasta conseguir el efecto esperado, en nuestro caso obtener una velocidad de giro que nos convenza.
    public float tumble;

    /**
     * Obtenemos la referencia al componente RigidBody del GameObject al que está asociado este script.
     */ 
	void Awake() {
		rb = GetComponent<Rigidbody>();
	}


	void Start() {
        // Random.Range(min, max) -> Devuelve un float comprendido entre los valores min y max, ambos inclusivos.
        // Si ambos argumentos son un entero se devuelve un entero, si ambos son float se devuelve float y
        // si uno es float y el otro es un entero se devolverá un float.

        float x = Random.Range(-1, 1);
		float y = Random.Range(-1, 1);
		float z = Random.Range(-1, 1);

        // Al utilizar el método Vector3.normalized la magnitud del vector pasa a ser 1
		Vector3 asteroidAngularVelocity = new Vector3(x, y, z).normalized;

        // Al multiplicar ahora por la variable tumble la magnitudó módulo del vector pasa a ser tumble. 
		rb.angularVelocity = asteroidAngularVelocity * tumble;

	}

}