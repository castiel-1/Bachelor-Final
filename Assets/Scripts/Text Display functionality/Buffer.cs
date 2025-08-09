using UnityEngine;
using UnityEngine.VFX;

// struct for each letter holding information for displaying it

[VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
public struct LetterStruct
{
    public int fIndex;
    public Vector3 position;
    public Vector3 normal;
    public Vector3 lineDirection;
    public float size;
    public Color color;
}

public class Buffer : MonoBehaviour
{
    public static Buffer Instance { get; private set; }

    public VisualEffect visualEffect;

    private GraphicsBuffer graphicsBuffer;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        SetUpBuffer(10000);
    }

    // create graphics buffer
    public void SetUpBuffer(int numLetterStructs)
    {
        graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, numLetterStructs,
            System.Runtime.InteropServices.Marshal.SizeOf(typeof(LetterStruct)));

        visualEffect.SetGraphicsBuffer("LetterBuffer", graphicsBuffer);

    }

    // adds a sentence to the buffer beginning at startIndex
    public void AddSentenceToBuffer(LetterStruct[] letterStructs, Sentence sentence, Vector3 size) // size for bounding box of vfx graph
    {
        graphicsBuffer.SetData(letterStructs, 0, sentence.StartIndex, sentence.Text.Length);

        visualEffect.SetVector3("Size", size);

        visualEffect.Reinit();
    }

    // deletes sentence from buffer (this leaves a hole in the memory which is not being dealt with so far)
    public void DeleteSentenceFromBuffer(Sentence sentence)
    {
        LetterStruct[] emptyLetters = new LetterStruct[sentence.Text.Length];
        graphicsBuffer.SetData(emptyLetters, 0, sentence.StartIndex, sentence.Text.Length);

        visualEffect.Reinit();
    }

    private void OnDestroy()
    {
        graphicsBuffer.Release();
    }
}
