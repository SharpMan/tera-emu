using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Maps
{
    public class IObject
    {
        private int _id;
        private int _state;
        private Map _map;
        private DofusCell _cell;
        private Boolean _interactive = true;
        private Timer _respawnTimer;
        private IObjectTemplate _template;

        public IObject(Map a_map, DofusCell a_cell, int a_id)
        {
            _id = a_id;
            _map = a_map;
            _cell = a_cell;
            _state = (int)IObjectEnum.STATE_FULL;
            int respawnTime = 10000;
            _template = IObjectTemplateTable.Get(_id);
            if (_template != null)
            {
                respawnTime = _template.RespawnTime;
            }
            if (respawnTime != -1)
            {
                _respawnTimer = new Timer(respawnTime);
                _respawnTimer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
                _respawnTimer.Enabled = true;
            }
        }

        public void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _respawnTimer.Stop();
            _state = (int)IObjectEnum.STATE_FULLING;
            _interactive = true;
            this._map.SendToMap(new MapObjectMessage(_map,_cell));
            _state = (int)IObjectEnum.STATE_FULL;
        }

        public int getID()
        {
            return _id;
        }

        public Boolean isInteractive()
        {
            return _interactive;
        }

        public void setInteractive(Boolean b)
        {
            _interactive = b;
        }

        public int getState()
        {
            return _state;
        }

        public void setState(int state)
        {
            _state = state;
        }

        public int getUseDuration()
        {
            int duration = 1500;
            if (_template != null)
            {
                duration = _template.Duration;
            }
            return duration;
        }

        public void startTimer()
        {
            if (_respawnTimer == null)
            {
                return;
            }
            _state = (int)IObjectEnum.STATE_EMPTY2;
            if (_respawnTimer.Enabled)
            {
                _respawnTimer.Stop();
            }
            _respawnTimer.Start();
        }

        public int getUnknowValue()
        {
            int unk = 4;
            if (_template != null)
            {
                unk = _template.Unk;
            }
            return unk;
        }

        public Boolean isWalkable()
        {
            if (_template == null)
            {
                return false;
            }
            return _template.Walakable && _state == (int)IObjectEnum.STATE_FULL;
        }
    }
}
