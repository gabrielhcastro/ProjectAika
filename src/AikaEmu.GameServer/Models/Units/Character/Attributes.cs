using System.Collections.Generic;
using AikaEmu.Shared.Model.Network;
using AikaEmu.Shared.Network;

namespace AikaEmu.GameServer.Models.Units.Character
{
    public class Attributes : BasePacket
    {
        private readonly ushort _baseStr;
        private readonly ushort _baseAgi;
        private readonly ushort _baseInt;
        private readonly ushort _baseCon;
        private readonly ushort _baseSpi;

        public ushort Strenght
        {
            get => _baseStr;
        }

        public ushort Agility
        {
            get => _baseAgi;
        }

        public ushort Intelligence
        {
            get => _baseInt;
        }

        public ushort Constitution
        {
            get => _baseCon;
        }

        public ushort Spirit
        {
            get => _baseSpi;
        }

        // Novos atributos de combate
        public int AttackFis { get; set; } // Dano Físico
        public int AttackMag { get; set; } // Dano Mágico
        public int Defense { get; set; } // Defesa Física
        public int MagicDefense { get; set; } // Defesa Mágica
        public int FisPenetration { get; set; } // Penetração Física
        public int MagPenetration { get; set; } // Penetração Mágica
        public int Critical { get; set; } // Chance de Crítico
        public int CritRes { get; set; } // Resistência a Crítico
        public int Accuracy { get; set; } // Acerto
        public int Evasion { get; set; } // Esquiva

        public Attributes(IReadOnlyList<ushort> attr)
        {
            _baseStr = attr[0];
            _baseAgi = attr[1];
            _baseInt = attr[2];
            _baseCon = attr[3];
            _baseSpi = attr[4];

            // Inicialização básica dos novos atributos (pode ser expandida com cálculos)
            AttackFis = _baseStr * 2; // Exemplo
            AttackMag = _baseInt * 2; // Exemplo
            Defense = _baseCon * 1; // Exemplo
            MagicDefense = _baseSpi * 1; // Exemplo
            FisPenetration = 0; // Placeholder
            MagPenetration = 0; // Placeholder
            Critical = _baseAgi / 2; // Exemplo
            CritRes = _baseCon / 2; // Exemplo
            Accuracy = _baseAgi; // Exemplo
            Evasion = _baseAgi; // Exemplo
        }

        public Attributes(ushort str, ushort agi, ushort inte, ushort con, ushort spi)
        {
            _baseStr = str;
            _baseAgi = agi;
            _baseInt = inte;
            _baseCon = con;
            _baseSpi = spi;

            // Inicialização básica dos novos atributos (pode ser expandida com cálculos)
            AttackFis = _baseStr * 2; // Exemplo
            AttackMag = _baseInt * 2; // Exemplo
            Defense = _baseCon * 1; // Exemplo
            MagicDefense = _baseSpi * 1; // Exemplo
            FisPenetration = 0; // Placeholder
            MagPenetration = 0; // Placeholder
            Critical = _baseAgi / 2; // Exemplo
            CritRes = _baseCon / 2; // Exemplo
            Accuracy = _baseAgi; // Exemplo
            Evasion = _baseAgi; // Exemplo
        }

        public override PacketStream Write(PacketStream stream)
        {
            stream.Write(Strenght);
            stream.Write(Agility);
            stream.Write(Intelligence);
            stream.Write(Constitution);
            stream.Write(Spirit);
            // TODO: Adicionar escrita dos novos atributos se forem enviados ao cliente
            return stream;
        }
    }
}