using AikaEmu.GameServer.Managers;
using AikaEmu.GameServer.Models.Units;
using AikaEmu.GameServer.Models.Units.Character;
using AikaEmu.GameServer.Network.GameServer;
using AikaEmu.GameServer.Network.Packets.Game;
using AikaEmu.Shared.Network;
using NLog;
using AikaEmu.GameServer.Services; // Importar o CombatService

namespace AikaEmu.GameServer.Network.Packets.Client
{
    public class RequestUseSkill : GamePacket
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        protected override void Read(PacketStream stream)
        {
            var targetConId = stream.ReadUInt16();
            stream.ReadInt16();
            stream.ReadUInt32(); // unk
            stream.ReadUInt32(); // unk
            stream.ReadUInt32(); // unk
            var unkType = stream.ReadUInt16();
            var skillId = stream.ReadUInt16();
            var coordX = stream.ReadSingle();
            var coordY = stream.ReadSingle();
            var tCoordX = stream.ReadSingle();
            var tCoordY = stream.ReadSingle();

            Log.Debug("RequestUseSkill: skillId: {0}, targetId: {1}, targetCoord: {2}/{3}", skillId, targetConId, tCoordX, tCoordY);

            var playerCharacter = Connection.ActiveCharacter; // O jogador que usou a skill
            if (playerCharacter == null) return; // Garante que o atacante existe

            var targetUnit = WorldManager.Instance.GetUnit(targetConId);
            if (targetUnit != null)
            {
                (int calculatedDamage, DamageType damageType) = CombatService.CalculateDamage(playerCharacter, targetUnit, skillId);

                if (damageType != DamageType.Miss && damageType != DamageType.Immune && damageType != DamageType.Block)
                {
                    Log.Debug("Target Unit HP before damage: {0}", targetUnit.Hp);
                    targetUnit.Hp -= calculatedDamage;
                    if (targetUnit.Hp < 0) targetUnit.Hp = 0;

                    Log.Debug("Target {0} (ID: {1}) took {2} damage ({4}). New HP: {3}", targetUnit.Name, targetUnit.Id, calculatedDamage, targetUnit.Hp, damageType);

                    //Connection.SendPacket(new UpdateHpMp(targetUnit));
                    Connection.SendPacket(new UpdateWithSkillEffect(Connection.Id, targetConId, unkType, skillId, calculatedDamage, targetUnit.Hp, Connection.ActiveCharacter.Position));

                    if (targetUnit.Hp == 0)
                    {
                        ulong expGain = 100;
                        playerCharacter.Experience += expGain;

                        playerCharacter.Connection.SendPacket(new UpdateExperience(playerCharacter));
                        playerCharacter.Connection.SendPacket(new SendXpGoldAnimation(playerCharacter.Id, (uint)expGain, 0));

                        // TODO: Lógica para despawn do mob, loot, etc.
                    }
                }
                else // Se o ataque foi Miss, Immune ou Block
                {
                    Log.Debug("Attack on Target {0} (ID: {1}) resulted in {2}", targetUnit.Name, targetUnit.Id, damageType);
                    Connection.SendPacket(new UpdateWithSkillEffect(Connection.Id, targetConId, unkType, skillId, 0, targetUnit.Hp, Connection.ActiveCharacter.Position));
                }
            }
        }
    }
}
