using UnityEngine;

public class InfluencePositionTracker : MonoBehaviour
{
    public Influence Influence { get; set; }

    private void Update()
    {
        if(transform.position != Influence.Position)
        {
            Influence.Position = transform.position;
        }
    }
}
