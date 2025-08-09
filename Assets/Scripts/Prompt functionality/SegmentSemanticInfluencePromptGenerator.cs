using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public static class SegmentSemanticInfluencePromptGenerator 
{
    public static string CalculateInfluencePrompt(Dictionary<string, List<SemanticInfluence>> influencesPerSegment)
    {
        string prompt = "";

        foreach(string segment in influencesPerSegment.Keys)
        {
            int numInfluencesPerSegment = influencesPerSegment[segment].Count;

            if (numInfluencesPerSegment == 0) 
            { 
                continue;
            }

            string segmentPrompt = "The " + segment + " of the sentence should be influenced by";

            if (numInfluencesPerSegment == 1)
            {
                segmentPrompt += " '" + influencesPerSegment[segment][0].PromptModifier + "'. ";
            }
            else if(numInfluencesPerSegment == 2)
            {
                segmentPrompt += " '" + influencesPerSegment[segment][0].PromptModifier + "' and" + " '" + influencesPerSegment[segment][1].PromptModifier + "'. ";
            }
            else
            {
                for (int i = 0; i < numInfluencesPerSegment; i++)
                {
                    string part = "'" + influencesPerSegment[segment][i].PromptModifier + "'";

                    // if it is the last influence in the list
                    if (i == numInfluencesPerSegment - 1)
                    {
                        segmentPrompt += " and " + part + ". ";
                    }
                    else
                    {
                        segmentPrompt += ", " + part;
                    }
                }
            }

            prompt += segmentPrompt;
        }
        return prompt;
    }

}
