using UnityEngine;
using System.Collections.Generic;

public class PathSentenceGenerator : MonoBehaviour
{
    public static PathSentenceGenerator Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

    }

    private void OnEnable()
    {
        GraphOperations.OnPathCreated += HandlePathCreated;
        GraphOperations.OnPathRecreated += HandlePathRecreated;
    }

    private void OnDisable()
    {
        GraphOperations.OnPathCreated -= HandlePathCreated;
        GraphOperations.OnPathRecreated -= HandlePathRecreated;
    }

    public async void HandlePathCreated(Path path, Graph graph)
    {
        // get full prompt
        int numWords = RandomizeNumberOfWords(RuntimeSettingsData.numberOfWordsMin, RuntimeSettingsData.numberOfWordsMax);
        string prompt = FullPromptBuilder.BuildPrompt(graph, path, numWords, RuntimeSettingsData.historyDepth);

        // call llm
        string llmOutput = await LLMManager.Instance.PromptLLM(prompt);
        int outputLength = llmOutput.Length;

        // caluclate pathPoints
        List<Vector3> pathPointPositions = SplineCalculator.CalculateSplinePoints(path.StartNode.Position, path.EndNode.Position, outputLength);

        // add path points (which raises event to spawn them as well)
        GraphOperations.AddPathPoints(graph, path, pathPointPositions);

        Handle startHandle = HandleManager.Instance.CreateHandleOnNode(path.StartNode, path, true);

        Handle endHandle = HandleManager.Instance.CreateHandleOnNode(path.EndNode, path, false);

        // calculate sizes
        ITextSizeStrategy textSizeStrategy = TextSizeStrategyFactory.CreateTextSizeStrategy();
        float[] sizes = textSizeStrategy.GetTextSizes(outputLength);

        // add colour
        Color[] colors = ColorInfluenceCalculator.CalculateColorInfluences(pathPointPositions.ToArray());

        // caluclate line directions
        Vector3[] lineDirections = OrientationOperations.CalculateLineDirections(pathPointPositions.ToArray());

        // create buffer
        Sentence sentence = SentenceBufferManager.Instance.AddSentence(llmOutput, path.pathPoints, sizes, RuntimeSettingsData.orientation, lineDirections, colors);
        path.Sentence = sentence;

    }

    public void HandlePathRecreated(Path path, Graph graph, string sentenceText)
    {
        // caluclate pathPoints
        List<Vector3> pathPointPositions = SplineCalculator.CalculateSplinePoints(path.StartNode.Position, path.EndNode.Position, sentenceText.Length);

        // add path points (which raises event to spawn them as well)
        GraphOperations.AddPathPoints(graph, path, pathPointPositions);

        Handle startHandle = HandleManager.Instance.CreateHandleOnNode(path.StartNode, path, true);

        Handle endHandle = HandleManager.Instance.CreateHandleOnNode(path.EndNode, path, false);

        // calculate sizes
        ITextSizeStrategy textSizeStrategy = TextSizeStrategyFactory.CreateTextSizeStrategy();
        float[] sizes = textSizeStrategy.GetTextSizes(path.pathPoints.Count);

        // add colour
        Color[] colors = ColorInfluenceCalculator.CalculateColorInfluences(path.pathPoints.ToArray());

        // caluclate line directions
        Vector3[] lineDirections = OrientationOperations.CalculateLineDirections(pathPointPositions.ToArray());

        // create buffer
        Sentence sentence = SentenceBufferManager.Instance.AddSentence(sentenceText, path.pathPoints, sizes, RuntimeSettingsData.orientation, lineDirections, colors);
        path.Sentence = sentence;

    }

    private int RandomizeNumberOfWords(int min, int max)
    {
        return UnityEngine.Random.Range(min, max + 1);
    }

}
