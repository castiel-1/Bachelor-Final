using System.Collections.Generic;
using UnityEngine;

public static class SegmentSemanticInfluenceCalculator
{
    public static Dictionary<string, List<SemanticInfluence>> CalculateSemanticInfluenceStrengths(Path path, List<SemanticInfluence> influences)
    {
        Vector3 p0 = path.StartNode.Position;
        Vector3 p3 = path.EndNode.Position;

        Vector3 dir = p3 - p0;

        Vector3 p1 = p0 + (1f/3f) * dir;
        Vector3 p2 = p0 + (2f/3f) * dir;

        // path segments
        Dictionary<string, (Vector3 start, Vector3 end)> segments = new()
        {
            { "beginning", (p0, p1) },
            { "middle",    (p1, p2) },
            { "end",       (p2, p3) }
        };

        // output dictionary
        Dictionary<string, List<SemanticInfluence>> result = new()
        {
            { "beginning", new List<SemanticInfluence>() },
            { "middle",    new List<SemanticInfluence>() },
            { "end",       new List<SemanticInfluence>() }
        };

        foreach(var segment in segments)
        {
            Vector3 segStart = segment.Value.start;
            Vector3 segEnd = segment.Value.end;

            foreach(SemanticInfluence influence in influences)
            {
                Vector3 closestPoint = CalculateClosestPointToLineSegment(segStart, segEnd, influence.Position);
                float distance = (closestPoint - influence.Position).magnitude;

                if (distance <= influence.Radius)
                {
                    result[segment.Key].Add(influence);
                }
            }
        }

        return result;
        
    }

    private static Vector3 CalculateClosestPointToLineSegment(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 lineDir = lineEnd - lineStart;
        Vector3 startToPoint = point - lineStart;


        // t = how far along the line in percent the closest point is -> between 0 and 1 means it's on the line
        float t = Vector3.Dot(lineDir, startToPoint) / lineDir.sqrMagnitude;

        if(t < 0 || t > 1)
        {
            return lineStart;
        }

        Vector3 closestPoint = lineStart + t * lineDir;

        return closestPoint;
    }
}
