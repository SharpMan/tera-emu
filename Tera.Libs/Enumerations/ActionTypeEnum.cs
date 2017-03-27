using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.Libs.Enumerations
{
    public enum ActionTypeEnum
    {
        CreateGuild = -2,
        OpenBank = -1,
        Teleport = 0,
        GiveQuestion = 1,
        ToogleOnMount = 2,
        GiveKamas = 4,
        GiveItem = 5,
        LearnJob = 6,
        WarpToSavePos = 7,
        AddStat = 8,
        LearnSpell = 9,
        EatOrDrinkItem = 10,
        ModifAlignement = 11,
        SpawnInArena = 12,
        ResetStats = 13,
        OpenPanelToForgetSpell = 14,
        EnterToDonjon = 15,
        AddHonor = 16,
        AddJobXP = 17,
        TeleportToMyHouse = 18,
        GuildHouse = 19,
        AddSpellPoints = 20,
        GiveEnergy = 21,
        AddXP = 22,
        UnLearnJob = 23,
        SetLook = 24,
        UnLook = 25,
        GuildEnclo = 26,
        Traque = 50,
        CibleTraque = 51,
        GiveTraqueRecompense = 52,
        UpdateMountAbility = 100,
        StopInMarriageCell = 101,
        DoMarriage = 102,
        DivorceMarriage = 103,
        SendMessage = 104,
        SetPDVPER = 105,
        SetTitle = 106,
        PlacePrisme = 201,
    }
}
