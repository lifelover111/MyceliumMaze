using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using static LevelGenerator;

public class GraphDebugger : MonoBehaviour
{
    public int graphsToGenerate = 100;


    [System.Serializable]
    public class GraphData
    {
        [XmlElement] public List<int> nodes = new List<int>();
        [XmlElement] public List<Vector2> edges = new List<Vector2>();
        public GraphData() { }
        public GraphData(Dictionary<int, RoomNode> dict)
        {
            foreach (var node in dict)
            {
                nodes.Add(node.Key);
                foreach (var child in node.Value.children)
                {
                    edges.Add(new Vector2Int((int)node.Value.id, (int)child.id));
                }
            }
        }
    }

    void Start()
    {
        LevelGenerator.Init();
        List<RoomNode[]> graphs = new List<RoomNode[]>();
        for (int i = 0; i < graphsToGenerate; i++)
        {
            graphs.Add(LevelGenerator.GenerateLevel(LevelGenerator.LevelType.prison));
        }
        DebugGraph(graphs);
    }


    static void DebugGraph(List<RoomNode[]> graphs)
    {
        GraphData[] graphDatas = new GraphData[graphs.Count];
        for (int i = 0; i < graphs.Count; i++)
        {
            Dictionary<int, RoomNode> dict = LevelGenerator.GetIndexedRoomNodeDictionary(graphs[i]);
            GraphData graphData = new GraphData(dict);
            graphDatas[i] = graphData;
        }

        string path = Path.Combine(Application.persistentDataPath, "GraphData.xml");
        Debug.Log("graphs saved at " + path);
        XmlSerializer serializer = new XmlSerializer(typeof(GraphData[]));
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, graphDatas);
        }
    }
}
