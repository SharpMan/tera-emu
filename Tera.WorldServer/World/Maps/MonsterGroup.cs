using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.World.Fights;

namespace Tera.WorldServer.World.Maps
{
    public sealed class MonsterGroup : IGameActor
    {
        static Random RANDOM = new Random();

        public List<ItemLoot> ItemLoot
        {
            get;
            private set;
        }

        public long KamasLoot
        {
            get;
            private set;
        }

        public int Alignement
        {
            get;
            private set;
        }

        public AlignmentTypeEnum AlignmentType
        {
            get
            {
                return (AlignmentTypeEnum)this.Alignement;
            }
        }

        public Boolean IsFix
        {
            get;
            set;
        }

        public int Orientation
        {
            get;
            set;
        }

        public int Aggrodistance
        {
            get;
            private set;
        }

        public Dictionary<int, MonsterLevel> Monsters
        {
            get;
            private set;
        }

        private long groupId;
        public long ActorId
        {
            get
            {
                return this.groupId;
            }
            set
            {
                this.groupId = value;
            }
        }

        public int CellId
        {
            get;
            set;
        }

        public GameActorTypeEnum ActorType
        {
            get
            {
                return GameActorTypeEnum.TYPE_MONSTER;
            }
        }

        private StringBuilder mySerializedString;
        public void SerializeAsGameMapInformations(StringBuilder SerializedString)
        {
            if (this.mySerializedString == null)
            {
                string MobIds = string.Join(",", this.Monsters.Select(x => x.Value.Monster.ID));
                string MobGfx = string.Join(",", this.Monsters.Select(x => x.Value.Monster.Look.ToString() + "^" + x.Value.Size));
                string MobLevels = string.Join(",", this.Monsters.Select(x => x.Value.Level));
                string MobColors = string.Join("", this.Monsters.Select(x => x.Value.Monster.Colors + ";0,0,0,0;"));

                this.mySerializedString = new StringBuilder();
                this.mySerializedString.Append(this.Orientation).Append(";0;");
                this.mySerializedString.Append(this.ActorId).Append(";");
                this.mySerializedString.Append(MobIds).Append(";-3;");
                this.mySerializedString.Append(MobGfx).Append(";");
                this.mySerializedString.Append(MobLevels).Append(";");
                this.mySerializedString.Append(MobColors);
            }

            // La cell peux changer donc on la stock pas en cache
            SerializedString.Append(this.CellId).Append(";");
            SerializedString.Append(this.mySerializedString.ToString());
        }

