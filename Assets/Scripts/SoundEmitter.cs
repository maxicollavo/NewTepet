using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    public void DoorSound()
    {
        AudioManager.Instance.PlaySound("rocaMoviendose");
    }
}
