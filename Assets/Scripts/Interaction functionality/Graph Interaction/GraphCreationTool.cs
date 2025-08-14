using UnityEngine;

public class GraphCreationTool : ISceneInteractionTool
{
    private Vector3 startPosition;
    private Vector3 endPosition;

    private bool hasStartPosition = false;

    public void StartInteraction()
    {
        RuntimeInteractionData.isCreatingGraph = true;
    }

    public void StopInteraction()
    {
        RuntimeInteractionData.isCreatingGraph = false;
        hasStartPosition = false;
    }

    public void HandleCursorConfirmation()
    {
        if(!hasStartPosition)
        {
            if (RuntimeSettingsData.onSurface)
            {
                startPosition = TargetCursor.Instance.GetSurfacePoint();
            }
            else
            {
                startPosition = TargetCursor.Instance.GetCursorPosition();
            }
            hasStartPosition = true;
        }
        else
        {
            if(RuntimeSettingsData.onSurface)
            {
                endPosition = TargetCursor.Instance.GetSurfacePoint();
            }
            else
            {
                endPosition = TargetCursor.Instance.GetCursorPosition();
            }
            GraphManager.Instance.CreateGraph(startPosition, endPosition);
            
            StopInteraction();
        }
    }
}