        public MonsterGroup(List<MonsterLevel> PossibleMonsters, int MaxSize, long GroupId)
        {
            this.ItemLoot = new List<ItemLoot>();
            this.ActorId = GroupId;
            this.Alignement = -1;
            this.IsFix = false;

            this.Monsters = new Dictionary<int, MonsterLevel>();

            var MonsterNum = 0;
            var RandomNum = 0;


            switch (MaxSize)
            {
                case 0:
                    return;

                case 1:
                    MonsterNum = 1;
                    break;

                case 2:
                    MonsterNum = RANDOM.Next(1, 2);
                    break;

                case 3:
                    MonsterNum = RANDOM.Next(1, 3);
                    break;
                case 4:
                    RandomNum = RANDOM.Next(0, 99);
                    if (RandomNum < 22) //1:22%
                    {
                        MonsterNum = 1;
                    }
                    else if (RandomNum < 48) //2:26%
                    {
                        MonsterNum = 2;
                    }
                    else if (RandomNum < 74) //3:26%
                    {
                        MonsterNum = 3;
                    }
                    else //4:26%
                    {
                        MonsterNum = 4;
                    }
                    break;

                case 5:
                    RandomNum = RANDOM.Next(0, 99);
                    if (RandomNum < 15) //1:22%
                    {
                        MonsterNum = 1;
                    }
                    else if (RandomNum < 38) //2:26%
                    {
                        MonsterNum = 2;
                    }
                    else if (RandomNum < 60) //3:26%
                    {
                        MonsterNum = 3;
                    }
                    else if (RandomNum < 85) //3:26%
                    {
                        MonsterNum = 4;
                    }
                    else //4:26%
                    {
                        MonsterNum = 5;
                    }
                    break;

                case 6:
                    RandomNum = RANDOM.Next(0, 99);
                    if (RandomNum < 10) //1:22%
                    {
                        MonsterNum = 1;
                    }
                    else if (RandomNum < 25) //2:26%
                    {
                        MonsterNum = 2;
                    }
                    else if (RandomNum < 45) //3:26%
                    {
                        MonsterNum = 3;
                    }
                    else if (RandomNum < 65) //3:26%
                    {
                        MonsterNum = 4;
                    }
                    else if (RandomNum < 85) //3:26%
                    {
                        MonsterNum = 5;
                    }
                    else //4:26%
                    {
                        MonsterNum = 6;
                    }
                    break;

                case 7:
                    RandomNum = RANDOM.Next(0, 99);
                    if (RandomNum < 9) //1:22%
                    {
                        MonsterNum = 1;
                    }
                    else if (RandomNum < 20) //2:26%
                    {
                        MonsterNum = 2;
                    }
                    else if (RandomNum < 35) //3:26%
                    {
                        MonsterNum = 3;
                    }
                    else if (RandomNum < 55) //3:26%
                    {
                        MonsterNum = 4;
                    }
                    else if (RandomNum < 75) //3:26%
                    {
                        MonsterNum = 5;
                    }
                    else if (RandomNum < 90) //3:26%
                    {
                        MonsterNum = 6;
                    }
                    else //4:26%
                    {
                        MonsterNum = 7;
                    }
                    break;

                default:
                    RandomNum = RANDOM.Next(0, 99);
                    if (RandomNum < 9) //1:22%
                    {
                        MonsterNum = 1;
                    }
                    else if (RandomNum < 20) //2:26%
                    {
                        MonsterNum = 2;
                    }
                    else if (RandomNum < 33) //3:26%
                    {
                        MonsterNum = 3;
                    }
                    else if (RandomNum < 50) //3:26%
                    {
                        MonsterNum = 4;
                    }
                    else if (RandomNum < 67) //3:26%
                    {
                        MonsterNum = 5;
                    }
                    else if (RandomNum < 80) //3:26%
                    {
                        MonsterNum = 6;
                    }
                    else if (RandomNum < 91) //3:26%
                    {
                        MonsterNum = 7;
                    }
                    else //4:26%
                    {
                        MonsterNum = 8;
                    }
                    break;
            }

            // Aucun monstre de l'alignement demandé
            if (!PossibleMonsters.Any(x => x.Monster.Alignement == Alignement))
            {
                return;
            }

            var MonsterGUID = -1;
            var MaxLevel = 0;

            for (int i = 0; i < MonsterNum; i++)
            {
                MonsterLevel MonsterGrade = null;

                do
                {
                    var RandIndex = RANDOM.Next(0, PossibleMonsters.Count - 1);
                    MonsterGrade = PossibleMonsters[RandIndex];
                }
                while (MonsterGrade.Monster.Alignement != this.Alignement);

                if (MonsterGrade.Level > MaxLevel)
                    MaxLevel = MonsterGrade.Level;

                this.Monsters.Add(MonsterGUID--, MonsterGrade);
            }

            this.Aggrodistance = Pathfinder.GetAggroDistanceByLevel(MaxLevel);
            this.Orientation = Pathfinder.ForViewOrientation(RANDOM.Next(0, 7));
        }

        public MonsterGroup(long GroupId, String groupData)
        {
            this.ItemLoot = new List<ItemLoot>();
            this.ActorId = GroupId;
            this.Alignement = -1;
            this.IsFix = true;

            this.Monsters = new Dictionary<int, MonsterLevel>();

            var MaxLevel = 0;
            var MonsterGUID = -1;
            foreach (String data in groupData.Split(';'))
            {
                String[] infos = data.Trim().Split(',');
                try{
                    int id = int.Parse(infos[0]);
                    int min = int.Parse(infos[1]);
                    int max = int.Parse(infos[2]);
                    Monster m = MonsterTable.GetMonster(id);
                    m.Initialize();
                    var mgs = new List<MonsterLevel>();
                    foreach (var MG in m.GetMobs())
                    {
                        if (MG.Level >= min && MG.Level <= max)
                        {
                            if (MG.Level > MaxLevel)
                                MaxLevel = MG.Level;

                            mgs.Add(MG);
                        }
                    }
                    if (mgs.Count < 1)
                    {
                        continue;
                    }
                    this.Monsters.Add(MonsterGUID--, mgs[Algo.getRandomValue(0, mgs.Count() - 1)]);
                }
                catch (Exception e)
                {
                    continue;
                };
            }
            this.Aggrodistance = Pathfinder.GetAggroDistanceByLevel(MaxLevel);
            this.Orientation = Pathfinder.ForViewOrientation(RANDOM.Next(0, 7));
        }

        public void AddItemLoot(ItemLoot ItemLoot)
        {
            lock (this.ItemLoot)
                this.ItemLoot.Add(ItemLoot);
        }

        public void AddKamasLoot(long Value)
        {
            this.KamasLoot += Value;
        }
    }
}
