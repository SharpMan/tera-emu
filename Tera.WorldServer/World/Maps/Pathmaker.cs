using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Fights;

namespace Tera.WorldServer.World.Maps
{
    public class Pathmaker
    {
        private List<int> openlist = new List<int>();
        private List<int> closelist = new List<int>();
        private Dictionary<int, Node> myNodes = new Dictionary<int, Node>();

        private bool FourDir;
        private bool isFight;

        private int nombreDePM;

        private struct Node
        {
            public int Parent;
            public double F;
            public double G;
            public double H;
        }

        private void loadSprites(Map Map, Fight Fight)
        {
            for (int i = Map.CellsCount - 1; i > 0; i--)
            {
                if (!Fight.IsCellWalkable(i))
                {
                    closelist.Add(i);
                }
            }
        }

        public MovementPath Pathing(Map Map, int nCellBegin, int nCellEnd, bool FourDir = false, int numberPM = 9999, bool IsInFight = false, Fight MyFight = null)
        {

            try
            {
                loadSprites(Map, MyFight);
                if (closelist.Contains(nCellBegin))
                    closelist.Remove(nCellBegin);
                if (closelist.Contains(nCellEnd))
                    closelist.Remove(nCellEnd);

                this.FourDir = FourDir;
                this.isFight = IsInFight;

                nombreDePM = numberPM;

                return findpath(Map, nCellBegin, nCellEnd);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public static List<int> getShortestPathBetween(SiteMap map, int start, int dest, int distMax)
        //{
        //    try
        //    {
        //        List<int> curPath = new List<int>();
        //        List<int> curPath2 = new List<int>();
        //        List<int> closeCells = new List<int>();
        //        int limit = 1000;

        //        SiteMap.Case curCase = map.getCase(start);
        //        int stepNum = 0;
        //        bool stop = false;

        //        while ((!stop) && (stepNum++ <= limit))
        //        {
        //            int nearestCell = getNearestCellAround(map, curCase.getID(), dest, closeCells);
        //            if (nearestCell == -1)
        //            {
        //                closeCells.add(curCase);
        //                if (curPath.size() > 0)
        //                {
        //                    curPath.remove(curPath.size() - 1);
        //                    if (curPath.size() > 0) curCase = (SiteMap.Case)curPath.get(curPath.size() - 1);
        //                    else
        //                        curCase = map.getCase(start);
        //                }
        //                else
        //                {
        //                    curCase = map.getCase(start);
        //                }
        //            }
        //            else
        //            {
        //                if ((distMax == 0) && (nearestCell == dest))
        //                {
        //                    curPath.add(map.getCase(dest));
        //                    break;
        //                } if (distMax > getDistanceBetween(map, nearestCell, dest))
        //                {
        //                    curPath.add(map.getCase(dest));
        //                    break;
        //                }

        //                curCase = map.getCase(nearestCell);
        //                closeCells.add(curCase);
        //                curPath.add(curCase);
        //            }
        //        }

        //        curCase = map.getCase(start);
        //        closeCells.clear();
        //        closeCells.add((SiteMap.Case)curPath.get(0));

        //        while ((!stop) && (stepNum++ <= limit))
        //        {
        //            int nearestCell = getNearestCellAround(map, curCase.getID(), dest, closeCells);
        //            if (nearestCell == -1)
        //            {
        //                closeCells.add(curCase);
        //                if (curPath2.size() > 0)
        //                {
        //                    curPath2.remove(curPath2.size() - 1);
        //                    if (curPath2.size() > 0) curCase = (SiteMap.Case)curPath2.get(curPath2.size() - 1);
        //                    else
        //                        curCase = map.getCase(start);
        //                }
        //                else
        //                {
        //                    curCase = map.getCase(start);
        //                }
        //            }
        //            else
        //            {
        //                if ((distMax == 0) && (nearestCell == dest))
        //                {
        //                    curPath2.add(map.getCase(dest));
        //                    break;
        //                } if (distMax > getDistanceBetween(map, nearestCell, dest))
        //                {
        //                    curPath2.add(map.getCase(dest));
        //                    break;
        //                }

        //                curCase = map.getCase(nearestCell);
        //                closeCells.add(curCase);
        //                curPath2.add(curCase);
        //            }
        //        }
        //        if (((curPath2.size() < curPath.size()) && (curPath2.size() > 0)) || (curPath.isEmpty()))
        //        {
        //            curPath = curPath2;
        //        }
        //        return curPath;
        //    }
        //    catch (Exception e)
        //    {
        //    }
        //    return null;
        //}


        private MovementPath findpath(Map Map, int cell1, int cell2)
        {
            int HeuristicEstimate = 20;
            int current = 0;
            int i = 0;

            openlist.Add(cell1);

            var NodeBegin = new Node();
            NodeBegin.G = 0;
            NodeBegin.H = Pathfinder.GoalDistanceNoSqrt(Map, current, cell2) * HeuristicEstimate;
            NodeBegin.F = NodeBegin.G + NodeBegin.H;
            NodeBegin.Parent = cell1;
            this.myNodes.Add(cell1, NodeBegin);


            var Test1 = Pathfinder.GoalDistanceScore(Map, cell1, cell2);
            var Test2 = Pathfinder.GoalDistanceEstimate(Map, cell1, cell2);
            var Test3 = Pathfinder.GoalDistanceNoSqrt(Map, cell1, cell2);

            current = cell1;

            while (!(openlist.Contains(cell2)))
            {
                if (i++ > 1000)
                    return null;

                if (current == cell2)
                    break; // TODO: might not be correct. Was : Exit Do
                closelist.Add(current);
                openlist.Remove(current);

                var cell = Map.GetBestCellBetween(current, cell2, closelist);
                if (cell != -1)
                {
                    if (!closelist.Contains(cell))
                    {
                        if (openlist.Contains(cell))
                        {
                            if (this.myNodes[cell].G > this.myNodes[current].G)
                            {
                                var Node = this.myNodes[cell];
                                Node.Parent = current;
                                Node.G = this.myNodes[current].G + 1;
                                Node.H = Pathfinder.GoalDistanceScore(Map, cell, cell2) * HeuristicEstimate;
                                Node.F = Node.G + Node.H;
                            }
                        }
                        else
                        {
                            openlist.Add(cell);
                            openlist[openlist.Count - 1] = cell;
                            var Node = new Node();
                            Node.G = this.myNodes[current].G + 1;
                            Node.H = Pathfinder.GoalDistanceScore(Map, cell, cell2) * HeuristicEstimate;
                            Node.F = Node.G + Node.H;
                            Node.Parent = current;
                            this.myNodes.Add(cell, Node);
                        }
                    }

                    current = cell;
                }
            }

            return (getParent(Map, cell1, cell2));
        }

        private MovementPath getParent(Map Map, int cell1, int cell2)
        {
            int current = cell2;
            List<int> pathCell = new List<int>();
            pathCell.Add(current);

            while (!(current == cell1))
            {
                pathCell.Add(this.myNodes[current].Parent);
                current = this.myNodes[current].Parent;
            }

            return getPath(Map, pathCell);
        }

        private MovementPath getPath(Map Map, List<int> pathCell)
        {
            pathCell.Reverse();
            MovementPath Path = new MovementPath();
            int current = 0;
            int child = 0;
            int PMUsed = 0;
            if (pathCell.Count > 1)
                Path.AddCell(pathCell[0], Pathfinder.GetDirection(Map, pathCell[0], pathCell[1]));
            for (int i = 0; i <= pathCell.Count - 2; i++)
            {
                PMUsed++;
                if ((PMUsed > nombreDePM))
                {
                    Path.MovementLength = nombreDePM;
                    return Path;
                }
                current = pathCell[i];
                child = pathCell[i + 1];
                Path.AddCell(child, Pathfinder.GetDirection(Map, current, child));
            }

            Path.MovementLength = PMUsed;

            return Path;
        }

        private int GetFPoint()
        {
            double x = 9999;
            int cell = 0;

            foreach (int item in openlist)
            {
                if (!closelist.Contains(item))
                {
                    if (this.myNodes[item].F < x)
                    {
                        x = this.myNodes[item].F;
                        cell = item;
                    }
                }
            }

            return cell;
        }
    }
}
