using System;
using System.Collections.Generic;
using Esper.ESave.SavableObjects;
using UnityEngine;

[Serializable]
public class PathSaveData
{
    public int startNodeID;
    public int endNodeID;
    public List<HandleSaveData> handles;
    public string sentenceText;
    public SavableVector[] letterColors;
}
