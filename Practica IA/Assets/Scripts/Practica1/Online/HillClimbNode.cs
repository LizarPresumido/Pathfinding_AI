using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.DataStructures
{


    public class HillClimbNode
    {
        CellInfo position;
        float distance;
        Locomotion.MoveDirection direction;

        public HillClimbNode() { }

        public HillClimbNode(CellInfo pos, float dist, Locomotion.MoveDirection dir)
        {
            position = pos;
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

        public float GetDistance()
        {
            return distance;
        }
    }
}
