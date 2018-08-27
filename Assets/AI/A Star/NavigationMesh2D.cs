using System;
using System.Collections.Generic;
using UnityEngine;


public enum NavDirection
{
    Top, Left, Bottom, Right, TopLeft, TopRight, BottomRight, BotttomLeft,
}

[ExecuteInEditMode]
public class NavigationMesh2D : MonoBehaviour
{

    static List<NavigationMesh2D> maps = new List<NavigationMesh2D>();
    public static NavigationMesh2D FindMap (Vector2 point)
    {
        foreach (var map in maps)
        {
            Vector2 halfSize = map.mapSize * .5f;

            if (point.x >= map.transform.position.x - halfSize.x && point.x <= map.transform.position.x + halfSize.x &&
                point.y >= map.transform.position.y - halfSize.y && point.y <= map.transform.position.y + halfSize.y)
            {
                return map;
            }
        }
        return null;
    }

    public Vector2 mapSize = Vector2.one; // Actual map size
    public float nodeDistance = 1f; // Distance beetween horizontal and vertical nodes

    [HideInInspector]
    [SerializeField]
    int width, height;

    [HideInInspector]
    [SerializeField]
    Node[] nodes = new Node[0];
    public Node[] GetNodes { get { return nodes; } }

    [HideInInspector]
    [SerializeField]
    NodeConnection[] nodeConnections = new NodeConnection[0]; // All the connections between nodes
    int connectionIdCounter = 0; // Temporary value

    [Header("Agent")]
    public float agentRadius = 0.3f;
    public LayerMask obstacleLayerMask;


    private void Awake()
    {
        maps.Add(this);
    }

    //Generar mapa
    public void Clear ()
    {
        width = height = 0;
        nodes = new Node[0];
        nodeConnections = new NodeConnection[0];
    }
    public void GenerateNavMesh()
    {
        DateTime startTime = DateTime.Now;
        Debug.Log("Creating 2D navigation map...");

        width = (int)(mapSize.x / nodeDistance) + 1;
        height = (int)(mapSize.y * 2 / nodeDistance) + 1;

        List<Node> nodeList = new List<Node>();
        List<NodeConnection> nodeConnectionList = new List<NodeConnection>();

        Vector2 positionOffset = (Vector2)transform.position + new Vector2(-mapSize.x * .5f, mapSize.y * .5f);
        int actualLocalId = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float ptx = x * nodeDistance;
                float pty = y * nodeDistance * -0.5f;

                if (y % 2 == 0)
                {
                    //Node out of limits
                    if (x >= width - 1) continue;

                    ptx += (nodeDistance / 2);
                }

                Vector2 pos = new Vector2(ptx, pty) + positionOffset;

                Node node = new Node(this, actualLocalId, x, y, pos);
                nodeList.Add(node);

                node.CheckIfValid();

                actualLocalId++;
            }
        }
        nodes = nodeList.ToArray();

        // Create all connections
        foreach (Node node in nodes)
        {
            if (node == null) continue;

            // Right node
            Node rightNode = GetNode(node.GetX + 1, node.GetY);
            CreateConnection(nodeConnectionList, node, rightNode, NavDirection.Right);

            // Down node
            Node downNode = GetNode(node.GetX, node.GetY + 2);
            CreateConnection(nodeConnectionList, node, downNode, NavDirection.Bottom);

            // Diagonal nodes
            int xOffset = (node.GetY % 2 == 0) ? 0 : -1;

            // Bottom left
            Node bottomLeftNode = GetNode(node.GetX + xOffset, node.GetY + 1);
            CreateConnection(nodeConnectionList, node, bottomLeftNode, NavDirection.BotttomLeft);

            // Bottom right
            Node bottomRightNode = GetNode(node.GetX + 1 + xOffset, node.GetY + 1);
            CreateConnection(nodeConnectionList, node, bottomRightNode, NavDirection.BottomRight);
        }

        // Validate all conections
        nodeConnectionList.ForEach(connection => connection.CheckIfValid());
        nodeConnections = nodeConnectionList.ToArray();

        // Debug time spent
        Debug.Log("Done in " + DateTime.Now.Subtract(startTime).TotalMilliseconds + " ms.");
    }

    void CreateConnection(List<NodeConnection> connectionList, Node node1, Node node2, NavDirection direction)
    {
        if (node1 == null || node2 == null) return;

        connectionList.Add(node1.CreateConnection(connectionIdCounter, node2, direction));
        connectionIdCounter++;
    }

    public NodeConnection GetConnection(int id)
    {
        if (id < 0 || id >= nodeConnections.Length) return null;

        return nodeConnections[id];
    }

    public Node GetNode(int id)
    {
        if (id < 0 || id >= nodes.Length) return null;

        return nodes[id];
    }
    public Node GetNode(int x, int y)
    {
        // Check if y and x are in the limits
        // X is width limit depends if y is odd or even
        if (y < 0 || y >= height || x < 0 || x >= (width - (y % 2 == 0 ? 1 : 0))) return null;

        int oddRows = (int)((1 + y) * .5f); // Number of rows with "width - 1" nodes
        int evenRows = y - oddRows; // Number of rows with "width" nodes

        int result = (oddRows * (width - 1)) + (evenRows * width) + x;

        Node gotNode = nodes[result];
        return gotNode;
        /*
        foreach (Node node in nodes)
        {
            if (node.x == x && node.y == y) return node;
        }
        return null;
        */
    }

    public Node GetNearestNode(Vector2 position)
    {
        Node actualNode = GetNode(width / 2, height / 2);
        Node lastNearestNode = null;

        float sqrDistance = Vector2.SqrMagnitude(actualNode.GetPosition - position);

        while (actualNode != lastNearestNode)
        {
            lastNearestNode = actualNode;
            actualNode = actualNode.GetNearestNeighbourToPoint(position, sqrDistance, out sqrDistance);
        }

        return lastNearestNode;
    }
    public Node GetNearestValidNode(Vector2 position)
    {
        Node nearestNode = null;
        float nearestDistance = Mathf.Infinity;
        foreach (Node node in nodes)
        {
            if (node == null) continue;

            float sqrDistance = Vector2.SqrMagnitude(node.GetPosition - position);
            if (node.IsValid && sqrDistance < nearestDistance)
            {
                nearestNode = node;
                nearestDistance = sqrDistance;
            }
        }
        return nearestNode;
    }

    //Inspector
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 0, 0, 0.3f);
        Gizmos.DrawCube(transform.position, mapSize);

        foreach (Node node in nodes)
        {
            if (node != null)
            {
                Gizmos.color = node.IsValid ? Color.green : Color.red;
                Gizmos.DrawWireSphere(node.GetPosition, 0.2f);
            }
        }

        foreach (NodeConnection connection in nodeConnections)
        {
            if (connection != null)
            {
                Gizmos.color = connection.IsValid ? Color.green : Color.red;

                Vector2 start, end;
                if (connection.GetLine(out start, out end)) Gizmos.DrawLine(start, end);
            }
        }
    }
}

