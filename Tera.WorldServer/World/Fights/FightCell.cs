using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights
{
    public sealed class FightCell
    {
        public int Id;
        private bool myWalkable;
        private bool myLineOfSight;

        private List<IFightObject> myFightObjects = new List<IFightObject>();

        public FightCell(int Id, bool walk, bool los)
        {
            this.Id = Id;
            this.myWalkable = walk;
            this.myLineOfSight = los;
        }

        public bool LineOfSight()
        {
            return myLineOfSight;
        }

        public bool IsWalkable()
        {
            return this.myWalkable;
        }

        public bool CanWalk()
        {
            return this.myWalkable && !this.HasGameObject(FightObjectType.OBJECT_STATIC) && !this.HasGameObject(FightObjectType.OBJECT_FIGHTER);
        }

        public bool HasUnWalkableFighter()
        {
            return this.HasGameObject(FightObjectType.OBJECT_STATIC) || this.HasGameObject(FightObjectType.OBJECT_FIGHTER);
        }

        public bool HasGameObject(FightObjectType ObjectType)
        {
            return myFightObjects.Any(x => x.ObjectType == ObjectType);
        }

        public List<T> GetObjects<T>() where T : IFightObject
        {
            return this.myFightObjects.OfType<T>().ToList();
        }

        public Fighter GetFighter()
        {
            if (HasUnWalkableFighter()) return this.GetObjects<Fighter>()[0];
            else return null;
        }

        public List<T> GetObjects<T>(FightObjectType ObjectType) where T : IFightObject
        {
            return this.myFightObjects.OfType<T>().ToList().FindAll(x => x.ObjectType == ObjectType);
        }

        public Fighter HasEnnemy(FightTeam Team)
        {
            if (!this.HasGameObject(FightObjectType.OBJECT_FIGHTER) && !this.HasGameObject(FightObjectType.OBJECT_STATIC))
                return null;

            return (this.GetObjects<Fighter>()[0].Team.Id != Team.Id && !this.GetObjects<Fighter>()[0].Dead) ? this.GetObjects<Fighter>()[0] : null; //Class not ID ...
        }

        public bool HasIFightObject(IFightObject Fobject){
            return this.myFightObjects.Contains(Fobject);
        }

        public Fighter HasFriend(FightTeam Team)
        {
            if (!this.HasGameObject(FightObjectType.OBJECT_FIGHTER) && !this.HasGameObject(FightObjectType.OBJECT_STATIC))
                return null;

            return (this.GetObjects<Fighter>()[0].Team.Id == Team.Id && !this.GetObjects<Fighter>()[0].Dead) ? this.GetObjects<Fighter>()[0] : null; //Class not ID ...
        }

        public bool HasSimilarType(IFightObject obj, FightObjectType[] types = null)
        {
             return this.myFightObjects.Any(x => x != obj 
                    && types==null?
                        (x.ObjectType == obj.ObjectType)
                        :
                        (types.Contains(x.ObjectType))
                    );
        }
        public List<IFightObject> GetSimilarObjects(IFightObject obj, FightObjectType[] types = null)
        {
            return this.myFightObjects.FindAll(x => x != obj
                   && types == null ?
                       (x.ObjectType == obj.ObjectType)
                       :
                       (types.Contains(x.ObjectType))
                   );
        }

        public int AddObject(IFightObject Object)
        {
            this.myFightObjects.Add(Object);

            // TODO ACTIVE FIGHTOBJECT TRAP ETC ...

            return -1;
        }

        public void RemoveObject(IFightObject Object)
        {
            this.myFightObjects.Remove(Object);
        }
    }
}
