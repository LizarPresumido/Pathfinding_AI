using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.DataStructures
{


    public class HorizonNode
    {
        CellInfo position;
        HorizonNode parent;
        float distance;
        int nodeCounter;
        Locomotion.MoveDirection direction;

        public HorizonNode(CellInfo pos, HorizonNode p, float dist, Locomotion.MoveDirection dir)
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
        public Locomotion.MoveDirection GetDirection()
        {
            return direction;
        }

        public CellInfo GetCellData()
        {
            return position;
        }

        public HorizonNode GetParent()
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
