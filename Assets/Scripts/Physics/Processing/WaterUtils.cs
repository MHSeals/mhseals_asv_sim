using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using Unity.Mathematics;

namespace Sim.Physics.Processing {
    public static class WaterUtils {
        public static WaterSearchResult Search(WaterSurface targetSurface, Vector3 position, float error=0.01f, int maxIterations=8, bool includeDeformers=false) {
            WaterSearchParameters searchParameters = new();
            WaterSearchResult searchResult = new();
           
            searchParameters.startPositionWS = searchResult.candidateLocationWS;
            searchParameters.targetPositionWS = (float3)position;
            searchParameters.error = error;
            searchParameters.maxIterations = maxIterations;
            searchParameters.includeDeformation = includeDeformers;
            searchParameters.excludeSimulation = false;
    
            // Can get both current and water height
            if (targetSurface.ProjectPointOnWaterSurface(searchParameters, out searchResult)) return searchResult;
            else { Debug.LogWarning("Water search failed"); return new WaterSearchResult(); }
        }
    }
}