using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class PathVisualizer : MonoBehaviour
{
    [SerializeField] List<GameObject> LineRendererPrefabs;
    List<LineRenderer> LineRenderers = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject lineRenderer in LineRendererPrefabs)
        {
            LineRenderers.Add(Instantiate(lineRenderer, transform).GetComponent<LineRenderer>());
            LineRenderers[^1].positionCount = 0;
        }

    }

    public void VisualizePath(Dictionary<Vector3, int> Path)
    {
        foreach (var lr in LineRenderers)
        {
            lr.positionCount = 0;
        }

        Dictionary<int, List<Vector3>> groupedPositions = new();

        foreach (var kvp in Path)
        {
            if (!groupedPositions.ContainsKey(kvp.Value))
                groupedPositions[kvp.Value] = new List<Vector3>();

            groupedPositions[kvp.Value].Add(kvp.Key + new Vector3(0, 0.2f, 0));
        }

        var sortedKeys = groupedPositions.Keys.OrderBy(k => k).ToList();

        for (int i = 1; i < sortedKeys.Count; i++)
        {
            int prevKey = sortedKeys[i - 1];
            int currentKey = sortedKeys[i];

            Vector3 lastPointOfPrev = groupedPositions[prevKey][^1]; 
            groupedPositions[currentKey].Insert(0, lastPointOfPrev);
        }

        foreach (var key in sortedKeys)
        {
            Vector3[] positions = groupedPositions[key].ToArray();
            LineRenderers[key].positionCount = positions.Length;
            LineRenderers[key].SetPositions(positions);    
        }

    }

    public void ClearPath(bool ConfigMode)
    {
        foreach (var lr in LineRenderers)
        {
            lr.positionCount = 0;
        }
    }

}
