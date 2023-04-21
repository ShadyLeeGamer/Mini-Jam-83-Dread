using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; protected set; }

    const int TILE_SIZE = 2;

    Dictionary<Room, Node[,]> NodeList;

    private void Awake()
    {
        Instance = this;
    }

    public void CreateNodes()
    {
        NodeList = new Dictionary<Room, Node[,]>();

        // Find the entrance room, our starting point
        Room workingRoom = null;
        foreach(Transform child in transform)
        {
            if(child.GetComponent<Room>() != null)
            {
                if(child.GetComponent<Room>().Type == RoomType.Entrance)
                {
                    workingRoom = child.GetComponent<Room>();
                    break;
                }
            }
        }
        if(workingRoom == null)
        {
            Debug.Log("Something went horribly wrong in level generation, there is no entrance room.");
            return;
        }

        // Start at the entrance, creating nodes for it and every connected room until reaching the end
        while (workingRoom != null)
        {
            // Decide base numbers for the nodes
            var nodeCount = (int)(workingRoom.GetSizeX() / TILE_SIZE);
            var topCorner = workingRoom.transform.position;
            topCorner.x -= workingRoom.GetSizeX() / 2;
            topCorner.z += workingRoom.GetSizeX() / 2;

            // Create enough nodes to fill the space
            NodeList.Add(workingRoom, new Node[nodeCount, nodeCount]);
            for (int x = 0; x < nodeCount; x++)
            {
                for (int y = 0; y < nodeCount; y++)
                {
                    // Create node and center it to it's space
                    var node = new Node()
                    {
                        Room = workingRoom,
                        Position = new Vector2(topCorner.x + (TILE_SIZE * x) + (TILE_SIZE / 2), topCorner.z - (TILE_SIZE * y) - (TILE_SIZE / 2))
                    };
                    NodeList[workingRoom][x, y] = node;

                    // Connect the new node to it's already created nearby nodes
                    if (x - 1 >= 0)
                    {
                        NodeList[workingRoom][x - 1, y].ConnectNode(node);
                    }

                    if (y - 1 >= 0)
                    {
                        NodeList[workingRoom][x, y - 1].ConnectNode(node);
                    }
                }
            }

            // Make obstacles act like walls
            foreach (Transform obs in workingRoom.transform.Find("Props").Find("Obstacles"))
            {
                GetClosestNode(workingRoom, obs.position).IsWall = true;
            }

            foreach (Transform monster in workingRoom.transform.Find("Props").Find("Enemies"))
            {
                monster.GetComponent<MonsterAI>().SetCurrentRoom(workingRoom);
            }

            // Move on to the next room down the chain
            workingRoom = workingRoom.NextNeighbour;
        }

        // Go through rooms and connect nodes between rooms
        foreach(var roomList in NodeList)
        {
            if (roomList.Key.NextNeighbour == null)
                continue;

            // Figure out which side the neighbor room is on
            var connectDir = new Vector2(-1, -1);
            if(roomList.Key.transform.position.x < roomList.Key.NextNeighbour.transform.position.x)
            {
                connectDir.x = 1f;
            }
            else if(roomList.Key.transform.position.x > roomList.Key.NextNeighbour.transform.position.x)
            {
                connectDir.x = 0f;
            }
            else if(roomList.Key.transform.position.z > roomList.Key.NextNeighbour.transform.position.z)
            {
                connectDir.y = 1f;
            }
            else if(roomList.Key.transform.position.z < roomList.Key.NextNeighbour.transform.position.z)
            {
                connectDir.y = 0f;
            }

            // If the room is up/down, connect the nodes
            if(connectDir.x < 0)
            {
                for (int x = 0; x < roomList.Value.GetLength(0); x++)
                {
                    // Let's just say I made it this scuffed to support differently placed doorways :)
                    if (x != roomList.Value.GetLength(0) / 2)
                        continue;

                    var node1 = roomList.Value[x, (int)(connectDir.y * (roomList.Value.GetLength(0) - 1))];
                    var node2 = NodeList[roomList.Key.NextNeighbour][x, connectDir.y == 0 ? (int)(roomList.Value.GetLength(0) - 1) : 0];
                    node1.ConnectNode(node2, 2);
                }
            }
            // If the room is left/right, connect the nodes
            else if(connectDir.y < 0)
            {
                for (int y = 0; y < roomList.Value.GetLength(1); y++)
                {
                    if (y != roomList.Value.GetLength(1) / 2)
                        continue;

                    var node1 = roomList.Value[(int)(connectDir.x * (roomList.Value.GetLength(1) - 1)), y];
                    var node2 = NodeList[roomList.Key.NextNeighbour][connectDir.x == 0 ? (int)(roomList.Value.GetLength(1) - 1) : 0, y];
                    node1.ConnectNode(node2, 2);
                }
            }
        }

        // Debug only, displays the nodes and their connections
        /*
        int count = 0;
        foreach(var x in NodeList)
        {
            foreach(var node in x.Value)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                if(node.IsWall)
                {
                    go.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 1);
                }

                go.name = count++.ToString();
                go.transform.parent = node.Room.transform;
                go.transform.position = new Vector3(node.Position.x, 1, node.Position.y);

                foreach(var connection in node.Connections)
                {
                    var lineGo = new GameObject();
                    var line = lineGo.AddComponent<LineRenderer>();
                    line.transform.parent = go.transform;
                    line.positionCount = 2;
                    line.SetPosition(0, new Vector3(go.transform.position.x, go.transform.position.y * connection.Value, go.transform.position.z));
                    line.SetPosition(1, new Vector3(connection.Key.Position.x, go.transform.position.y * connection.Value, connection.Key.Position.y));
                }
            }
        }
        */
    }

    public Node GetClosestNode(Room room, Vector3 position)
    {
        var lockedPosition = new Vector2(position.x, position.z);

        Node closestNode = NodeList[room][0,0];
        float distance = 999999;

        foreach(var x in NodeList[room])
        {
            if(Vector2.Distance(x.Position, lockedPosition) < distance)
            {
                closestNode = x;
                distance = Vector2.Distance(x.Position, lockedPosition);
            }
        }

        return closestNode;
    }

    public Node GetClosestWalkableNode(Room room, Vector3 position)
    {
        var lockedPosition = new Vector2(position.x, position.y);

        Node closestNode = NodeList[room][0, 0];
        float distance = 999999;

        foreach (var x in NodeList[room])
        {
            if (x.IsWall)
                continue;

            if (Vector2.Distance(x.Position, lockedPosition) < distance)
            {
                closestNode = x;
                distance = Vector2.Distance(x.Position, lockedPosition);
            }
        }

        return closestNode;
    }

    public Node GetNode(Room room, int x, int y)
    {
        return NodeList[room][x, y];
    }

    public List<Node> Pathfind(Node start, Node end)
    {
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();

        var frontier = new Queue<Node>();
        frontier.Enqueue(start);

        cameFrom.Add(start, null);

        Node current = null;
        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();

            foreach(var x in current.Connections)
            {
                if (x.Key.IsWall)
                    continue;

                if(!cameFrom.ContainsKey(x.Key))
                {
                    frontier.Enqueue(x.Key);
                    cameFrom.Add(x.Key, current);
                }

                if (x.Key == end)
                {
                    frontier.Clear();
                    break;
                }
            }
        }

        List<Node> path = new List<Node>();
        var pathNode = end;
        while(true)
        {
            path.Add(pathNode);

            pathNode = cameFrom[pathNode];

            if (pathNode == null)
                break;
        }

        path.Reverse();
        return path;
    }
}

public class Node
{
    public Room Room { get; set; }
    public Vector2 Position { get; set; }
    public Dictionary<Node, int> Connections;
    public bool IsWall { get; set; }

    public Node()
    {
        Connections = new Dictionary<Node, int>();
    }

    /// <summary>
    /// Connect to a node and have it connect to us
    /// </summary>
    /// <param name="node">The node to connect to</param>
    /// <param name="weight">The weight of the path between us</param>
    public void ConnectNode(Node node, int weight = 1)
    {
        if (Connections.ContainsKey(node))
            return;

        Connections.Add(node, weight);
        node.ConnectNode(this, weight);
    }
}
