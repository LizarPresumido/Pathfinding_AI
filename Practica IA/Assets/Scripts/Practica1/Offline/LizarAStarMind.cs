using Assets.Scripts.DataStructures;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.SampleMind
{
    public class LizarAStarMind : AbstractPathMind
    {
        bool meta = false;
        bool pathed = false;
        List<AStarNode> nodeList = new List<AStarNode>();
        List<AStarNode> path = new List<AStarNode>();
        int count = 1;
        //Al ser un grid mapeo el movimiento para evitar ciclos simples de movimiento
        bool[,] bitmap = new bool[15, 15];
        System.DateTime startTime;

        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            //evito el caso en que se llegue a pedir un nuevo movimiento antes de haber creado la ruta
            if (!pathed)
            {
                bitmap[0, 0] = true;
                startTime = System.DateTime.Now;
                AStarSearch(new AStarNode(currentPos, null, Vector2.Distance(currentPos.GetPosition, goals[0].GetPosition), Locomotion.MoveDirection.None), ref boardInfo, ref goals);
                Debug.Log((System.DateTime.Now - startTime).TotalMilliseconds);
                Debug.Log(count);
                Debug.Log(path.Count);
                pathed = true;
                //Debug.Log("Start");
            }
            //en caso de que tardara el algoritmo se le da un movimiento vacio, si no, la ruta
            if (path.Count == 0)
                return Locomotion.MoveDirection.None;
            else
            {
                Locomotion.MoveDirection direction = path[0].GetDirection();
                path.RemoveAt(0);
                return direction;
            }
        }

        //En caso de que hubiera varias salidas de mismo valor y una cerrada y otra no, habria que filtrar de que eliga las transitables
        private void AStarSearch(AStarNode currentNode, ref BoardInfo boardInfo, ref CellInfo[] goals)
        {
            Debug.Log(count);
            CellInfo[] nextMoves = currentNode.GetCellData().WalkableNeighbours(boardInfo);
            //Compruebo la meta por distancia, si no es transitable me quedo al lado y si no, llego hasta la meta
            for (int i = 0; i < goals.Length && !meta; i++)
                if (!goals[i].Walkable && Vector2.Distance(currentNode.GetCellData().GetPosition, goals[i].GetPosition) <= 1f)
                    meta = true;
                else if (goals[i].Walkable && Vector2.Distance(currentNode.GetCellData().GetPosition, goals[i].GetPosition) == 0)
                    meta = true;

            if (meta)
            {
                //Creo la lista de nodos de la ruta y la invierto para ir desde el comienzo al final
                CreatePath(currentNode);
                path.Reverse();
                Debug.Log(count);
            }
            else
            {
                for (int i = 0; i < nextMoves.Length && !meta; i++)
                {
                    if (nextMoves[i] != null)
                    {
                        Vector2 position = nextMoves[i].GetPosition;

                        if ((currentNode.GetParent() == null || bitmap[(int)position.x, (int)position.y] == false) && nextMoves[i].Walkable)
                        {
                            nodeList.Add(new AStarNode(nextMoves[i], currentNode, Vector2.Distance(currentNode.GetCellData().GetPosition, goals[0].GetPosition), GetDirection2Vector(position, currentNode.GetCellData().GetPosition)));
                            count++;
                            bitmap[(int)position.x, (int)position.y] = true;
                        }
                    }
                }
                //Empiezo a eliminar los nodos de la lista despues de haber expandido por primera vez
                if (count != 3)
                    nodeList.RemoveAt(0);
                if (!meta)
                {
                    //ordeno la lista utilizando la distancia vectorial y vuelvo a llamar la funcion con el nodo mas prometedor
                    nodeList.Sort((a,b) => a.CompareTo(b));
                    AStarSearch(nodeList[0], ref boardInfo, ref goals);
                }

            }
        }

        private Locomotion.MoveDirection GetDirection2Vector(Vector2 end, Vector2 origin)
        {
            Vector2 direction = end - origin;
            if (direction.x == 0)
            {
                if (direction.y == 1)
                    return Locomotion.MoveDirection.Up;
                else
                    return Locomotion.MoveDirection.Down;
            }
            else
            {
                if (direction.x == 1)
                    return Locomotion.MoveDirection.Right;
                else
                    return Locomotion.MoveDirection.Left;
            }
        }

        private void CreatePath(AStarNode goal)
        {
            if (goal.GetParent() != null)
            {
                path.Add(goal);
                CreatePath(goal.GetParent());
            }
        }
    }
}
