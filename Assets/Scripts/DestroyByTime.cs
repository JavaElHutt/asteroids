using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Este script representa el tiempo de vida de un objeto.
 * Transcurrido el tiempo especificado en la variable lifeTime el objeto se eliminará,
 * liberando memoria y optimizando así los recursos consumidos por el juego.
 * 
 */ 
public class DestroyByTime : MonoBehaviour {
	public float lifeTime;

    void Start() {
		Destroy(gameObject, lifeTime);
    } 
}
