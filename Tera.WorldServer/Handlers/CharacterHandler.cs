using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Network;
using Tera.WorldServer.Utils;
using Tera.WorldServer.World.Character;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Handlers
{
    public static class CharacterHandler
    {
        public static Dictionary<int, EffectEnum> BOOST_ID_TO_STATS = new Dictionary<int, EffectEnum>()
        {
            {10, EffectEnum.AddForce},
            {11, EffectEnum.AddVitalite},
            {12, EffectEnum.AddSagesse},
            {13, EffectEnum.AddChance},
            {14, EffectEnum.AddAgilite},
            {15, EffectEnum.AddIntelligence},
        };

        public static void ProcessBoostStatsRequest(WorldClient Client, string Packet)
        {
            int StatsId = 0,capital = 1;

            if (!int.TryParse(Packet.Substring(2,2), out StatsId))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var EffectType = EffectEnum.None;

            if (!CharacterHandler.BOOST_ID_TO_STATS.TryGetValue(StatsId, out EffectType))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Character = Client.Character;

            var StatsValue = Character.GetStats().GetEffect(EffectType).Base;
            var BoostValue = StatsId == 11 && (ClassEnum)Character.Classe == ClassEnum.CLASS_SACRIEUR ? 2 : 1;

            if (Settings.AppSettings.GetBoolElement("Extra.ElbustCore"))
            {
                try
                {
                    capital = int.Parse(Packet.Split(';')[1]);
                    int quantite = 0;
                    if (capital > Client.Character.CaractPoint)
                    {
                        capital = Client.Character.CaractPoint;
                    }
                    while (capital >= quantite)
                    {
                        StatsValue = Character.GetStats().GetEffect(EffectType).Base;
                        quantite = GenericStats.GetRequiredStatsPoint((ClassEnum)Character.Classe, StatsId, StatsValue);
                        if (capital >= quantite)
                        {
                            lock (Client.BoostStatsSync)
                            {
                                switch (EffectType)
                                {
                                    case EffectEnum.AddForce:
                                        Character.Strength += BoostValue;
                                        break;

                                    case EffectEnum.AddVitalite:
                                        Character.Vitality += BoostValue;
                                        Character.Life += BoostValue; // on boost la life
                                        break;

                                    case EffectEnum.AddSagesse:
                                        Character.Wisdom += BoostValue;
                                        break;

                                    case EffectEnum.AddIntelligence:
                                        Character.Intell += BoostValue;
                                        break;

                                    case EffectEnum.AddChance:
                                        Character.Chance += BoostValue;
                                        break;

                                    case EffectEnum.AddAgilite:
                                        Character.Agility += BoostValue;
                                        break;
                                }
                                Character.GetStats().AddBase(EffectType, BoostValue);
                                Character.CaractPoint -= quantite;
                                capital -= quantite;
                            }
                        }
                    }
                    Client.Send(new AccountStatsMessage(Client.Character));
                    return;
                }
                catch (Exception e)
                {
                }
            }

            var RequiredStatsPoints = GenericStats.GetRequiredStatsPoint((ClassEnum)Character.Classe, StatsId, StatsValue);

            if (Character.CaractPoint < RequiredStatsPoints)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }


            lock (Client.BoostStatsSync)
            {
                switch (EffectType)
                {
                    case EffectEnum.AddForce:
                        Character.Strength += BoostValue;
                        break;

                    case EffectEnum.AddVitalite:
                        Character.Vitality += BoostValue;
                        Character.Life += BoostValue; // on boost la life
                        break;

                    case EffectEnum.AddSagesse:
                        Character.Wisdom += BoostValue;
                        break;

                    case EffectEnum.AddIntelligence:
                        Character.Intell += BoostValue;
                        break;

                    case EffectEnum.AddChance:
                        Character.Chance += BoostValue;
                        break;

                    case EffectEnum.AddAgilite:
                        Character.Agility += BoostValue;
                        break;
                }

                Character.GetStats().AddBase(EffectType, BoostValue);
                Character.CaractPoint -= RequiredStatsPoints;
            }

            Client.Send(new AccountStatsMessage(Client.Character));
        }

    }
}
