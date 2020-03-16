using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Robot_Rampage
{
    static class PathFinder
    {
        #region Declarations
        private enum NodeStatus { Open, Closed };

        private static Dictionary<Vector2, NodeStatus> nodeStatus = new Dictionary<Vector2, NodeStatus>();

        private const int CostStraight = 10;
        private const int CostDiagonal = 15;

        private static List<PathNode> openList = new List<PathNode>();

        private static Dictionary<Vector2, float> nodeCosts = new Dictionary<Vector2, float>();
        #endregion

        #region Helper Methods
        static private void AddNodeToOpenList(PathNode node)
        {
            int index = 0;
            float cost = node.TotalCost;

            while ((openList.Count() > index) && (cost < openList[index].TotalCost))
            {
                index++;
            }

            openList.Insert(index, node);
            nodeCosts[node.GridLocation] = node.TotalCost;
            nodeStatus[node.GridLocation] = NodeStatus.Open;
        }

        static private List<PathNode> FindAdjacentNodes(PathNode currentNode, PathNode endNode)
        {
            List<PathNode> adjacentNodes = new List<PathNode>();

            int x = currentNode.GridX;
            int y = currentNode.GridY;

            bool upLeft = true;
            bool upRight = true;
            bool downLeft = true;
            bool downRight = true;

            if ((x > 0) && (!TileMap.IsWallTile(x - 1, y)))
            {
                adjacentNodes.Add(new PathNode(currentNode, endNode, new Vector2(x - 1, y), CostStraight + currentNode.DirectCost));
            }
            else
            {
                upLeft = false;
                downLeft = false;
            }

            if ((x < 49) && (!TileMap.IsWallTile(x + 1, y)))
            {
                adjacentNodes.Add(new PathNode(currentNode, endNode, new Vector2(x + 1, y), CostStraight + currentNode.DirectCost));
            }
            else
            {
                upRight = false;
                downRight = false;
            }

            if ((y > 0) && (!TileMap.IsWallTile(x, y - 1)))
            {
                adjacentNodes.Add(new PathNode(currentNode, endNode, new Vector2(x, y - 1), CostStraight + currentNode.DirectCost));
            }
            else
            {
                upLeft = false;
                upRight = false;
            }

            if ((y < 49) && (!TileMap.IsWallTile(x, y + 1)))
            {
                adjacentNodes.Add(new PathNode(currentNode, endNode, new Vector2(x, y + 1), CostStraight + currentNode.DirectCost));
            }
            else
            {
                downLeft = false;
                downRight = false;
            }

            if ((upLeft) && (!TileMap.IsWallTile(x - 1, y - 1)))
            {
                adjacentNodes.Add(new PathNode(currentNode, endNode, new Vector2(x - 1, y - 1), CostDiagonal + currentNode.DirectCost));
            }

            if ((upRight) && (!TileMap.IsWallTile(x + 1, y - 1)))
            {
                adjacentNodes.Add(new PathNode(currentNode, endNode, new Vector2(x + 1, y - 1), CostDiagonal + currentNode.DirectCost));
            }

            if ((downLeft) && (!TileMap.IsWallTile(x - 1, y + 1)))
            {
                adjacentNodes.Add(new PathNode(currentNode, endNode, new Vector2(x - 1, y + 1), CostDiagonal + currentNode.DirectCost));
            }

            if ((downLeft) && (!TileMap.IsWallTile(x + 1, y + 1)))
            {
                adjacentNodes.Add(new PathNode(currentNode, endNode, new Vector2(x + 1, y + 1), CostDiagonal + currentNode.TotalCost));
            }

            return adjacentNodes;
        }
        #endregion

        #region Public Methods
        static public List<Vector2> FindPath(Vector2 startTile, Vector2 endTile)
        {
            if (TileMap.IsWallTile(endTile) || TileMap.IsWallTile(startTile))
            {
                return null;
            }
            openList.Clear();
            nodeCosts.Clear();
            nodeStatus.Clear();

            PathNode startNode;
            PathNode endNode;

            endNode = new PathNode(null, null, endTile, 0);
            startNode = new PathNode(null, endNode, startTile, 0);

            AddNodeToOpenList(startNode);
            while (openList.Count > 0)
            {
                PathNode currentNode = openList[openList.Count - 1];
                if (currentNode.IsEqualToNode(endNode))
                {
                    List<Vector2> bestPath = new List<Vector2>();
                    while (currentNode != null)
                    {
                        bestPath.Insert(0, currentNode.GridLocation);
                        currentNode = currentNode.ParentNode;
                    }
                    return bestPath;
                }
                openList.Remove(currentNode);
                nodeCosts.Remove(currentNode.GridLocation);

                foreach (PathNode possibleNode in FindAdjacentNodes(currentNode, endNode))
                {
                    if (nodeStatus.ContainsKey(possibleNode.GridLocation))
                    {
                        if (nodeStatus[possibleNode.GridLocation] == NodeStatus.Closed)
                        {
                            continue;
                        }
                        if (nodeStatus[possibleNode.GridLocation] == NodeStatus.Open)
                        {
                            if (possibleNode.TotalCost >= nodeCosts[possibleNode.GridLocation])
                            {
                                continue;
                            }
                        }
                    }
                    AddNodeToOpenList(possibleNode);
                }
                nodeStatus[currentNode.GridLocation] = NodeStatus.Closed;
            }
            return null;
        }
        #endregion
    }
}
