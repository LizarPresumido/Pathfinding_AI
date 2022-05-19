using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.DataStructures
{


    public class SimpleNode
    {
        CellInfo position;
        SimpleNode parent;
        Locomotion.MoveDirection direction;

        public SimpleNode(CellInfo pos, SimpleNode p, Locomotion.MoveDirection dir)
        {
            position = pos;
            parent = p;
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

        public SimpleNode GetParent()
        {
            return parent;
        }
    }
}