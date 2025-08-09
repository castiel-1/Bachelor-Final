using Esper.ESave.SavableObjects;
using Esper.ESave;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public static class GraphSaveAndLoad 
{
    public static void SaveGraph(Graph graph, string graphSaveKey, SaveFile saveFile)
    {
        GraphSaveData graphSaveData = ConvertToGraphSaveData(graph);

        Debug.Log("graph save data: " + graphSaveData);

        saveFile.AddOrUpdateData(graphSaveKey, graphSaveData);
        saveFile.Save();
    }

    private static GraphSaveData ConvertToGraphSaveData(Graph graph)
    {
        GraphSaveData graphSaveData = new GraphSaveData();

        // nodeID
        graphSaveData.NodeID = graph.NodeID;

        // convert list of nodes
        List<NodeSaveData> nodes = new();

        foreach (Node node in graph.Nodes)
        {
            NodeSaveData nodeSaveData = new NodeSaveData();
            nodeSaveData.ID = node.ID;
            nodeSaveData.position = node.Position;

            nodes.Add(nodeSaveData);
        }

        graphSaveData.Nodes = nodes;

        // convert list of paths
        List<PathSaveData> paths = new();

        foreach (Path path in graph.Paths)
        {
            PathSaveData pathSaveData = new PathSaveData();
            pathSaveData.startNodeID = path.StartNode.ID;
            pathSaveData.endNodeID = path.EndNode.ID;

            List<HandleSaveData> savableHandles = new();

            foreach (Handle handle in path.HandlesWithIndeces.Keys)
            {
                int handleIndex = path.HandlesWithIndeces[handle];

                // without adding handles on start and end node
                if ((handleIndex != 0) && (handleIndex != path.pathPoints.Count))
                {
                    HandleSaveData savableHandle = new HandleSaveData();
                    savableHandle.position = handle.Position;
                    savableHandle.index = path.HandlesWithIndeces[handle];
                    savableHandles.Add(savableHandle);
                }
            }

            pathSaveData.handles = savableHandles;

            pathSaveData.sentenceText = path.Sentence.Text;

            paths.Add(pathSaveData);
        }

        graphSaveData.Paths = paths;

        return graphSaveData;
    }

    public static void LoadGraph(string graphSaveKey, SaveFile saveFile)
    {
        GraphSaveData graphSaveData = saveFile.GetData<GraphSaveData>(graphSaveKey);

        // create graph
        Graph graph = GraphManager.Instance.RecreateGraph();

        // create paths
        Dictionary<int, Node> nodesByID = new();
        foreach(NodeSaveData nodeSaveData in graphSaveData.Nodes)
        {
            Node node = GraphOperations.CreateNode(graph, nodeSaveData.position);
            node.ID = nodeSaveData.ID;
            nodesByID.Add(nodeSaveData.ID, node);
        }

        foreach(PathSaveData path in graphSaveData.Paths)
        {
            // start and end node
            Node startNode = nodesByID[path.startNodeID];
            Node endNode = nodesByID[path.endNodeID];


            Path recreatedPath = GraphOperations.RecreatePath(graph, startNode, endNode, path.sentenceText);

            // handles
            List<Handle> recreatedHandles = new();

            // recreate the handles
            foreach(HandleSaveData handle in path.handles)
            {
                Handle recreatedHandle = HandleManager.Instance.CreateHandleOnPath(recreatedPath.pathPoints[handle.index], recreatedPath, handle.index);
                recreatedHandles.Add(recreatedHandle);
            }

            // move the handles
            for (int j = 0; j < recreatedHandles.Count; j++)
            {
                Handle handle = recreatedHandles[j];
                Vector3 position = path.handles[j].position;
                HandleManager.Instance.MoveRecreatedHandleOnPath(handle, position, recreatedPath);
            }

        }
    }
}
