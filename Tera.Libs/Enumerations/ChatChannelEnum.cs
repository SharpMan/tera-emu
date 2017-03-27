using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.Libs.Enumerations
{
    public enum ChatChannelEnum : int
    {
        CHANNEL_GENERAL = '*',
        CHANNEL_RECRUITMENT = '?',
        CHANNEL_DEALING = ':',
        CHANNEL_TEAM = '#',
        CHANNEL_ADMIN = '@',
        CHANNEL_GROUP = '$',
        CHANNEL_GUILD = '%',
        CHANNEL_ALIGNMENT = '!',
        CHANNEL_PRIVATE_SEND = 'T',
        CHANNEL_PRIVATE_RECIEVE = 'F',
        CHANNEL_POINT,
    }

  

}
