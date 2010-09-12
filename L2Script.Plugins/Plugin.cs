using L2Script.Library;
using L2Script.Packets.Game;
using L2Script.Packets;

namespace L2Script.Plugins
{
    public interface Plugin
    {
        PluginInfo GetInfo();
        bool HandlePacket(PacketList.Server opcode, GameReader packet, GameData gameData, bool Handled, ExtensionHandler extensions);
        Extension[] GetExtensions();
        void RegisterEvents(object events);
    }
}
