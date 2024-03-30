using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

public static class LevelGenerator
{
    static int maxDoorsForwardNum = 3;

    public static Dictionary<LevelType, List<RoomNode.RoomType>> requiredLevelRoomNodesDict = new Dictionary<LevelType, List<RoomNode.RoomType>>
    {
        { LevelType.castle, new List<RoomNode.RoomType>{ RoomNode.RoomType.chest } },
    };

    static Dictionary<LevelType, int> levelDepthDict = new Dictionary<LevelType, int>()
    {
        { LevelType.castle, 3},
        { LevelType.prison, 4},
    };

    static Dictionary<LevelType, int> levelWidthDict = new Dictionary<LevelType, int>()
    {
        { LevelType.castle, 3},
        { LevelType.prison, 3},
    };

    static Dictionary<LevelType, int> levelMeanNumRooms = new Dictionary<LevelType, int>()
    {
        { LevelType.castle, 10},
        { LevelType.prison, 15},
    };

    public enum LevelType 
    {
        castle,
        prison,
        mines,
    }
    public class RoomNode
    {
        public enum RoomType
        {
            start,
            end,
            quickFight,
            longFight,
            bonfire,
            chest,
            requiredRoom,
            any,
            deal,//торговец
        }
        public RoomType type = RoomType.any;
        public RoomNode[] children = new RoomNode[0];
        public RoomNode[] parents = new RoomNode[0];
        public RoomNode[] additionalChildren = new RoomNode[0];
        public int depth = 0;
        public int? id = null;
        

        public void AddParent(RoomNode node)
        {
            RoomNode[] parentsTmp = new RoomNode[parents.Length + 1];
            for(int i = 0; i < parents.Length; i++)
            {
                parentsTmp[i] = parents[i];
            }
            parentsTmp[parents.Length] = node;
            parents = parentsTmp;
        }

        public void SetRequiredType(LevelType levelType)
        {
            if (requiredLevelRoomNodesDict[levelType].Count == 0)
                return;
            type = requiredLevelRoomNodesDict[levelType][Random.Range(0, requiredLevelRoomNodesDict[levelType].Count)];
            if (type != RoomType.any) requiredLevelRoomNodesDict[levelType].Remove(type);
        }
    }

