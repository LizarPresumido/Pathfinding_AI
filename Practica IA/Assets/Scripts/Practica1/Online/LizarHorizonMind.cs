using Assets.Scripts.DataStructures;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.SampleMind
{
    public class LizarHorizonMind : AbstractPathMind
    {
        List<HorizonNode> openNodes = new List<HorizonNode>();
        List<HorizonNode> path = new List<HorizonNode>();
        HorizonNode nextMove = null;
        List<EnemyBehaviour> enemies = new List<EnemyBehaviour>();
        HorizonNode nodeAux = null;
        //Detecto el objetivo mas cercano a conveniencia
        CellInfo nearestGoal;
        int[,] bitmap = new int[15, 15];
        int count = 1;
        bool pathed = false;
        System.DateTime startTime;


        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            //inicializo los enemigos a cazar
            if (enemies.Count == 0)
            {
                enemies = boardInfo.Enemies;
            }
            if (path.Count == 0)
            {
                //Creo la ruta si no existe ninguna o si ya a acabado de ejecutar la anterior
                if (!pathed && nextMove == null) {
                    //Debug.Log(currentPos.GetPosition);
                    startTime = System.DateTime.Now;
                    HorizonSearch(new HorizonNode(currentPos, null, 9999, Locomotion.MoveDirection.None), ref boardInfo, ref goals);
                    Debug.Log((System.DateTime.Now - startTime).TotalMilliseconds);
                    pathed = true;
                }
                return Locomotion.MoveDirection.None;
            }
            else
            {
                //Debug.Log(path[0].GetCellData().GetPosition);
                //actualizo el mapa de movimientos
                bitmap[(int)path[0].GetCellData().GetPosition.x, (int)path[0].GetCellData().GetPosition.y]++;
                Locomotion.MoveDirection direction = path[0].GetDirection();
                path.RemoveAt(0);
                if (path.Count == 0)
                    nextMove = null;
                pathed = false;
                return direction;
            }
        }

        private void HorizonSearch(HorizonNode currentNode, ref BoardInfo boardInfo, ref CellInfo[] goals)
        {
            CellInfo[] nextMoves = currentNode.GetCellData().WalkableNeighbours(boardInfo);
            
            
            //para ahorrar memoria y dejar el codigo mas limpio uso esta variable para guardar la posicion de lso nodos a analizar
            Vector2 newPosition;
            //con esta variable cierro los cuellos de botella cuando se aproxima al goal
            int walkableTiles;
            //array de movimientos futuros, para detectar los cuellos de botella
            CellInfo[] futureMoves;

            //establezco el orden de prioridades de subobjetivos
            if (enemies.Count != 0)
            {
                for (int i = 0; i < enemies.Count; i++)
                    if (enemies[0] == null)
                    {
                        //limpio el array de enemigos para que no me guarde nulos al atrapar un enemigo
                        enemies.RemoveAt(i);
                        //reinicio el mapa de a* para ir a por el siguiente objetivo
                        bitmap = new int[15, 15];
                    }
                nearestGoal = enemies[0].CurrentPosition();
            }
            else
            {
                if (nearestGoal == null || nearestGoal.CellId != goals[0].CellId)
                {
                    bitmap = new int[15, 15];
                    nearestGoal = goals[0];
                }
            }
            //Debug.Log(currentNode.GetNodeCounter() + " " + currentNode.GetCellData().GetPosition);
            //Se limita la profundidad de la búsqueda
            if (currentNode.GetNodeCounter() < 2) {
                for (int i = 0; i < nextMoves.Length; i++)
                {
                    walkableTiles = 0;
                    if (nextMoves[i] != null)
                    {
                        if (nextMoves[i].Walkable)
                        {
                            //persecución del enemigo en distancia corta
                            if (Vector2.Distance(currentNode.GetCellData().GetPosition, nearestGoal.GetPosition) <= 2f && goals[0].GetPosition != nearestGoal.GetPosition) {
                                nodeAux = new HorizonNode(nextMoves[i], currentNode, Vector2.Distance(nearestGoal.GetPosition, nextMoves[i].GetPosition), GetDirection2Vector(nextMoves[i].GetPosition, currentNode.GetCellData().GetPosition));
                                openNodes.Add(nodeAux);
                                count++;
                            }
                            else
                            {
                                //Ruta en a* para aproximarse al enemigo o para llegar a la meta
                                newPosition = nextMoves[i].GetPosition;
                                futureMoves = nextMoves[i].WalkableNeighbours(boardInfo);
                                for (int j = 0; j < futureMoves.Length && walkableTiles <= 1; j++)
                                    if (futureMoves[j] != null)
                                        if (futureMoves[j].Walkable)
                                            walkableTiles++;
                                if (walkableTiles > 1 && bitmap[(int)newPosition.x, (int)newPosition.y] == 0)
                                {
                                    //Debug.Log(walkableTiles);
                                    nodeAux = new HorizonNode(nextMoves[i], currentNode, Vector2.Distance(nearestGoal.GetPosition, nextMoves[i].GetPosition), GetDirection2Vector(nextMoves[i].GetPosition, currentNode.GetCellData().GetPosition));
                                    openNodes.Add(nodeAux);
                                    count++;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (nextMove == null)
                    nextMove = currentNode;
                else if (nextMove.GetDistance() > currentNode.GetDistance())
                    nextMove = currentNode;
            }

            //limpio el nodo ya investigado, no en la primera iteración
            if(count > 2)
                openNodes.RemoveAt(0);
            if(openNodes.Count == 0)
            {
                CreatePath(nextMove);
                path.Reverse();
                Debug.Log(count);
                count = 0;
                bitmap = new int[15, 15];
            }
            else
            {
                HorizonSearch(openNodes[0], ref boardInfo, ref goals);
            }

        }

        private void CreatePath(HorizonNode goal)
        {
            if (goal.GetParent() != null)
            {
                path.Add(goal);
                CreatePath(goal.GetParent());
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
    }
}
