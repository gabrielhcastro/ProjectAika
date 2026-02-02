using AikaEmu.GameServer.Helpers;
using AikaEmu.GameServer.Network.GameServer;
using AikaEmu.Shared.Network;

namespace AikaEmu.GameServer.Network.Packets.Client
{
    public class InviteToParty : GamePacket
    {
        protected override void Read(PacketStream stream)
        {
            var conId = stream.ReadUInt16();
            // stream.ReadInt16();
            // stream.ReadBytes(16);
            GroupHelper.PartyRequest(Connection, conId);
        }
    }
}