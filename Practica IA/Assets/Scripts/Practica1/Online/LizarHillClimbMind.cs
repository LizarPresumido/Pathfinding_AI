using Assets.Scripts.DataStructures;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.SampleMind
{
    public class LizarHillClimbMind : AbstractPathMind
    {
        HillClimbNode nextMove = new HillClimbNode();
        List<EnemyBehaviour> enemies = new List<EnemyBehaviour>();
        bool[,] bitmap = new bool[15, 15];
        int count = 0;
        bool semaforo = false;
        System.DateTime startTime;


        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            if(enemies.Count == 0)
            {
                enemies = boardInfo.Enemies;
            }
            //alterno el moverse con crear el siguiente movimiento
            if (!semaforo)
            {
                nextMove = new HillClimbNode(currentPos, 9999, Locomotion.MoveDirection.None);
                startTime = System.DateTime.Now;
                HillClimbSearch(nextMove, ref boardInfo, ref goals);
                Debug.Log((System.DateTime.Now - startTime).TotalMilliseconds);
                Debug.Log(count);
                semaforo = true;
                return Locomotion.MoveDirection.None;
            }
            else
            {
                Locomotion.MoveDirection direction = nextMove.GetDirection();
                semaforo = false;
                return direction;
            }
        }

        private void HillClimbSearch(HillClimbNode currentNode, ref BoardInfo boardInfo, ref CellInfo[] goals)
        {
            CellInfo[] nextMoves = currentNode.GetCellData().WalkableNeighbours(boardInfo);
            //Detecto el objetivo mas cercano a conveniencia
            CellInfo nearestGoal;
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
                        bitmap = new bool[15, 15];
                    }
                nearestGoal = enemies[0].CurrentPosition();
            }
            else
            {
                bitmap = new bool[15, 15];
                nearestGoal = goals[0];
            }

            
            for(int i = 0; i < nextMoves.Length; i++)
            {
                walkableTiles = 0;
                if(nextMoves[i] != null)
                {
                    if (nextMoves[i].Walkable)
                    {
                        newPosition = nextMoves[i].GetPosition;
                        //ruta de persecucion de enemigos próximos
                        if(Vector2.Distance(currentNode.GetCellData().GetPosition, nearestGoal.GetPosition) <= 3f && goals[0].GetPosition != nearestGoal.GetPosition)
                        {
                            if (nextMove.GetDistance() > Vector2.Distance(nearestGoal.GetPosition, newPosition))
                            {
                                nextMove = new HillClimbNode(nextMoves[i], Vector2.Distance(newPosition, nearestGoal.GetPosition), GetDirection2Vector(newPosition, currentNode.GetCellData().GetPosition));
                                count++;
                            }
                        }
                        //ruta de a* para aproximarse a un objetivo
                        else
                        {
                            futureMoves = nextMoves[i].WalkableNeighbours(boardInfo);
                            for (int j = 0; j < futureMoves.Length && walkableTiles <= 1; j++)
                                if (futureMoves[j] != null)
                                    if(futureMoves[j].Walkable)
                                        walkableTiles++;
                            if (walkableTiles > 1 && !bitmap[(int)newPosition.x, (int)newPosition.y])
                            {

                                if (nextMove.GetDistance() > Vector2.Distance(nearestGoal.GetPosition, newPosition))
                                {
                                    nextMove = new HillClimbNode(nextMoves[i], Vector2.Distance(newPosition, nearestGoal.GetPosition), GetDirection2Vector(newPosition, currentNode.GetCellData().GetPosition));
                                    count++;
                                }
                            }
                        }
                    }
                }
                
            }
            //actualizo el mapa de ruta para no repetir movimientos
            bitmap[(int)nextMove.GetCellData().GetPosition.x, (int)nextMove.GetCellData().GetPosition.y] = true;
            if (Vector2.Distance(currentNode.GetCellData().GetPosition, nearestGoal.GetPosition) <= 3f && Vector2.Distance(currentNode.GetCellData().GetPosition, nearestGoal.GetPosition) < Vector2.Distance(nextMove.GetCellData().GetPosition, nearestGoal.GetPosition))
                bitmap = new bool[15, 15];

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
