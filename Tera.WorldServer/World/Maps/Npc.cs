using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;

namespace Tera.WorldServer.World.Maps
{
    public class Npc : IGameActor
    {

        private Dictionary<int, ItemTemplateModel> myItems = new Dictionary<int, ItemTemplateModel>();
        private StringBuilder mySerializedPattern;
        private StringBuilder mySerializedItems;
        private bool myInitialized = false;
        private long myActorId;
        private NpcTemplateModel TemplateCache;
        private int _cellID;
        private int _orientation;


        public Npc(NpcTemplateModel temp, int cell, int o)
        {
            TemplateCache = temp;
            _cellID = cell;
            _orientation = o;
        }

        public NpcTemplateModel get_template()
        {
            return TemplateCache;
        }



        public void Initialize(long Id)
        {
            this.myActorId = Id;
        }


        public long ActorId
        {
            get { return this.myActorId; }
        }

        public int Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                _orientation = value;
            }
        }

        public int CellId
        {
            get
            {
                return _cellID;
            }
            set
            {
                _cellID = value;
            }
        }

        public GameActorTypeEnum ActorType
        {
            get { return GameActorTypeEnum.TYPE_NPC; }
        }

        public void SerializeAsGameMapInformations(StringBuilder SerializedString)
        {
            if (this.mySerializedPattern == null)
            {
                this.mySerializedPattern = new StringBuilder();
                this.mySerializedPattern.Append(this.CellId).Append(';');
                this.mySerializedPattern.Append(this.Orientation).Append(';');
                this.mySerializedPattern.Append('0').Append(';'); // Unknow
                this.mySerializedPattern.Append(this.ActorId).Append(';');
                this.mySerializedPattern.Append(this.TemplateCache.ID).Append(';');
                this.mySerializedPattern.Append((int)this.ActorType).Append(';');
                this.mySerializedPattern.Append(this.TemplateCache.SkinID).Append('^');
                this.mySerializedPattern.Append(this.TemplateCache.ScaleX).Append('x'); // SkinSize
                this.mySerializedPattern.Append(this.TemplateCache.ScaleY).Append(';');
                this.mySerializedPattern.Append(this.TemplateCache.Sexe).Append(';');
                this.mySerializedPattern.Append(this.PatternColors(';')).Append(';');
                this.mySerializedPattern.Append(this.TemplateCache.Accessories).Append(';');
                this.mySerializedPattern.Append(this.TemplateCache.ExtraClip == -1 ? "" : this.TemplateCache.ExtraClip.ToString()).Append(';');
                this.mySerializedPattern.Append(this.TemplateCache.CustomArtWork);
            }

            SerializedString.Append(this.mySerializedPattern.ToString());
        }

        public string PatternColors(char Separator)
        {
            return (this.TemplateCache.Color1 == -1 ? "-1" : this.TemplateCache.Color1.ToString("x")) + Separator +
                   (this.TemplateCache.Color2 == -1 ? "-1" : this.TemplateCache.Color2.ToString("x")) + Separator +
                   (this.TemplateCache.Color3 == -1 ? "-1" : this.TemplateCache.Color1.ToString("x"));
        }

        public void SerializeAsItemList(StringBuilder SerializedString)
        {
            if (this.mySerializedItems == null)
            {
                if (!this.myInitialized)
                    this.Initialize();
            }

            SerializedString.Append(this.mySerializedItems.ToString());
        }

        private void Initialize()
        {
            if (this.myInitialized)
                return;

            this.mySerializedItems = new StringBuilder();

            if ((this.TemplateCache.Ventes != string.Empty || this.TemplateCache.Ventes != "-1") && this.TemplateCache.Ventes.Contains(','))
            {
                var TemplateIds = this.TemplateCache.Ventes.Split(',');

                foreach (var TemplateId in TemplateIds)
                {
                    try
                    {
                        var Template = ItemTemplateTable.GetTemplate(int.Parse(TemplateId));

                        if (Template != null)
                        {
                            if (!this.myItems.ContainsKey(Template.ID))
                            {
                                this.mySerializedItems.Append(Template.ID);
                                this.mySerializedItems.Append(';');
                                this.mySerializedItems.Append(Template.StatsTemplate);
                                this.mySerializedItems.Append('|');

                                this.myItems.Add(Template.ID, Template);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Fail to Int::Parse " + TemplateId);
                    }
                }
            }

            this.myInitialized = true;
        }

        public bool HasItemTemplate(int TemplateId)
        {
            return this.myItems.ContainsKey(TemplateId);
        }

        public ItemTemplateModel GetItemTemplate(int TemplateId)
        {
            return this.myItems[TemplateId];
        }
    }
}
