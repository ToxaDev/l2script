using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L2Script.Packets.Game;
using L2Script.Library;

namespace L2Script.Plugins.R4Toolkit.InformationExtension.Packets
{
    public static class NPCInfo
    {
        public static Character parsePacket(GameReader packet)
        {
            Character toon = new Character();
            toon.ObjectID = packet.readD();
            toon.NpcTypeId = packet.readD();
            toon.isAttackable = packet.readD();
            toon.X = packet.readD();
            toon.Y = packet.readD();
            toon.Z = packet.readD();
            toon.Heading = packet.readD();
            packet.readD();
            toon.MatkSpd = packet.readD();
            toon.PatkSpd = packet.readD();
            toon.RunSpd = packet.readD();
            toon.WalkSpd = packet.readD();
            toon.SwimRunSpd = packet.readD();
            toon.SwimWalkSpd = packet.readD();
            toon.RunSpd = packet.readD();
            toon.WalkSpd = packet.readD();
            toon.FlyRunSpd = packet.readD();
            toon.FlyWalkSpd = packet.readD();
            toon.MoveMul = packet.readF();
            toon.AtkSpeedMul = packet.readF();
            toon.ColRadius = packet.readF();
            toon.ColHeight = packet.readF();
            toon.IDRhand = packet.readD();
            packet.readD();
            toon.IDLhand = packet.readD();
            packet.readC();
            toon.isRunning = packet.readC();
            toon.isInCombat = packet.readC();
            toon.isAlikeDead = packet.readC();
            toon.Invisible = packet.readC();
            toon.Name = packet.readS();
            toon.Title = packet.readS();
            packet.readD();
            packet.readD();
            packet.readD();
            toon.AbnormalEffect = packet.readD();
            toon.ClanID = packet.readD();
            toon.ClanCrestID = packet.readD();
            packet.readD();
            packet.readD();
            packet.readC();
            toon.Team = packet.readC();
            toon.ColRadius = packet.readF();
            toon.ColHeight = packet.readF();
            return toon;
        }
    }
}