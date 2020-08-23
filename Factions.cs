using System;
using System.Collections.Generic;
using System.Linq;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;
using Oxide.Game.Rust.Cui;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Factions", "Whisper", "1.0.0", ResourceId = 0)]
    [Description("")]
    class Factions : RustPlugin
    {
        static Factions ins;
        static Dictionary<ulong, Player> cachedPlayers = new Dictionary<ulong, Player>();
        private void OnServerInitialized()
        {
            ins = this;
            //permission.RegisterPermission ("ELORanks.Admin", this);
            foreach (BasePlayer bPlayer in BasePlayer.activePlayerList) OnPlayerInit(bPlayer);
        }
        private void OnPlayerInit(BasePlayer bPlayer) => Player.TryLoad(bPlayer);

        private bool CanLock(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        private bool CanDeployItem(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        private bool OnPayForUpgrade(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        private bool OnPayForPlacement(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        private bool CanUseWires(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        private bool CanBuild(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        private bool CanChangeCode(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        private bool CanChangeGrade(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        private bool CanDemolish(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        private bool CanPickupEntity(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        private bool CanRenameBed(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        private bool CanPickupLock(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        private bool CanUnlock(BasePlayer bPlayer) => Fact.CanAct(bPlayer);

        private bool OnPlayerAttack(BasePlayer bPlayer, HitInfo info) => Fact.CanAttack(bPlayer, info?.Initiator);
        private bool OnMeleeAttack(BasePlayer bPlayer, HitInfo info) => Fact.CanAttack(bPlayer, info?.Initiator);
        private bool OnPlayerAssist(BasePlayer bPlayer, HitInfo info) => Fact.CanAttack(bPlayer, info?.Initiator);

        private void Unload()
        {
            foreach (var data in cachedPlayers) data.Value.Save(data.Key);
        }

        public class Player
        {
            public string userName;
            public string faction;
            internal static void TryLoad(ulong id, string uname)
            {
                if (cachedPlayers.ContainsKey(id)) return;

                Player data = Interface.Oxide.DataFileSystem.ReadObject<Player>($"Factions/Players/{id}");

                if (data == null)
                {
                    data = new Player();
                    data.userName = uname;
                }

                cachedPlayers.Add(id, data);
            }

            internal void Save(ulong id)
            {
                Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Players/{id}"), this, true);
            }
        }
        public class Fact
        {
            public string name = "";
            public string tag = "";
            public int power = 100;
            public List<ulong> members = new List<ulong>();
            public List<string> allies = new List<string>();
            public List<string> enemies = new List<string>();
            public List<string> chunks = new List<string>();
            public ulong chieftain = new ulong();
            public string colour = "#00ff00";
            // #ff9900 #6699ff #ff00ff #ff9999 #339933 #ffcc99".Split(" ")[Math.Round(Random()*8 - 0.5)] || "#ff0000";

            internal static bool CanAct(BasePlayer bPlayer)
            {
                string owner = Chunk.Entered(bPlayer);

                if (owner != "Wilderness")
                {
                    Player p = Interface.Oxide.DataFileSystem.ReadObject<Player>($"Factions/Players/{bPlayer.userID}");

                    if (p == null)
                    {
                        return false;
                    }

                    Fact fac = Interface.Oxide.DataFileSystem.ReadObject<Fact>($"Factions/Factions/{p.Faction}");

                    if (fac != null)
                    {
                        if (fac.name != player.faction)
                        {
                            return false;
                        }
                    }
                }

                return null;
            }

            internal static bool CanAttack(BasePlayer victim, BasePlayer attacker)
            {
                string victimChunk = Chunk.Entered(victim);
                string attackerChunk = Chunk.Entered(victim);

                if (victimChunk == "Wilderness" && attacker == "Wilderness")
                {
                    return null;
                }

                return false;
            }

            internal static string Info(BasePlayer bPlayer, string name)
            {
                Fact query = Interface.Oxide.DataFileSystem.ReadObject<Fact>($"Factions/Factions/{name}");

                if (query == null)
                {
                    return "Invalid Faction Name";
                }

                string playerList = "";

                foreach (string p in query.Players)
                {
                    Player plr = Interface.Oxide.DataFileSystem.ReadObject<Player>($"Factions/Players/{bPlayer.userID}");

                    if (plr != null)
                    {
                        playerList += plr.userName;
                        playerList += "<br>";
                    }
                }

                return $"<color = {query.colour}>Faction: {query.name}</color><br>Power: {query.power}<br><color = {"#00ffff"}>Members:</color><br>{playerList}";
            }
        }

        public class Chunk
        {
            int X;
            int Y;
            string faction;

            internal static string Entered(BasePlayer bPlayer)
            {
                Vector3 pos = bPlayer.transform.position;
                int X = (int)Math.Floor(pos.x / 50);
                int Y = (int)Math.Floor(pos.y / 50);
                Chunk data = Interface.Oxide.DataFileSystem.ReadObject<Chunk>($"Factions/Chunks/{X},{Y}");

                if (data == null)
                {
                    return "Wilderness";
                }
                else
                {
                    return data.faction;
                }
            }
        }

        [ChatCommand("f")]
        private void fCommand(BasePlayer bPlayer, string command, string[] args)
        {
            OnPlayerInit(bPlayer);

            if (args[0].ToLower() == "create")
            {
                string name = command.ToLower().Replace("!f ", "").Replace("!factions ", "").Replace("create ", "").Replace(" ", "_").ToLower();

                Fact data = Interface.Oxide.DataFileSystem.ReadObject<Fact>($"Factions/Factions/{name}");

                if (data == null)
                {
                    data = new Fact();
                    data.name = name;
                    data.chieftain = bPlayer.userID;
                    data.members.Add(bPlayer.userID);

                    Player p = Interface.Oxide.DataFileSystem.ReadObject<Player>($"Factions/Players/{bPlayer.userID}");

                    if (p == null)
                    {
                        PrintToChat(bPlayer, "Something went wrong. Error: x001");
                    }

                    p.faction = name;

                    Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Factions/{name}"), data, true);
                    Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Players/{bPlayer.userID}"), p, true);
                    PrintToChat(bPlayer, $"Faction {name} Created");
                }
                else
                {
                    PrintToChat(bPlayer, $"Faction Name {name} Already in use");
                }
            }

            if (args[0].ToLower() == "claim")
            {
                Player p = Interface.Oxide.DataFileSystem.ReadObject<Player>($"Factions/Players/{bPlayer.userID}");

                Fact claimer = Interface.Oxide.DataFileSystem.ReadObject<Fact>($"Factions/Factions/{p.Faction}");

                if (claimer == null)
                {
                    PrintToChat(bPlayer, "You are not in a Faction!");
                }

                if (claimer.chieftain != bPlayer.userID)
                {
                    PrintToChat(bPlayer, "Only your Factions Chieftain may claim land");
                }

                Vector3 pos = bPlayer.transform.position;
                int X = (int)Math.Floor(pos.x / 50);
                int Y = (int)Math.Floor(pos.y / 50);
                Chunk data = Interface.Oxide.DataFileSystem.ReadObject<Chunk>($"Factions/Chunks/{X},{Y}");

                if (data != null)
                {
                    if (data.faction != "")
                    {
                        Fact claimee = Interface.Oxide.DataFileSystem.ReadObject<Fact>($"Factions/Factions/{data.Faction}");

                        if (claimee == null)
                        {
                            PrintToChat(bPlayer, "Something went wrong. Contact an Administrator! error x002");
                        }

                        if (claimer.name == claimee.name)
                        {
                            PrintToChat(bPlayer, "You already own this land!");
                        }

                        if (claimer.power > claimee.power)
                        {
                            data.faction = claimer.name;
                            claimer.chunks.Add($"{X},{Y}");
                            claimee.chunks.Remove($"{X},{Y}");

                            claimer.power -= 50;
                            claimee.power += 50;

                            Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Factions/{claimee.name}"), claimee, true);
                            Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Factions/{claimer.name}"), claimer, true);
                            Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Chunks/{X},{Y}"), data, true);
                            PrintToChat(bPlayer, "Land Claimed");
                        }
                    }
                }
                else if (claimer.power > 50)
                {
                    data = new Chunk();
                    data.Faction = faction.name;
                    data.X = X;
                    data.Y = Y;

                    claimer.power -= 50;

                    Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Factions/{claimer.name}"), claimer, true);
                    Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Chunks/{X},{Y}"), data, true);
                    PrintToChat(bPlayer, "Land Claimed");
                }
                else
                {
                    PrintToChat(bPlayer, "Need more power");
                }
            }

            if (args[0].ToLower() == "unclaim")
            {
                Player p = Interface.Oxide.DataFileSystem.ReadObject<Player>($"Factions/Players/{bPlayer.userID}");

                if (p == null)
                {
                    PrintToChat(bPlayer, "Something went wrong. Error: x001");
                }

                Fact unclaimer = Interface.Oxide.DataFileSystem.ReadObject<Fact>($"Factions/Factions/{p.Faction}");

                if (unclaimer == null)
                {
                    PrintToChat(bPlayer, "You are not in a Faction!");
                }

                if (unclaimer.chieftain != bPlayer.userID)
                {
                    PrintToChat(bPlayer, "Only your Factions Chieftain may unclaim land");
                }

                Vector3 pos = bPlayer.transform.position;
                int X = (int)Math.Floor(pos.x / 50);
                int Y = (int)Math.Floor(pos.y / 50);
                Chunk data = Interface.Oxide.DataFileSystem.ReadObject<Chunk>($"Factions/Chunks/{X},{Y}");

                if (data == null || data.faction != unclaimer.name)
                {
                    PrintToChat(bPlayer, "You do not own this Chunk!");
                }

                data.faction = "";

                unclaimer.power += 50;

                Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Factions/{unclaimer.name}"), unclaimer, true);
                Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Chunks/{X},{Y}"), data, true);
                PrintToChat(bPlayer, "Land Unclaimed");
            }

            if (args[0].ToLower() == "info")
            {
                PrintToChat(bPlayer, Fact.Info(bPlayer, command.ToLower().Replace("!f ", "").Replace("!factions ", "").Replace("info ", "").Replace(" ", "_").ToLower()));
            }
        }
    }
}