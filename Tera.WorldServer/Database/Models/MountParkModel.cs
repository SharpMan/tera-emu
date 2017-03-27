using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.World.Maps;

namespace Tera.WorldServer.Database.Models
{
    public class MountPark
    {
        private long _owner;
        private Object _door;
        private int _size;
        private List<DofusCell> _cases = new List<DofusCell>();
        private Guild _guild;
        private Map _map;
        private int _cellid = -1;
        private int _price;
        

        public MountPark(long owner, Map map, int cellid, int size, String data, int guild, int price)
        {
            _owner = owner;
            _door = map.getMountParkDoor();
            _size = size;
            _guild = GuildTable.GetGuild(guild);
            _map = map;
            _cellid = cellid;
            _price = price;
            if (_map != null)
            {
                _map.mountPark = this;
            }
        }

        public long get_owner()
        {
            return _owner;
        }

        public void set_owner(long AccID)
        {
            _owner = AccID;
        }

        public Object get_door()
        {
            return _door;
        }

        public int get_size()
        {
            return _size;
        }

        public Guild get_guild()
        {
            return _guild;
        }

        public void set_guild(Guild guild)
        {
            _guild = guild;
        }

        public Map get_map()
        {
            return _map;
        }

        public int get_cellid()
        {
            return _cellid;
        }

        public int get_price()
        {
            return _price;
        }

        public void set_price(int price)
        {
            _price = price;
        }

        public int getObjectNumb() {
			int n = 0;
			foreach (DofusCell C in _cases) {
				if (C.Object != null) {
					n++;
				}
			}
			return n;
		}

        public String parseData()
        {
            String str = "";
            return str;
        }
    }
}