    public static Dictionary<RoomNode, GameObject> GenerateLevel(LevelType type)
    {
        RoomNode startNode = new RoomNode();
        startNode.depth = 0;
        startNode.type = RoomNode.RoomType.start;
        RoomNode[] startContainer = new RoomNode[1];
        startContainer[0] = startNode;
        RecursiveGraphGeneration(type, startContainer);
        Dictionary<RoomNode, GameObject> correspondedNodes = new Dictionary<RoomNode, GameObject>();
        CorrespondNodesToRooms(startContainer, type, correspondedNodes);
        ArrangeRooms(correspondedNodes);
        DisableRooms(correspondedNodes);
        return correspondedNodes;
    }
    public static void DisableRooms(Dictionary<RoomNode, GameObject> rooms, int depth = 0)
    {
        foreach(var pair in rooms)
        {
            if(Mathf.Abs(pair.Key.depth - depth) > 0)
                pair.Value.gameObject.SetActive(false);
            else 
                pair.Value.gameObject.SetActive(true);
        }
    }
    static void RecursiveGraphGeneration(LevelType type, RoomNode[] nodes, int currentDepth = 0, int nodesTotal = 0)
    {
        if (currentDepth == levelDepthDict[type])
        {
            RoomNode endNode = new RoomNode();
            endNode.depth = currentDepth + 1;
            endNode.type = RoomNode.RoomType.end;
            int exitRoomsNum = Random.Range(1, nodes.Length > maxDoorsForwardNum ? maxDoorsForwardNum : nodes.Length);
            List<int> indexes = new List<int>();
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].depth = currentDepth;
                indexes.Add(i);
            }
            for (int i = 0; i < exitRoomsNum; i++)
            {
                int nodeIndex = Random.Range(0, indexes.Count);
                nodes[indexes[nodeIndex]].children = new RoomNode[1];
                nodes[indexes[nodeIndex]].children[0] = endNode;
                endNode.AddParent(nodes[indexes[nodeIndex]]);
                indexes.RemoveAt(nodeIndex);
            }
            return; 
        }

        List<RoomNode> nextDepthNodes = new List<RoomNode>(); 
        int maxWidth = levelWidthDict[type];
        bool connectedWithNextDepthNodes = false;
        RoomNode lastConnectedNode = new RoomNode();
        for(int n = 0; n < nodes.Length; n++)
        {
            nodes[n].depth = currentDepth;
            int connectionsNumber = Random.Range(0, maxWidth < maxDoorsForwardNum + 1 ? maxWidth + 1 : maxDoorsForwardNum + 1);
            if(connectionsNumber == 0 && !connectedWithNextDepthNodes)
            {
                if (GetRandomChance((n + 1) / nodes.Length))
                    connectionsNumber = Random.Range(1, maxWidth < maxDoorsForwardNum + 1 ? maxWidth + 1 : maxDoorsForwardNum + 1);
            }
            if (connectionsNumber == 1 && nodesTotal / (currentDepth + 1) < levelMeanNumRooms[type] / levelDepthDict[type] + 1)
            {
                if (GetRandomChance(1 - (nodesTotal / (currentDepth + 1)) / (levelMeanNumRooms[type] / levelDepthDict[type] + 1)))
                    connectionsNumber = Random.Range(2, maxWidth < maxDoorsForwardNum + 1 ? maxWidth + 1 : maxDoorsForwardNum + 1);
            }
            else if(connectionsNumber > 1 && nodesTotal / (currentDepth + 1) > levelMeanNumRooms[type] / levelDepthDict[type] + 1)
            {
                if (GetRandomChance(1 - (levelMeanNumRooms[type] / levelDepthDict[type] + 1) / (nodesTotal + connectionsNumber / (currentDepth + 2))))
                    connectionsNumber = connectedWithNextDepthNodes ? 0 : 1;
            }
            maxWidth -= connectionsNumber;
            connectedWithNextDepthNodes = connectionsNumber > 0 ? true : connectedWithNextDepthNodes;
            nodes[n].children = new RoomNode[connectionsNumber];
            
            for (int i = 0; i < connectionsNumber; i++)
            {
                if (i == 0)
                    nodes[n].children[i] = GetRandomChance(0.75f * (1 - (lastConnectedNode.parents.Length >= maxDoorsForwardNum ? 1 : 0))) ? lastConnectedNode : new RoomNode();
                else
                    nodes[n].children[i] = new RoomNode();

                if (nodes[n].children[i].type == RoomNode.RoomType.any && GetRandomChance(0.25f))
                {
                    nodes[n].children[i].SetRequiredType(type);
                }
                nodes[n].children[i].AddParent(nodes[n]);
                if (!nextDepthNodes.Contains(nodes[n].children[i]))
                    nextDepthNodes.Add(nodes[n].children[i]);

                lastConnectedNode = nodes[n].children[i];
            }
        }
        RecursiveGraphGeneration(type, nextDepthNodes.ToArray(), ++currentDepth, nodesTotal + nodes.Length);
    }

    public static Dictionary<int, RoomNode> GetIndexedRoomNodeDictionary(RoomNode[] startNodes)
    {
        Dictionary<int, RoomNode> dict = new Dictionary<int, RoomNode>();
        IndexNodes(startNodes, dict);
        return dict;
    }

    static void IndexNodes(RoomNode[] nodes, Dictionary<int, RoomNode> dict, int nextId = 0)
    {
        List<RoomNode> nextNodes = new List<RoomNode>();
        foreach (RoomNode node in nodes)
        {
            if (node.id == null)
            {
                node.id = nextId;
                nextId++;
                dict.Add((int)node.id, node);
                foreach (RoomNode child in node.children)
                {
                    if (!nextNodes.Contains(child))
                        nextNodes.Add(child);
                }
            }
        }
        if (nextNodes.Count == 0)
            return;
        else
            IndexNodes(nextNodes.ToArray(), dict, nextId);
    }

    static void IndexNodes(RoomNode[] nodes, int nextId = 0)
    {
        List<RoomNode> nextNodes = new List<RoomNode>();
        foreach (RoomNode node in nodes)
        {
            if (node.id == null)
            {
                node.id = nextId;
                nextId++;
                foreach (RoomNode child in node.children)
                {
                    if (!nextNodes.Contains(child))
                        nextNodes.Add(child);
                }
            }
        }
        if (nextNodes.Count == 0)
            return;
        else
            IndexNodes(nextNodes.ToArray(), nextId);
    }

    static void UnindexNodes(RoomNode[] nodes)
    {
        List<RoomNode> nextNodes = new List<RoomNode>();
        foreach (RoomNode node in nodes)
        {
            if (node.id != null)
            {
                node.id = null;
                foreach (RoomNode child in node.children)
                {
                    if (!nextNodes.Contains(child))
                        nextNodes.Add(child);
                }
            }
        }
        if (nextNodes.Count == 0)
            return;
        else
            UnindexNodes(nextNodes.ToArray());
    }

    static void CorrespondNodesToRooms(RoomNode[] rootNodes, LevelType type, Dictionary<RoomNode, GameObject> corresponded)
    {
        List<RoomNode> nextNodes = new List<RoomNode>();
        foreach (RoomNode node in rootNodes)
        {
            foreach (RoomNode child in node.children)
            {
                if (!nextNodes.Contains(child))
                    nextNodes.Add(child);
            }
            GameObject room = RoomManager.instance.GetRoom(type, node.type, node.parents.Length, node.children.Length);
            if (room == null) {
                room = RoomManager.instance.GetRoom(type, RoomNode.RoomType.any, node.parents.Length, node.children.Length);
            }
            room = Object.Instantiate(room);
            room.GetComponent<Room>().SetDepth(node.depth);
            corresponded.Add(node, room);
        }
        if (nextNodes.Count == 0)
            return;
        else
            CorrespondNodesToRooms(nextNodes.ToArray(), type, corresponded);

    }

    static void ArrangeRooms(Dictionary<RoomNode, GameObject> nodeRoomDict)
    {
        int x = 0;
        int y = 0;
        foreach(var pair in nodeRoomDict)
        {
            if(y != pair.Key.depth)
            {
                x = 0;
                y++;
            }
            pair.Value.transform.position = new Vector3(x * 100, 0, y * 100);
            Room[] roomsToConnectWith = nodeRoomDict.Where(x => pair.Key.children.Contains(x.Key)).Select(x => x.Value.GetComponent<Room>()).ToArray();
            int[] connectionsBackward = nodeRoomDict.Where(x => pair.Key.children.Contains(x.Key)).Select(p => p.Key.parents.Length).ToArray();

            Room room = pair.Value.GetComponent<Room>();
            room.ConnectWith(roomsToConnectWith, connectionsBackward);

            room.SpawnEnemies();

            x++;
        }
    }



    public static bool GetRandomChance(float chance)
    {
        return Random.value <= chance;
    }


}
