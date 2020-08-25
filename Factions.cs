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
        private List<MapMarkerGenericRadius> chunkMarkers = new List<MapMarkerGenericRadius>();
        private void OnServerInitialized()
        {
            ins = this;
            //permission.RegisterPermission ("ELORanks.Admin", this);
            foreach (BasePlayer bPlayer in BasePlayer.activePlayerList) OnPlayerInit(bPlayer);

            timer.Every(1f, () =>
            {
                foreach (BasePlayer bPlayer in BasePlayer.activePlayerList)
                {
                    int xC = -5;
                    int zC = -5;
                    for (xC = -5; xC <= 5; xC++)
                    {
                        for (zC = -5; zC <= 5; zC++)
                        {
                            Vector3 pos = bPlayer.transform.position;

                            int X = (int)Math.Floor(pos.x / 50) + xC;
                            int Z = (int)Math.Floor(pos.y / 50) + zC;

                            Chunk data = Interface.Oxide.DataFileSystem.ReadObject<Chunk>($"Factions/Chunks/{X},{Z}");

                            if (data.faction != null && data.faction != "")
                            {
                                float h = 0;
                                for (h = 0; h < 100; h += 5)
                                {
                                    square(bPlayer, X * 50, Z * 50, h, 50);
                                    square(bPlayer, X * 50, Z * 50, h + 0.1f, 50);
                                    square(bPlayer, X * 50, Z * 50, h + 0.2f, 50);
                                }
                            }
                        }
                    }
                }
            });

            // int a = -100;
            // int b = -100;
            // while (a < 100)
            // {
            //     Chunk data = Interface.Oxide.DataFileSystem.ReadObject<Chunk>($"Factions/Chunks/{a},{b}");

            //     if (data.faction != null && data.faction != "")
            //     {
            //         Vector3 center;
            //         center.x = (a*50)+25;
            //         center.z = (b*50)+25;
            //         center.y = 100;
            //         createChunkMarker(center);
            //     }

            //     b++;
            //     if (b == 100)
            //     {
            //         b = -100;
            //         a++;
            //     }
            // }
        }

        void square(BasePlayer bPlayer, float x, float z, float h, float width)
        {
            Vector3 tl;
            Vector3 tr;
            Vector3 bl;
            Vector3 br;

            tl.x = x;
            tl.z = z;
            tl.y = h;

            tr.x = x + width;
            tr.z = z;
            tr.y = h + 10;

            bl.x = x + width;
            bl.z = z + width;
            bl.y = h;

            br.x = x;
            br.z = z + width;
            br.y = h + 10;

            bPlayer.SendConsoleCommand("ddraw.line", 1, "", tl, tr);
            bPlayer.SendConsoleCommand("ddraw.line", 1, "", tr, bl);
            bPlayer.SendConsoleCommand("ddraw.line", 1, "", bl, br);
            bPlayer.SendConsoleCommand("ddraw.line", 1, "", br, tl);
        }
        // private void KillMapMarker(MapMarkerGenericRadius mapMarker)
        // {
        //     if (chunkMarkers.Contains(mapMarker))
        //         chunkMarkers.Remove(mapMarker);

        //     if (chunkMarkers.Contains(mapMarker))
        //         chunkMarkers.Remove(mapMarker);

        //     mapMarker.Kill();
        //     mapMarker.SendUpdate();
        // }

        // private void createChunkMarker(Vector3 pos)
        // {
        //     var mapMarker = GameManager.server.CreateEntity("assets/prefabs/tools/map/genericradiusmarker.prefab", pos) as MapMarkerGenericRadius;
        //     mapMarker.alpha = 0.2f;
        //     mapMarker.color1 = Color.yellow;
        //     mapMarker.radius = 25f;

        //     mapMarker.Spawn();
        //     mapMarker.SendUpdate();

        //     chunkMarkers.Add(mapMarker);
        // }
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
            public List<ulong> invites = new List<ulong>();
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

            internal static string info(BasePlayer bPlayer, string name)
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

            internal static String invite(BasePlayer bPlayer, ulong id)
            {
                Player recruit = Interface.Oxide.DataFileSystem.ReadObject<Player>($"Factions/Players/{id}");

                if (recruit.userName == "")
                {
                    return "Invalid Name";
                }

                Player recruiter = Interface.Oxide.DataFileSystem.ReadObject<Player>($"Factions/Players/{bPlayer.userID}");

                if (recruiter.userName == "")
                {
                    return "Error x001";
                }

                Fact faction = Interface.Oxide.DataFileSystem.ReadObject<Fact>($"Factions/Factions/{recruiter.faction}");

                if (faction.name == "")
                {
                    return "You are not in a Faction!";
                }

                if (faction.chieftain != bPlayer.userID)
                {
                    return "Only your Factions Chieftain may invite members";
                }

                recruit.invites.Add(faction.name);
                faction.invites.Add(id);

                Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Players/{id}"), recruit);
                Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Factions/{recruiter.faction}"), faction);

                return "Invite sent!";
            }

            internal static String leave(BasePlayer bPlayer)
            {
                Player leaver = Interface.Oxide.DataFileSystem.ReadObject<Player>($"Factions/Players/{bPlayer.userID}");

                if (leaver.userName == "")
                {
                    return "Error x001";
                }

                Fact faction = Interface.Oxide.DataFileSystem.ReadObject<Fact>($"Factions/Factions/{leaver.faction}");

                if (faction.name == "")
                {
                    return "You are not in a Faction!";
                }

                if (faction.chieftain == bPlayer.userID)
                {
                    return "A Faction's Chieftain cannot leave the Faction. They must disband it.";
                }

                leaver.faction = "";
                faction.members.Remove(bPlayer.userID);

                Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Players/{bPlayer.userID}"), leaver);
                Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Factions/{leaver.faction}"), faction);

                return "Left Faction!";
            }
        }

        public class Chunk
        {
            public int X;
            public int Z;
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
                int Z = (int)Math.Floor(pos.z / 50);
                Chunk data = Interface.Oxide.DataFileSystem.ReadObject<Chunk>($"Factions/Chunks/{X},{Z}");

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
                            claimer.chunks.Add($"{X},{Z}");
                            claimee.chunks.Remove($"{X},{Z}");

                            claimer.power -= 50;
                            claimee.power += 50;

                            Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Factions/{claimee.name}"), claimee);
                            Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Factions/{claimer.name}"), claimer);
                            Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Chunks/{X},{Z}"), data);
                            return "Land Claimed";
                        }
                    }
                }
                else if (claimer.power >= 50)
                {
                    data = new Chunk();
                    data.faction = claimer.name;
                    data.X = X;
                    data.Z = Z;

                    claimer.power -= 50;

                    Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Factions/{claimer.name}"), claimer);
                    Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Chunks/{X},{Z}"), data);
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
                int Z = (int)Math.Floor(pos.z / 50);
                Chunk data = Interface.Oxide.DataFileSystem.ReadObject<Chunk>($"Factions/Chunks/{X},{Z}");

                if (data.faction == "" || data.faction != unclaimer.name)
                {
                    return "You do not own this Chunk!";
                }

                data.faction = "";

                unclaimer.power += 50;

                Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Factions/{unclaimer.name}"), unclaimer);
                Interface.Oxide.DataFileSystem.WriteObject(($"Factions/Chunks/{X},{Z}"), data);
                return "Land Unclaimed";
            }

            internal static string Entered(BasePlayer bPlayer)
            {
                Vector3 pos = bPlayer.transform.position;
                int X = (int)Math.Floor(pos.x / 50);
                int Z = (int)Math.Floor(pos.z / 50);
                Chunk data = Interface.Oxide.DataFileSystem.ReadObject<Chunk>($"Factions/Chunks/{X},{Z}");

                if (data.faction == null || data.faction == "")
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

                    if (p.faction != "")
                    {
                        PrintToChat(bPlayer, "You are already in a Faction");
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

            if (args[0].ToLower() == "invite")
            {
                PrintToChat(bPlayer, Fact.invite(bPlayer, (ulong)Convert.ToUInt64(args[1])));
            }

            if (args[0].ToLower() == "info")
            {
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

                PrintToChat(bPlayer, Fact.info(bPlayer, name));
            }

            if (args[0].ToLower() == "help")
            {
                PrintToChat(bPlayer, "<color='#ff0000'>Factions Commands:</color><br>/f create [Name]<br>/f info [Name]<br>/f invite [Steam ID]<br>/f join [Name]<br>/f kick [Name]<br>/f claim<br>/f unclaim<br>/f leave<br>/f disband<br>/f ally [Name]<br>/f enemy [Name]");
            }

            if (args[0].ToLower() == "leave")
            {
                PrintToChat(bPlayer, Fact.leave(bPlayer));
            }
        }
    }
}