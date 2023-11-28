using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

public static class LevelGenerator
{
    static OldProject.Room[,] _level;
    static public OldProject.Room[,] level { get { return _level; } private set { _level = value; } }
    static OldProject.Room _startRoom;
    static public OldProject.Room startRoom { get { return _startRoom; } private set { _startRoom = value; } }

    static int intSeed;
    static public int numRoomsX;
    static public int numRoomsY;
    static public int roomWidth = 21;
    static public int roomHeight = 15;

    static int minWidthInRooms = 5;
    static int minHeightInRooms = 5;
    static int maxWidthInRooms = 10;
    static int maxHeightInRooms = 10;

    static public int H;
    static public int W;

    static public Vector2[] environmentsCenters = { new Vector2(5f, 4.5f), new Vector2(5f, 9.5f), new Vector2(16f, 4.5f), new Vector2(16f, 9.5f), new Vector2(10.5f, 7f) };

    public static void Init(string seed = null)
    {
        if (seed != null)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(seed);
            if (System.BitConverter.IsLittleEndian)
            {
                if (bytes.Length < 4)
                {
                    byte[] newBytes = new byte[4];
                    System.Array.Copy(bytes, 0, newBytes, newBytes.Length - bytes.Length, bytes.Length);
                    bytes = newBytes;
                }
                System.Array.Reverse(bytes);
            }
            intSeed = System.BitConverter.ToInt32(bytes, 0);
            Random.InitState(intSeed);
        }


        numRoomsX = Random.Range(minWidthInRooms, maxWidthInRooms);
        numRoomsY = Random.Range(minHeightInRooms, maxHeightInRooms);


        W = numRoomsX*roomWidth;
        H = numRoomsY*roomHeight;
    }

    public static OldProject.Room[,] GenerateLevel()
    {
        level = new OldProject.Room[numRoomsX, numRoomsY];
        for (int x = 0; x < numRoomsX; x++)
            for (int y = 0; y < numRoomsY; y++)
                level[x, y] = new OldProject.Room(x, y);
        Stack<OldProject.Room> stack = new Stack<OldProject.Room>();
        OldProject.Room current = level[Random.Range(0, numRoomsX), Random.Range(0, numRoomsY)];
        current.visited = true;
        while (true)
        {
            List<OldProject.Room> neighbors = new List<OldProject.Room>();
            for (int i = -1; i < 2; i += 2)
            {
                if (current.x + i >= 0 && current.x + i < numRoomsX)
                {
                    if (!level[current.x + i, current.y].visited)
                        neighbors.Add(level[current.x + i, current.y]);
                }
                if (current.y + i >= 0 && current.y + i < numRoomsY)
                {
                    if (!level[current.x, current.y + i].visited)
                        neighbors.Add(level[current.x, current.y + i]);
                }
            }
            if(neighbors.Count > 0)
            {
                OldProject.Room next = neighbors[Random.Range(0, neighbors.Count)];
                current.Connect(next);
                stack.Push(current);
                current = next;
                current.visited = true;
            }
            else if (stack.Count > 0)
            {
                current = stack.Pop();
            }
            else
            {
                break;
            }
        }

        startRoom = level[Random.Range(0, numRoomsX), Random.Range(0, numRoomsY)];
        startRoom.SetType(OldProject.Room.eRoomType.rest);
        foreach (OldProject.Room room in level)
            room.visited = false;


        SetRoomsDepth(startRoom, 0);

        foreach (OldProject.Room room in level)
            room.visited = false;
        
        SetRoomsRest(startRoom, 0);

        return level;
    }

    static void SetRoomsDepth(OldProject.Room room, int depth)
    {
        room.depth = depth;
        room.visited = true;
        foreach(OldProject.Room.eDoorLoc doorLoc in room.doorLocs)
        {
            Vector2 bypassDir = new Vector2(Mathf.Cos(Mathf.PI/2*(int)doorLoc), Mathf.Sin(Mathf.PI/2 * (int)doorLoc));
            if (!level[room.x + Mathf.RoundToInt(bypassDir.x), room.y + Mathf.RoundToInt(bypassDir.y)].visited)
                SetRoomsDepth(level[room.x + Mathf.RoundToInt(bypassDir.x), room.y + Mathf.RoundToInt(bypassDir.y)], depth + 1);
        }
    }

    static void SetRoomsRest(OldProject.Room room, float chance)
    {
        room.visited = true;
        float nxtChance = chance + 0.025f;
        if (room.type == null)
        {
            if (GetRandomChance(nxtChance))
            {
                room.type = OldProject.Room.eRoomType.rest;
                nxtChance = 0;
            }
            else
                room.type = OldProject.Room.eRoomType.fight;
        }
        foreach (OldProject.Room.eDoorLoc doorLoc in room.doorLocs)
        {
            Vector2 bypassDir = new Vector2(Mathf.Cos(Mathf.PI / 2 * (int)doorLoc), Mathf.Sin(Mathf.PI / 2 * (int)doorLoc));
            if (!level[room.x + Mathf.RoundToInt(bypassDir.x), room.y + Mathf.RoundToInt(bypassDir.y)].visited)
                SetRoomsRest(level[room.x + Mathf.RoundToInt(bypassDir.x), room.y + Mathf.RoundToInt(bypassDir.y)], nxtChance);
        }
    }


    public static bool GetRandomChance(float chance)
    {
        return Random.value <= chance;
    }



    // Новый код
    static int maxDoorsForwardNum = 3;
    public static Dictionary<LevelType, List<RoomNode.RoomType>> requiredLevelRoomNodesDict = new Dictionary<LevelType, List<RoomNode.RoomType>>
    {
        { LevelType.prison, new List<RoomNode.RoomType>{ RoomNode.RoomType.chest } },
    };

    static Dictionary<LevelType, int> levelDepthDict = new Dictionary<LevelType, int>()
    {
        { LevelType.prison, 3},
        { LevelType.mines, 4},
    };

    static Dictionary<LevelType, int> levelWidthDict = new Dictionary<LevelType, int>()
    {
        { LevelType.prison, 3},
        { LevelType.mines, 3},
    };

    static Dictionary<LevelType, int> levelMeanNumRooms = new Dictionary<LevelType, int>()
    {
        { LevelType.prison, 10},
        { LevelType.mines, 15},
    };

    public enum LevelType 
    {
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

    public static RoomNode[] GenerateLevel(LevelType type)
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
        return startContainer;
    }
    
    static void RecursiveGraphGeneration(LevelType type, RoomNode[] nodes, int currentDepth = 0, int nodesTotal = 0)
    {
        if (currentDepth == levelDepthDict[type])
        {
            RoomNode endNode = new RoomNode();
            endNode.depth = currentDepth + 1;
            endNode.type = RoomNode.RoomType.end;
            int exitRoomsNum = Random.Range(1, nodes.Length + 1);
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
            pair.Value.transform.position = new Vector3(x * 100 , y * 100, 0);
            Room[] roomsToConnectWith = nodeRoomDict.Where(x => pair.Key.children.Contains(x.Key)).Select(x => x.Value.GetComponent<Room>()).ToArray();
            int[] connectionsBackward = nodeRoomDict.Where(x => pair.Key.children.Contains(x.Key)).Select(p => p.Key.parents.Length).ToArray();
            pair.Value.GetComponent<Room>().ConnectWith(roomsToConnectWith, connectionsBackward);
            x++;
        }
    }
}
