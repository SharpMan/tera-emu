using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Tables;

namespace Tera.WorldServer.Database.Models
{
    public class AccountDataModel
    {
        public int Guid { get; set; }
        public String Bank { get; set; }
        public long BankKamas { get; set; }
        public String Stables { get; set; }
        public String Friends { get; set; }
        public String Ennemys { get; set; }
        public bool showFriendConnection { get; set; }

        public Dictionary<int, String> FriendsList = new Dictionary<int, string>();
        public Dictionary<int, String> EnnemyList = new Dictionary<int, string>();
        public Dictionary<long, InventoryItemModel> bankItems = new Dictionary<long, InventoryItemModel>();
        public Dictionary<int, Mount> Mounts = new Dictionary<int, Mount>();
        public Dictionary<int, List<BidHouseItem>> BHI;

        public bool myInitialized = false;

        public void Initialize()
        {
            if (myInitialized) return;
            try
            {
                BHI = new Dictionary<int, List<BidHouseItem>>();
                if (!BidHouseTable.BHITEMS.ContainsKey(this.Guid))
                {
                    BidHouseTable.BHITEMS.Add(Guid,new Dictionary<int, List<BidHouseItem>>());
                }
                BHI = BidHouseTable.BHITEMS[Guid];
                foreach (String s in Friends.Split(';'))
                {
                    if (String.IsNullOrEmpty(s))
                    {
                        continue;
                    }
                    FriendsList.Add(Convert.ToInt32(s.Split('|')[0]), s.Split('|')[1]);
                }
                foreach (String s in Ennemys.Split(';'))
                {
                    if (String.IsNullOrEmpty(s))
                    {
                        continue;
                    }
                    EnnemyList.Add(Convert.ToInt32(s.Split('|')[0]), s.Split('|')[1]);
                }
                foreach (String s in Bank.Split(';'))
                {
                    if (String.IsNullOrEmpty(s))
                    {
                        continue;
                    }
                    long id;
                    if(!long.TryParse(s,out id))
                    {
                        continue;
                    }
                    InventoryItemModel obj = InventoryItemTable.getItem(id);
                    if (obj != null)
                    {
                        bankItems.Add(id, obj);
                    }
                    else
                    {
                        obj = InventoryItemTable.Load(id);
                        if (obj != null)
                        {
                            bankItems.Add(id, obj);
                        }
                    }
                }
                foreach (String s in Stables.Split(';'))
                {
                    if (String.IsNullOrEmpty(s))
                    {
                        continue;
                    }
                    int id;
                    if (!int.TryParse(s, out id))
                    {
                        continue;
                    }
                    Mount DD = MountTable.getMount(id);
                    if (DD != null)
                    {
                        Mounts.Add(id, DD);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            myInitialized = true;
        }

        public BidHouseItem[] getBidHouseItemDeposed(int BHID)
        {
            if (!this.BHI.ContainsKey(BHID))
                return new BidHouseItem[1];
            BidHouseItem[] ListBidHouseItems = new BidHouseItem[20];
            for (int i = 0; i < this.BHI[BHID].Count; i++)
            {
                ListBidHouseItems[i] = this.BHI[BHID][i];
            }
            return ListBidHouseItems;
        }

        public int canTaxBidHouseItem(int BHID)
        {
            if (!this.BHI.ContainsKey(BHID))
                return 0;
            return BHI[BHID].Count;
        }

        public void Save()
        {
            this.Friends = "";
            foreach(KeyValuePair<int,String> Key in this.FriendsList)
            {
                this.Friends += Key.Key + "|" + Key.Value + ";";
            }
            this.Ennemys = "";
            foreach (KeyValuePair<int, String> Key in this.EnnemyList)
            {
                this.Ennemys += Key.Key + "|" + Key.Value + ";";
            }
            this.Bank = "";
            foreach (KeyValuePair<long, InventoryItemModel> KeyPair in this.bankItems)
            {
                this.Bank += KeyPair.Key + ";";
                InventoryItemTable.Update(KeyPair.Value);
            }
            this.Stables = "";
            foreach (KeyValuePair<int, Mount> KeyPair in this.Mounts)
            {
                this.Stables += KeyPair.Key + ";";
            }
            AccountDataTable.Update(this);
        }

    }
}
