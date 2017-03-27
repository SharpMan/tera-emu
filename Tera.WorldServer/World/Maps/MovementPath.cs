using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Maps
{
    public class MovementPath
    {
        private StringBuilder mySerializedPath = new StringBuilder();
        private bool mySerialized = false;

        public override string ToString()
        {
            if (!this.mySerialized)
            {
                for (int i = 0; i < TransitCells.Count; i++)
                {
                    this.mySerializedPath.Append(Pathfinder.GetDirectionChar(Directions[i]));
                    this.mySerializedPath.Append(CellHelper.CellIdToCharCode(TransitCells[i]));
                }
                this.mySerialized = true;
            }

            return this.mySerializedPath.ToString();
        }

        public int BeginCell
        {
            get
            {
                return TransitCells.FirstOrDefault();
            }
        }

        public int MovementLength
        {
            get;
            set;
        }

        public int MovementTime
        {
            get
            {
                return (int)Pathfinder.GetPathTime(this.MovementLength);
            }
        }

        public int LastStep
        {
            get
            {
                return TransitCells[TransitCells.Count - 2];
            }
        }

        public int EndCell
        {
            get
            {
                return TransitCells.LastOrDefault();
            }
        }

        public List<int> TransitCells = new List<int>();
        public List<int> Directions = new List<int>();

        public void AddCell(int Cell, int Direction)
        {
            this.TransitCells.Add(Cell);
            this.Directions.Add(Direction);
        }

        public int GetDirection(int Cell)
        {
            return this.Directions[TransitCells.IndexOf(Cell) + 1];
        }

        public void Clean()
        {
            var TransitCells = new List<int>();
            var Directions = new List<int>();

            for (int i = 0; i < this.Directions.Count; i++)
            {
                if (i == this.Directions.Count - 1)
                {
                    TransitCells.Add(this.TransitCells[i]);
                    Directions.Add(this.Directions[i]);
                }
                else
                {
                    if (this.Directions[i] != this.Directions[i + 1])
                    {
                        TransitCells.Add(this.TransitCells[i]);
                        Directions.Add(this.Directions[i]);
                    }
                }
            }

            this.TransitCells = TransitCells;
            this.Directions = Directions;
        }
    }
}
