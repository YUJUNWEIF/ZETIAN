using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FXScaling : MonoBehaviour
{
    public void OnWillRenderObject()
    {
        var particle = GetComponent<Renderer>() as ParticleSystemRenderer;        
        if (particle != null)
        {
            var mat = particle.material;
            //if (mat.HasProperty("_Center")) 
            {
                mat.SetVector("_Center", transform.position); 
            }
            //if (mat.HasProperty("_Scaling")) 
            {
                mat.SetVector("_Scaling", transform.lossyScale); 
            }
            //if (mat.HasProperty("_Camera")) 
            { 
                mat.SetMatrix("_Camera", Camera.current.worldToCameraMatrix); 
            }
            //if (mat.HasProperty("_CameraInv")) 
            { 
                mat.SetMatrix("_CameraInv", Camera.current.worldToCameraMatrix.inverse); 
            }
        }
    }
}
