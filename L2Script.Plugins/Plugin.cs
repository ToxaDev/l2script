using L2Script.Library;
using L2Script.Packets.Game;
using L2Script.Packets;
namespace L2Script.Plugins
{
    public interface Plugin
    {
        bool HandlePacket(PacketList.Server opcode, GameReader packet, GameData gameData, bool Handled);
        bool HandlePacket(PacketList.Client opcode, GameReader packet, GameData gameData, bool Handled);
    }
}
