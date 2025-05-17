using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendActionToTorch : MonoBehaviour
{
    [SerializeField] Torch torch;

    public void OnAnimFinish()
    {
        torch.OnAnimFinish();
    }
}
