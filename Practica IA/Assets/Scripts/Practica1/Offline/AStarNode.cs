using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.DataStructures
{


    public class AStarNode
    {
        CellInfo position;
        AStarNode parent;
        float distance;
        int nodeCounter;
        Locomotion.MoveDirection direction;

        public AStarNode(CellInfo pos, AStarNode p, float dist, Locomotion.MoveDirection dir)
        {
            position = pos;
            parent = p;
            if (p != null)
                nodeCounter = p.GetNodeCounter() + 1;
            else
                nodeCounter = 0;
            distance = dist;
            direction = dir;
        }

        public int CompareTo(AStarNode other)
        {
            if (this.nodeCounter == other.GetNodeCounter())
                return this.distance.CompareTo(other.GetDistance());
            else
                return this.nodeCounter.CompareTo(other.GetNodeCounter());
        }


        public Locomotion.MoveDirection GetDirection()
        {
            return direction;
        }

        public CellInfo GetCellData()
        {
            return position;
        }

        public AStarNode GetParent()
        {
            return parent;
        }

        public float GetDistance()
        {
            return distance;
        }

        public int GetNodeCounter()
        {
            return nodeCounter;
        }
    }
}
