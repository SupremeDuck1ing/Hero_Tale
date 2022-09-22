using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeroesTaleSave
{
    public List<float> playerPosition = new List<float>();
    public List<float> playerAngles = new List<float>();
    public List<float> cameraPosition = new List<float>();
    public List<float> cameraAngles = new List<float>();

    // public List<int> livingTargetPositions = new List<int>();
    // public List<int> livingTargetsTypes = new List<int>();
    // public int hits = 0;
    // public int shots = 0;
}
