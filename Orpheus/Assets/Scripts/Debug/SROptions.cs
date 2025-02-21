using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public partial class SROptions
{

    [Category("Camera Testing")]
    public void FocusCameraOnZeroZeroTile()
    {
        CameraController.Instance.FocusPosition(new Vector3(0, 0, 0));
    }
}
