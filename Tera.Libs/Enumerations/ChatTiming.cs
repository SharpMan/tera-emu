using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.Libs.Enumerations
{
    public static class ChatTiming
    {
        private static Dictionary<ChatChannelEnum, int> myChatRestrictions = new Dictionary<ChatChannelEnum, int>()
        {
            { ChatChannelEnum.CHANNEL_ALIGNMENT, 58000 },
            { ChatChannelEnum.CHANNEL_DEALING, 58000 },
            { ChatChannelEnum.CHANNEL_GROUP, 500 },
            { ChatChannelEnum.CHANNEL_GENERAL, 500 },
            { ChatChannelEnum.CHANNEL_GUILD, 750 },
            { ChatChannelEnum.CHANNEL_RECRUITMENT, 58000 },
        };

        public static bool Exist(ChatChannelEnum Channel)
        {
            return myChatRestrictions.ContainsKey(Channel);
        }

        public static int getTime(ChatChannelEnum Channel)
        {
            return myChatRestrictions[Channel];
        }

    }
}
