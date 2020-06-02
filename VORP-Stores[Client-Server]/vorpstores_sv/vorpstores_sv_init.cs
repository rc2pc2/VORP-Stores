﻿using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vorpstores_sv
{
    public class vorpstores_sv_init : BaseScript
    {
        public vorpstores_sv_init()
        {
            EventHandlers["vorpstores:buyItems"] += new Action<Player, string, int, double>(buyItems);
            EventHandlers["vorpstores:sellItems"] += new Action<Player, double, string>(sellItems);
        }
        private void buyItems([FromSource]Player source, string name, int quantity, double cost)
        {
            int _source = int.Parse(source.Handle);

            string sid = "steam:" + source.Identifiers["steam"];

            TriggerEvent("vorp:getCharacter", _source, new Action<dynamic>((user) =>
            {
                double money = user.money;
                double totalCost = (double)(cost * quantity);
                if (totalCost <= money)
                {
                    TriggerEvent("vorpCore:getItemCount", _source, new Action<dynamic>((itemcount) =>
                    {
                        int count = itemcount;
                        int limit = int.Parse(LoadConfig.ItemsFromDB[name]["limit"].ToString());
                        int hisLimit = limit - count;
                        if (quantity > hisLimit)
                        {
                            source.TriggerEvent("vorp:Tip", string.Format(LoadConfig.Langs["NoMore"], LoadConfig.ItemsFromDB[name]["label"].ToString()), 4000);
                        }
                        else
                        {
                            TriggerEvent("vorp:removeMoney", _source, 0, totalCost);
                            TriggerEvent("vorpCore:addItem", _source, name, quantity);
                            source.TriggerEvent("vorp:Tip", string.Format(LoadConfig.Langs["Buyed"], quantity, LoadConfig.ItemsFromDB[name]["label"].ToString(), totalCost.ToString()), 4000);
                        }

                    }), name);


                }
                else
                {
                    source.TriggerEvent("vorp:Tip", LoadConfig.Langs["NoMoney"], 4000);
                }

            }));
        }

        private void sellItems([FromSource]Player source, double totalCost, string jsonCloths)
        {

        }

    }
}
