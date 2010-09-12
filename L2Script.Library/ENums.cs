using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Script.Library
{
    public enum ChatType : byte
    {
        All = 0x00, // This is actually 0x00
        Shout = 0x01,
        Private = 0x02,
        Party = 0x00,
        Clan = 0x04,
        GM = 0x00,
        Petition = 0x00,
        PetitionReply = 0x00,
        Trade = 0x08,
        Ally = 0x00,
        Announcement = 0x0A,
        Boat = 0x00,
        PartyRoom = 0x00,
        PartyCommander = 0x00,
        Hero = 0x00,
    }
}
