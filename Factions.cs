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
    [Info("Factions", "Whisper", "0.1.0", ResourceId = 0)]
    [Description("Factions plugin for the game Rust using the umod API ")]
    class Factions : RustPlugin
    {
        static Factions ins;
        static Dictionary<ulong, Player> cachedPlayers = new Dictionary<ulong, Player>();
        private void OnServerInitialized()
        {
            ins = this;
            //permission.RegisterPermission ("ELORanks.Admin", this);
            foreach (BasePlayer bPlayer in BasePlayer.activePlayerList) OnPlayerInit(bPlayer);

            timer.Every(3f, () =>
            {
                foreach (BasePlayer bPlayer in BasePlayer.activePlayerList)
                {
                    int xC = -1;
                    int yC = -1;
                    for (xC = -1; xC < 2; xC++)
                    {
                        for (yC = -1; yC < 2; yC++)
                        {
                            Vector3 pos = bPlayer.transform.position;
                            int X = (int)Math.Floor(pos.x / 50) + xC;
                            int Y = (int)Math.Floor(pos.y / 50) + yC;
                            int zC = 0;
                            Chunk data = Interface.Oxide.DataFileSystem.ReadObject<Chunk>($"Factions/Chunks/{X},{Y}");

                            if (data.faction != null)
                            {
                                for (zC = 0; zC < 100; zC += 10)
                                {
                                    Vector3 from;
                                    Vector3 to;

                                    from.x = X * 50;
                                    from.y = Y * 50;
                                    from.z = zC;

                                    to.x = X * 50 + 50;
                                    to.y = Y * 50;
                                    to.z = zC + 10;

                                    bPlayer.SendConsoleCommand("ddraw.line", 1, "ff0000", from, to);
                                }

                                for (zC = 0; zC < 100; zC += 10)
                                {
                                    Vector3 from;
                                    Vector3 to;

                                    from.x = X * 50 + 50;
                                    from.y = Y * 50;
                                    from.z = zC;

                                    to.x = X * 50 + 50;
                                    to.y = Y * 50 + 50;
                                    to.z = zC + 10;

                                    bPlayer.SendConsoleCommand("ddraw.line", 1, "ff0000", from, to);
                                }

                                for (zC = 0; zC < 100; zC += 10)
                                {
                                    Vector3 from;
                                    Vector3 to;

                                    from.x = X * 5 + 50;
                                    from.y = Y * 50 + 50;
                                    from.z = zC;

                                    to.x = X * 50;
                                    to.y = Y * 50 + 50;
                                    to.z = zC + 10;

                                    bPlayer.SendConsoleCommand("ddraw.line", 1, "ff0000", from, to);
                                }

                                for (zC = 0; zC < 100; zC += 10)
                                {
                                    Vector3 from;
                                    Vector3 to;

                                    from.x = X * 50;
                                    from.y = Y * 50 + 50;
                                    from.z = zC;

                                    to.x = X * 50;
                                    to.y = Y * 50;
                                    to.z = zC + 10;

                                    bPlayer.SendConsoleCommand("ddraw.line", 1, "ff0000", from, to);
                                }
                            }
                        }
                    }
                }
            });
        }
        private void OnPlayerInit(BasePlayer bPlayer) => Player.TryLoad(bPlayer);

        private void OnPlayerSpawn(BasePlayer player) => OnPlayerInit(player);
        private void OnPlayerRespawn(BasePlayer player) => OnPlayerInit(player);
        private void OnPlayerSleepEnded(BasePlayer player) => OnPlayerInit(player);
        private void OnPlayerConnected(BasePlayer player) => OnPlayerInit(player);
        private void OnTick()
        {
            foreach (BasePlayer bPlayer in BasePlayer.activePlayerList)
            {

                Player data = Interface.Oxide.DataFileSystem.ReadObject<Player>($"Factions/Players/{bPlayer.userID}");

                data.chunkLast = data.chunkIn;
                data.chunkIn = Chunk.Entered(bPlayer);

                Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Players/{bPlayer.userID}"), data);

                if (data.chunkLast != data.chunkIn)
                {
                    PrintToChat(bPlayer, $"Entered: {data.chunkIn}");
                }
            }
        }

        // private object CanLock(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        // private object CanDeployItem(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        // private object OnPayForUpgrade(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        // private object OnPayForPlacement(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        // private object CanUseWires(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        // private object CanBuild(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        // private object CanChangeCode(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        // private object CanChangeGrade(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        // private object CanDemolish(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        // private object CanPickupEntity(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        // private object CanRenameBed(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        // private object CanPickupLock(BasePlayer bPlayer) => Fact.CanAct(bPlayer);
        // private object CanUnlock(BasePlayer bPlayer) => Fact.CanAct(bPlayer);

        private object OnPlayerAttack(BasePlayer bPlayer, HitInfo info) => Fact.CanAttack(bPlayer, info?.Initiator as BasePlayer);
        private object OnMeleeAttack(BasePlayer bPlayer, HitInfo info) => Fact.CanAttack(bPlayer, info?.Initiator as BasePlayer);
        private object OnPlayerAssist(BasePlayer target, BasePlayer attacker) => Fact.CanAttack(target, attacker);
        public class Player
        {
            public string userName = "";
            public string faction = "";
            public string chunkIn = "";
            public string chunkLast = "";
            public List<string> invites = new List<string>();
            internal static void TryLoad(BasePlayer bPlayer)
            {
                Player data = Interface.Oxide.DataFileSystem.ReadObject<Player>($"Factions/Players/{bPlayer.userID}");

                data.userName = bPlayer.displayName;

                Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Players/{bPlayer.userID}"), data);
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
            public List<string> invites = new List<string>();
            public ulong chieftain = new ulong();
            public string colour = "#00ff00";
            // #ff9900 #6699ff #ff00ff #ff9999 #339933 #ffcc99".Split(" ")[Math.Round(Random()*8 - 0.5)] || "#ff0000";

            internal static object CanAct(BasePlayer bPlayer)
            {
                string owner = Chunk.Entered(bPlayer);

                if (owner != "Wilderness")
                {
                    Player p = Interface.Oxide.DataFileSystem.ReadObject<Player>($"Factions/Players/{bPlayer.userID}");

                    if (p.userName == "")
                    {
                        return false;
                    }

                    Fact fac = Interface.Oxide.DataFileSystem.ReadObject<Fact>($"Factions/Factions/{p.faction}");

                    if (fac.name != "")
                    {
                        if (fac.name != p.faction)
                        {
                            return false;
                        }
                    }
                }

                return null;
            }

            internal static object CanAttack(BasePlayer victim, BasePlayer attacker)
            {
                string victimChunk = Chunk.Entered(victim);
                string attackerChunk = Chunk.Entered(victim);

                if (victimChunk == "Wilderness" && attackerChunk == "Wilderness")
                {
                    return null;
                }

                return false;
            }

            internal static string Info(BasePlayer bPlayer, string name)
            {
                Fact query = Interface.Oxide.DataFileSystem.ReadObject<Fact>($"Factions/Factions/{name}");

                if (query.name == "")
                {
                    return "Invalid Faction Name";
                }

                string playerList = "";

                foreach (ulong p in query.members)
                {
                    Player plr = Interface.Oxide.DataFileSystem.ReadObject<Player>($"Factions/Players/{p}");

                    if (plr.userName != "")
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
            public int X;
            public int Y;
            public string faction;
            internal static string Claim(BasePlayer bPlayer)
            {
                Player p = Interface.Oxide.DataFileSystem.ReadObject<Player>($"Factions/Players/{bPlayer.userID}");

                Fact claimer = Interface.Oxide.DataFileSystem.ReadObject<Fact>($"Factions/Factions/{p.faction}");

                if (claimer.name == "")
                {
                    return "You are not in a Faction!";
                }

                if (claimer.chieftain != bPlayer.userID)
                {
                    return "Only your Factions Chieftain may claim land";
                }

                Vector3 pos = bPlayer.transform.position;
                int X = (int)Math.Floor(pos.x / 50);
                int Y = (int)Math.Floor(pos.y / 50);
                Chunk data = Interface.Oxide.DataFileSystem.ReadObject<Chunk>($"Factions/Chunks/{X},{Y}");

                if (data.faction != null)
                {
                    if (data.faction != "")
                    {
                        Fact claimee = Interface.Oxide.DataFileSystem.ReadObject<Fact>($"Factions/Factions/{data.faction}");

                        if (claimee.name == "")
                        {
                            return "Something went wrong. Contact an Administrator! error x002";
                        }

                        if (claimer.name == claimee.name)
                        {
                            return "You already own this land!";
                        }

                        if (claimer.power > claimee.power)
                        {
                            data.faction = claimer.name;
                            claimer.chunks.Add($"{X},{Y}");
                            claimee.chunks.Remove($"{X},{Y}");

                            claimer.power -= 50;
                            claimee.power += 50;

                            Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Factions/{claimee.name}"), claimee);
                            Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Factions/{claimer.name}"), claimer);
                            Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Chunks/{X},{Y}"), data);
                            return "Land Claimed";
                        }
                    }
                }
                else if (claimer.power >= 50)
                {
                    data = new Chunk();
                    data.faction = claimer.name;
                    data.X = X;
                    data.Y = Y;

                    claimer.power -= 50;

                    Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Factions/{claimer.name}"), claimer);
                    Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Chunks/{X},{Y}"), data);
                    return "Land Claimed";
                }

                return "Need more power";
            }

            internal static string Unclaim(BasePlayer bPlayer)
            {
                Player p = Interface.Oxide.DataFileSystem.ReadObject<Player>($"Factions/Players/{bPlayer.userID}");

                if (p.userName == "")
                {
                    return null;
                }

                Fact unclaimer = Interface.Oxide.DataFileSystem.ReadObject<Fact>($"Factions/Factions/{p.faction}");

                if (unclaimer.name == "")
                {
                    return "You are not in a Faction!";
                }

                if (unclaimer.chieftain != bPlayer.userID)
                {
                    return "Only your Factions Chieftain may unclaim land";
                }

                Vector3 pos = bPlayer.transform.position;
                int X = (int)Math.Floor(pos.x / 50);
                int Y = (int)Math.Floor(pos.y / 50);
                Chunk data = Interface.Oxide.DataFileSystem.ReadObject<Chunk>($"Factions/Chunks/{X},{Y}");

                if (data.faction == "" || data.faction != unclaimer.name)
                {
                    return "You do not own this Chunk!";
                }

                data.faction = "";

                unclaimer.power += 50;

                Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Factions/{unclaimer.name}"), unclaimer);
                Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Chunks/{X},{Y}"), data);
                return "Land Unclaimed";
            }

            internal static string Entered(BasePlayer bPlayer)
            {
                Vector3 pos = bPlayer.transform.position;
                int X = (int)Math.Floor(pos.x / 50);
                int Y = (int)Math.Floor(pos.y / 50);
                Chunk data = Interface.Oxide.DataFileSystem.ReadObject<Chunk>($"Factions/Chunks/{X},{Y}");

                if (data.faction == null)
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
                if (args.Length == 1)
                {
                    PrintToChat(bPlayer, "Invalid Name");
                    return;
                }

                int c = 0;

                string name = "";

                foreach (string arg in args)
                {
                    if (c != 0)
                    {
                        if (c != 1)
                        {
                            name += "_";
                        }
                        name += arg;
                    }

                    c++;
                }

                Fact data = Interface.Oxide.DataFileSystem.ReadObject<Fact>($"Factions/Factions/{name}");

                if (data.name == "")
                {
                    data = new Fact();
                    data.name = name;
                    data.chieftain = bPlayer.userID;
                    data.members.Add(bPlayer.userID);

                    Player p = Interface.Oxide.DataFileSystem.ReadObject<Player>($"Factions/Players/{bPlayer.userID}");

                    if (p.userName == "")
                    {
                        return;
                    }

                    p.faction = name;

                    Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Factions/{name}"), data);
                    Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Players/{bPlayer.userID}"), p);
                    PrintToChat(bPlayer, $"Faction {name} Created");
                    PrintToChat(bPlayer, Chunk.Claim(bPlayer));
                }
                else
                {
                    PrintToChat(bPlayer, $"Faction Name {name} Already in use");
                }
            }

            if (args[0].ToLower() == "claim")
            {
                PrintToChat(bPlayer, Chunk.Claim(bPlayer));
            }

            if (args[0].ToLower() == "unclaim")
            {
                PrintToChat(bPlayer, Chunk.Unclaim(bPlayer));
            }

            if (args[0].ToLower() == "info")
            {
                PrintToChat(bPlayer, Fact.Info(bPlayer, command.ToLower().Replace("!f ", "").Replace("!factions ", "").Replace("info ", "").Replace(" ", "_").ToLower()));
            }
        }
    }
}