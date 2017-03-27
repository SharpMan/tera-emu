using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.Libs.Enumerations
{
    public enum GuildRightEnum
    {
        //Valeur des droits de guilde
        RIGHT_BOOST = 2,			//G�rer les boost
        RIGHT_SET_RIGHT = 4,			//G�rer les droits
        RIGHT_INVIT = 8,			//Inviter de nouveaux membres
        RIGHT_BAN = 16,				//Bannir
        RIGHT_SET_ALLXP = 32,			//G�rer les r�partitions d'xp
        RIGHT_SET_GRADE = 64,			//G�rer les rangs
        RIGHT_PUT_PERCEPTOR = 128,		//Poser un percepteur
        RIGHT_SET_XP = 256,			//G�rer sa r�partition d'xp
        RIGHT_COLLECT_PERCEPTOR = 512,		//Collecter les percepteurs
        RIGHT_USE_MOUNTPARK = 4096,		//Utiliser les enclos
        RIGHT_CONFIG_MOUNTPARK = 8192,	//Am�nager les enclos
        RIGHT_MANAGE_OTHERSMOUNT = 16384,		//G�rer les montures des autres membres
    }
}
