using AikaEmu.GameServer.Models.Units;
using AikaEmu.GameServer.Network.GameServer;
using AikaEmu.Shared.Network;

namespace AikaEmu.GameServer.Network.Packets.Game
{
    public class UpdateHpMp : GamePacket
    {
        private readonly BaseUnit _unit;

        public UpdateHpMp(BaseUnit unit)
        {
            _unit = unit;

            Opcode = (ushort)GameOpcode.UpdateHpMp;
            SetSenderIdWithUnit(_unit);
        }

        public override PacketStream Write(PacketStream stream)
        {
            stream.Write(_unit.MaxHp); // TODO MAX
            stream.Write(_unit.Hp);
            stream.Write(_unit.MaxMp); // TODO MAX
            stream.Write(_unit.Mp);
            stream.Write(0);
            return stream;
        }
    }
}