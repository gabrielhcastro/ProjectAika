using AikaEmu.GameServer.Models.Units;
using AikaEmu.GameServer.Network.GameServer;
using AikaEmu.Shared.Network;

namespace AikaEmu.GameServer.Network.Packets.Game
{
    public class UpdateWithSkillEffect : GamePacket
    {
        private readonly ushort _conId;
        private readonly ushort _targetId;
        private readonly ushort _unkType;
        private readonly ushort _skillId;
        private readonly int _damage;
        private readonly int _targetHp;
        private readonly Position _pos;

        public UpdateWithSkillEffect(ushort conId, ushort targetId, ushort unkType, ushort skillId, int damage, int targetHp, Position pos)
        {
            _conId = conId;
            _targetId = targetId;
            _unkType = unkType;
            _skillId = skillId;
            _damage = damage;
            _targetHp = targetHp;
            _pos = pos;

            Opcode = (ushort)GameOpcode.UpdateWithSkillEffect;
            SenderId = conId;
        }

        public override PacketStream Write(PacketStream stream)
        {
            byte a = 26;
            byte c = 1;
            var e = 0;
            var f = 0;
            stream.Write((uint)_skillId);
            stream.Write(_pos.CoordX);
            stream.Write(_pos.CoordY);
            stream.Write(0);
            stream.Write(_conId);
            stream.Write((byte)0);
            stream.Write(_unkType);
            stream.Write((byte)0);
            stream.Write((short)0);
            stream.Write(0);
            stream.Write(0);
            stream.Write(_targetHp); // HP atual do alvo para o cliente sincronizar
            stream.Write(0);
            stream.Write(0);
            stream.Write(_targetId);
            stream.Write((byte)c);
            stream.Write((byte)a);
            stream.Write(_damage);   // Dano que aparecerá flutuando
            stream.Write(e);
            stream.Write(f);
            stream.Write(_targetHp); // HP atual repetido
            stream.Write(0);
            stream.Write(0);
            return stream;
        }
    }
}