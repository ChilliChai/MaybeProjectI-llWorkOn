using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class FitToWaterSurface : MonoBehaviour
{
    public WaterSurface targetSurface = null;

    // Internal search params
    WaterSearchParameters searchParameters = new WaterSearchParameters();
    WaterSearchResult searchResult = new WaterSearchResult();

    void Update()
    {
        if (targetSurface != null)
        {
            // Construct search parameters using the correct "WS" (world space) members
            searchParameters.startPositionWS = searchResult.candidateLocationWS;
            searchParameters.targetPositionWS = transform.position;
            searchParameters.error = 0.01f;
            searchParameters.maxIterations = 8;

            // Perform search using the right method
            if (targetSurface.ProjectPointOnWaterSurface(searchParameters, out searchResult))
            {
                // Use the projectedPositionWS returned in the result
                transform.position = searchResult.projectedPositionWS;
            }
            else
            {
                Debug.LogError("Can't Find Projected Position");
            }
        }
    }
}