[Serializable]
public class Node
{
    [SerializeField]
    NavigationMesh2D owner;
    public NavigationMesh2D GetOwner { get { return owner; } }

    [SerializeField]
    int localId;
    public int GetLocalId { get { return localId; } }

    [SerializeField]
    bool validNode;
    public bool IsValid { get { return validNode; } }

    [SerializeField]
    Vector2 position;
    public Vector2 GetPosition { get { return position; } }

    //Grid coordinates
    [SerializeField]
    int x, y;
    public int GetX { get { return x; } }
    public int GetY { get { return y; } }

    //Node connections
    public NodeConnection Top { get; private set; }
    public NodeConnection Right { get; private set; }
    public NodeConnection Bottom { get; private set; }
    public NodeConnection Left { get; private set; }
    public NodeConnection TopLeft { get; private set; }
    public NodeConnection TopRight { get; private set; }
    public NodeConnection BottomRight { get; private set; }
    public NodeConnection BottomLeft { get; private set; }

    [SerializeField]
    public List<NodePath> adyacentNodePaths = new List<NodePath>(8);

    public Node(NavigationMesh2D owner, int localId, int gridX, int gridY, Vector2 position)
    {
        this.owner = owner;

        this.localId = localId;
        x = gridX; y = gridY;

        this.position = position;
    }

    public NodeConnection CreateConnection(int id, Node connectTo, NavDirection direction)
    {
        if (connectTo == null) return null;

        NodeConnection thisConnection = null;
        NodeConnection targetConnection = null;
        bool diagonal = false;

        switch (direction)
        {
            case NavDirection.Top:
                Top = thisConnection;
                connectTo.Bottom = targetConnection;
                break;
            case NavDirection.Right:
                Right = thisConnection;
                connectTo.Left = targetConnection;
                break;
            case NavDirection.Bottom:
                Bottom = thisConnection;
                connectTo.Top = targetConnection;
                break;
            case NavDirection.Left:
                Left = thisConnection;
                connectTo.Right = targetConnection;
                break;
            case NavDirection.TopLeft:
                TopLeft = thisConnection;
                connectTo.BottomRight = targetConnection;
                diagonal = true;
                break;
            case NavDirection.TopRight:
                TopRight = thisConnection;
                connectTo.BottomLeft = targetConnection;
                diagonal = true;
                break;
            case NavDirection.BottomRight:
                BottomRight = thisConnection;
                connectTo.TopLeft = targetConnection;
                diagonal = true;
                break;
            case NavDirection.BotttomLeft:
                BottomLeft = thisConnection;
                connectTo.TopRight = targetConnection;
                diagonal = true;
                break;
        }
        NodeConnection newConnection = new NodeConnection(owner, id, this, connectTo, diagonal);

        thisConnection = targetConnection = newConnection;

        this.adyacentNodePaths.Add(new NodePath(connectTo, newConnection));
        connectTo.adyacentNodePaths.Add(new NodePath(this, newConnection));

        return newConnection;
    }

