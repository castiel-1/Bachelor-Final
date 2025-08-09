using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NodeToCursorPathCreationStrategy : IPathCreationStrategy
{
    public bool waitingForCursorConfirmation = false;

    private Node startNode;
    private Graph graph;
    private PathCreationTool pathCreationTool;

    public NodeToCursorPathCreationStrategy(PathCreationTool tool)
    {
        pathCreationTool = tool;
    }

    public void HandleClick(RaycastHit hitInfo)
    {

        TargetCursor cursor = TargetCursor.Instance;

        GameObject hitObject = hitInfo.collider.gameObject;
        NodeComponent nodeComponent = hitObject.GetComponent<NodeComponent>();

        // if we haven't clicked on a node we continue waiting
        if (hitObject == null || nodeComponent == null)
        {
            return;
        }

        Node currentNode = nodeComponent.Node;
        graph = nodeComponent.Graph;

        if (startNode == null)
        {
            startNode = currentNode;

            // make button appear that asks for confirmation
            RuntimeInteractionData.askForPathCreationCursorPositionConfirmation = true;

            // tell the controller that we are waiting for the cursor confirmation so we stop using left clicks in the scene
            waitingForCursorConfirmation = true;
        }
    }

    public void HandleCursorPositionConfirmation()
    {
        TargetCursor cursor = TargetCursor.Instance;

        Vector3 cursorPosition;

        if (RuntimeSettingsData.onSurface)
        {
            cursorPosition = cursor.GetSurfacePoint();
        }
        else
        {
            cursorPosition = cursor.GetCursorPosition();
        }

        Node endNode = GraphOperations.CreateNode(graph, cursorPosition);

        // create a path
        // debugging 
        Debug.Log("graph: " + graph);
        Debug.Log("startnode: " + startNode);
        Debug.Log("endNode: " + endNode);

        GraphOperations.CreatePath(graph, startNode, endNode);

        pathCreationTool.StopInteraction();

        RuntimeInteractionData.askForPathCreationCursorPositionConfirmation = false;
        startNode = null;
        graph = null;
        waitingForCursorConfirmation = false;
    }
}
