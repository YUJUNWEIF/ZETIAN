using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FindPath
{
    public class DetectableObject
    {
        public Vector3 position;
        public float Radius;
    }
    class Node
    {
        public Grid grid;
        public Node prev;
        public float far;
        public float predicate;
    }
    float cellsize = 0.5f;
    Vector3 m_from;
    public LinkedList<Vector3> waypoints { get; private set; }
    public bool Find(Vector3 from, float fromRadius, Vector3 target, List<DetectableObject> obstacles)
    {
        m_from = from;
        Dictionary<int, Node> walkedNodes = new Dictionary<int, Node>();

        Util.BinaryStack<Node> m_sorter = new Util.BinaryStack<Node>((x, y) => (x.far + x.predicate).CompareTo(y.far + y.predicate));
        var occupies = GenerateDynamicObstacles(fromRadius, obstacles);

        var targetNode = CaculateOccupiedCell(target);
        var first = new Node();        
        first.grid = CaculateOccupiedCell(from);
        first.far = 0;
        first.predicate = PredicateDistance(targetNode, first.grid);
        walkedNodes.Add(first.grid.GetHashCode(), first);
        m_sorter.Add(first);
        while (walkedNodes.Count < 1024)
        {
            Node node;
            if (!m_sorter.Pop(out node))
            {
                return false;
            }
            var grids = new Grid[]{
                new Grid((short)(node.grid.x - 1), (short)(node.grid.z + 0)),
                new Grid((short)(node.grid.x - 1), (short)(node.grid.z + 1)),
                new Grid((short)(node.grid.x - 0), (short)(node.grid.z + 1)),
                new Grid((short)(node.grid.x + 1), (short)(node.grid.z + 1)),
                new Grid((short)(node.grid.x + 1), (short)(node.grid.z - 0)),
                new Grid((short)(node.grid.x + 1), (short)(node.grid.z - 1)),
                new Grid((short)(node.grid.x + 0), (short)(node.grid.z - 1)),
                new Grid((short)(node.grid.x - 1), (short)(node.grid.z - 1)),
            };
            for (int index = 0; index < grids.Length; ++index)
            {
                var grid = grids[index];

                float weight;
                if (!occupies.TryGetValue(grid.GetHashCode(), out weight)) 
                {
                    weight = (grid.x - node.grid.x) * (grid.z - node.grid.z) == 0 ? 1f : 1.414f;
                }

                Node childNode;
                if (walkedNodes.TryGetValue(grid.GetHashCode(), out childNode))
                {
                    if (childNode.far > node.far + weight)
                    {
                        childNode.prev = node;
                        childNode.far = node.far + weight;
                        //childNode.predicate = childNode.far + PredicateDistance(tg, grid);
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    childNode = new Node();
                    childNode.grid = grid;
                    childNode.prev = node;
                    childNode.far = node.far + weight;
                    childNode.predicate = childNode.far + PredicateDistance(targetNode, grid);
                    walkedNodes.Add(grid.GetHashCode(), childNode);
                }
                if (childNode.grid == targetNode)
                {
                    waypoints = GeneratePath(childNode);
                    if (waypoints.Count >= 2)
                    {
                        waypoints.RemoveFirst();
                        waypoints.RemoveLast();
                        waypoints.AddLast(target);
                    }
                    return true;
                }
                else
                {
                    m_sorter.Add(childNode);
                }
            }
        }
        {
            Node node;
            m_sorter.Pop(out node);
            waypoints = GeneratePath(node);
            if (waypoints.Count >= 2)
            {
                waypoints.RemoveFirst();
                waypoints.RemoveLast();
                waypoints.AddLast(target);
            }
        }
        return false;
    }
    /** 弗洛伊德路径平滑处理 */
    LinkedList<Node> FloYD(Node end, Dictionary<int, float> occupies)
    {
        LinkedList<Node> wayNodes = new LinkedList<Node>();
        var no = end;
        while (no != null)
        {
            wayNodes.AddFirst(no);
            no = no.prev;
        }
        //RemoveNodesInLine(wayNodes);
        //RemoveNodesNoBarrier(wayNodes, occupies);
        return wayNodes;
    }
    static void RemoveNodesInLine(LinkedList<Node> wayNodes)
    {
        LinkedListNode<Node> first, middle, last = wayNodes.Last;
        while (true)
        {
            if (last != null) { middle = last.Previous; }
            else { break; }
            if (middle != null) { first = middle.Previous; }
            else { break; }
            if (first != null)
            {
                if ((first.Value.grid.x - middle.Value.grid.x) * (middle.Value.grid.z - last.Value.grid.z) ==
                    (middle.Value.grid.x - last.Value.grid.x) * (first.Value.grid.z - middle.Value.grid.z))
                {
                    wayNodes.Remove(middle);
                }
                else
                {
                    last = last.Previous;
                }
            }
            else { break; }
        }
    }
    void RemoveNodesNoBarrier(LinkedList<Node> wayNodes, Dictionary<int, int> occupies)
    {
        if (wayNodes.Count <= 2) { return; }

        LinkedListNode<Node> last = wayNodes.Last;
        while (last != null)
        {
            var start = wayNodes.First;
            while (start != last && start.Next != last)
            {
                if (!HasBarrier(start.Value.grid, last.Value.grid, occupies))
                {
                    var nodeRmv = start.Next;
                    while (nodeRmv != last)
                    {
                        var tmp = nodeRmv;
                        nodeRmv = nodeRmv.Next;
                        wayNodes.Remove(tmp);
                    }
                    break;
                }
                start = start.Next;
            }
            last = last.Previous;
        }
    }
    static bool HasBarrier(Grid start, Grid end, Dictionary<int, int> occupies)
    {
        var hori = Mathf.Abs(end.x - start.x) > Mathf.Abs(end.z - start.z);
        if (hori)
        {
            short x = start.x;
            while (x != end.x)
            {
                float z = (x - start.x) * 1f / (end.x - start.x) + start.z;
                int hashCode = Grid.HashCode(x, (short)Mathf.RoundToInt(z));
                if (occupies.ContainsKey(hashCode)) { return false; }
                x += (short)(end.x > start.x ? 1 : -1);
            }
        }
        else
        {
            short z = start.z;
            while (z != end.z)
            {
                float x = (z - start.z) * 1f / (end.z - start.z) + start.x;
                int hashCode = Grid.HashCode((short)Mathf.RoundToInt(x), z);
                if (occupies.ContainsKey(hashCode)) { return false; }
                z += (short)(end.z > start.z ? 1 : -1);
            }
        }
        return true;
    }
    float PredicateDistance(Grid a, Grid b)
    {
        return Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.z - b.z) * (a.z - b.z));
    }
    Grid CaculateOccupiedCell(Vector3 pos)
    {
        return new Grid((short)Mathf.RoundToInt(pos.x / cellsize), (short)Mathf.RoundToInt(pos.z / cellsize));
    }
    LinkedList<Vector3> GeneratePath(Node end)
    {
        waypoints = new LinkedList<Vector3>();
        while (end != null)
        {
            waypoints.AddFirst(new Vector3(end.grid.x * cellsize, 0, end.grid.z * cellsize));
            end = end.prev;
        }
        return waypoints;
    }
    Dictionary<int, float> GenerateDynamicObstacles(float fromRadius, List<DetectableObject> obstacles)
    {
        Dictionary<int, float> occupies = new Dictionary<int, float>();
        for (int index = 0; index < obstacles.Count; ++index )
        {
            var obstacle = obstacles[index];
            var center = new Vector2(obstacle.position.x, obstacle.position.z);
            float boundingRadius = (obstacle.Radius + fromRadius) * 0.8f;
            short left = (short)Mathf.FloorToInt((center.x - boundingRadius) / cellsize);
            short right = (short)Mathf.CeilToInt((center.x + boundingRadius) / cellsize);
            short up = (short)Mathf.FloorToInt((center.y - boundingRadius) / cellsize);
            short down = (short)Mathf.CeilToInt((center.y + boundingRadius) / cellsize);
            float sqrRoundingRadius = boundingRadius * boundingRadius;
            for (short col = left; col < right; ++col )
            {
                for (short row = up; row < down; ++row)
                {
                    var cell = new Vector2(col * cellsize, row * cellsize);
                    float sqrDistance = Vector2.SqrMagnitude(cell - center) / sqrRoundingRadius;
                    if (sqrDistance < 1f)
                    {
                        int far = sqrDistance < 0.00001f ? 100000 : Mathf.CeilToInt(1 / sqrDistance);
                        var hashCode = Grid.HashCode(col, row);
                        float exist;
                        if (occupies.TryGetValue(hashCode, out exist))
                        {
                            occupies[hashCode] = exist + far;
                        }
                        else
                        {
                            occupies.Add(hashCode, far); 
                        }
                    }
                }
            }
        }
        return occupies;
    }
    public void OnDrawGizmos()
    {
        if (waypoints != null)
        {
            Gizmos.color = Color.white;
            var next = waypoints.First;
            while (next != null)
            {
                //UnityEditor.Handles.DrawLine(prev, next.Value);
                var first = CaculateOccupiedCell(m_from);
                var start = new Vector3(first.x * cellsize, 0, first.z * cellsize);
                var prev = next.Previous != null ? next.Previous.Value : start;
                Gizmos.DrawLine(prev, next.Value);
                Gizmos.DrawSphere(next.Value, 0.05f);
                next = next.Next;
            }
        }
    }
}