    public void CheckIfValid()
    {
        /*
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(position, grid.agentRadius))
        {
            if (collider.transform.gameObject.isStatic)
            {
                validNode = false;
                return;
            }
        }
        validNode = true;
        */
        validNode = !Physics2D.OverlapCircle(position, owner.agentRadius, owner.obstacleLayerMask);
    }

    //Use square distances
    public Node GetNearestNeighbourToPoint(Vector2 targetPosition)
    {
        float i;
        return GetNearestNeighbourToPoint(targetPosition, Mathf.Infinity, out i);
    }
    public Node GetNearestNeighbourToPoint(Vector2 targetPosition, float actualDistance, out float nearestDistance)
    {
        NodePath path;
        return GetNearestNeighbourToPoint(targetPosition, actualDistance, out nearestDistance, out path);
    }
    public Node GetNearestNeighbourToPoint(Vector2 targetPosition, out NodePath path)
    {
        float i;
        return GetNearestNeighbourToPoint(targetPosition, Mathf.Infinity, out i, out path);
    }
    public Node GetNearestNeighbourToPoint(Vector2 targetPosition, float actualDistance, out float nearestDistance, out NodePath nodePath)
    {
        Node nearestNode = this;
        nearestDistance = actualDistance;
        nodePath = null;

        foreach (NodePath path in adyacentNodePaths)
        {
            Node getNode = path.GetNode;
            float neighbourDistance = Vector2.SqrMagnitude(targetPosition - getNode.position);
            if (neighbourDistance < nearestDistance)
            {
                nearestNode = getNode;
                nearestDistance = neighbourDistance;
                nodePath = path;
            }
        }
        return nearestNode;
    }

    public float GetHeuristicCost(Node toNode)
    {
        Node actualNode = this;
        float finalCost = 0;

        int i = 0;
        while (actualNode != toNode && i < 256)
        {
            NodePath nodePath = null;
            actualNode = actualNode.GetNearestNeighbourToPoint(toNode.GetPosition, out nodePath);

            finalCost += nodePath.GetConnection.GetCost;
            i++;
        }

        return finalCost;
    }
}

[Serializable]
public class NodeConnection
{
    const float diagonalCost = 0.707f;

    [SerializeField]
    NavigationMesh2D owner;

    [SerializeField]
    int localId;
    public int GetLocalId { get { return localId; } }

    [SerializeField]
    int nodeId1, nodeId2 = -1;

    [SerializeField]
    bool valid = true;
    public bool IsValid { get { return valid; } }

    [SerializeField]
    float cost = 1;
    public float GetCost { get { return cost; } }

    public NodeConnection(NavigationMesh2D owner, int id, Node node1, Node node2)
    {
        this.owner = owner;

        this.localId = id;

        nodeId1 = node1.GetLocalId;
        nodeId2 = node2.GetLocalId;

        if ((node1 != null && !node1.IsValid) || (node2 != null && !node2.IsValid))
        {
            valid = false;
        }
    }
    public NodeConnection(NavigationMesh2D owner, int id, Node node1, Node node2, bool diagonal) 
        : this(owner, id, node1, node2)
    {
        if (diagonal) cost = diagonalCost;
    }

    public Node GetOtherNodeId(Node fromNode)
    {
        if (fromNode.GetLocalId == nodeId1)
        {
            return owner.GetNode(nodeId2);
        }
        else if (fromNode.GetLocalId == nodeId2)
        {
            return owner.GetNode(nodeId1);
        }
        else return null;
    }

    public void CheckIfValid()
    {
        if (!valid) return;

        Vector2 start, end;
        if (!GetLine(out start, out end))
        {
            valid = false;
            return;
        }

        RaycastHit2D[] colliders = Physics2D.LinecastAll(start, end);
        foreach (RaycastHit2D collider in colliders)
        {
            if (collider.transform.gameObject.isStatic)
            {
                valid = false;
                return;
            }
        }
    }
    public bool GetLine (out Vector2 start, out Vector2 end)
    {
        Node node1 = owner.GetNode(nodeId1);
        Node node2 = owner.GetNode(nodeId2);

        if (node1 == null || node2 == null)
        {
            start = end = Vector2.zero;
            return false;
        }

        start = node1.GetPosition;
        end = node2.GetPosition;
        return true;
    }
}

[Serializable]
public class NodePath
{
    [SerializeField]
    NavigationMesh2D owner;

    [SerializeField]
    int toNodeId;
    public int GetNodeId { get { return toNodeId; } }
    public Node GetNode { get { return owner.GetNode(toNodeId); } }

    [SerializeField]
    int connectionId;
    public int GetConnectionId { get { return connectionId; } }
    public NodeConnection GetConnection { get { return owner.GetConnection(connectionId); } }

    public bool IsValid { get { return owner.GetConnection(connectionId).IsValid && owner.GetNode(toNodeId).IsValid; } }

    public NodePath(Node toNode, NodeConnection connection)
    {
        owner = toNode.GetOwner;
        toNodeId = toNode.GetLocalId;
        connectionId = connection.GetLocalId;
    }
}