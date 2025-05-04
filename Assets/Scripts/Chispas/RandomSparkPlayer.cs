using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSparkPlayer : MonoBehaviour
{
    ParticleSystem _mySparks;

    // Start is called before the first frame update
    void Start()
    {
        _mySparks = GetComponent<ParticleSystem>();
    }


    bool _canSpark = true;
    // Genera un evento aleatorio en el que saltarian las chispas del objeto.
    private void FixedUpdate()
    {
        var randomNumber = Random.Range(0, 100);

        if(randomNumber == 0) 
        { 
            if(_canSpark) _mySparks.Play();
            _canSpark = !_canSpark;
        }
    }
}
