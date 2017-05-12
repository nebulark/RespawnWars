using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMP1_Prototype
{
    enum UnitType : byte
    {
        RocketMan,
        Sniper,
        Burst
    }
    class UnitManager
    {
        public const int UnitTypeCount = 3;
        public const int MaxUnitCount = 4;
        //Sobal erreicht ist verliert man
        private const int defeatLimit = 20;
        //zählt für jedes besiegte unit eins hoch
        private int defeated; 
        Texture2D[] textures;
        I_UnitPool[] storage;
        
        public UnitManager(Texture2D[] textures, Color color)
        {
            this.textures = textures;
            storage = new I_UnitPool[UnitTypeCount];
            storage[(int)UnitType.RocketMan] = new UnitPool<RocketUnit>(textures[0], color);
            storage[(int)UnitType.Sniper] = new UnitPool<SniperUnit>(textures[1],color);
            storage[(int)UnitType.Burst] = new UnitPool<BurstUnit>(textures[2],color);
            

            defeated = 0;
        }
        //Produziere Unit wenn möglich, gib erfolg zurück
        public bool CreateUnit(Point pos, UnitType type)
        {
            if (GetLivingUnits() >= MaxUnitCount) { return false; }            
            switch (type)
            {
                case UnitType.RocketMan:
                    ((UnitPool<RocketUnit>)storage[(int)UnitType.RocketMan]).CreateUnit(pos);
                    break;
                case UnitType.Sniper:
                    ((UnitPool<SniperUnit>)storage[(int)UnitType.Sniper]).CreateUnit(pos);
                    break;
                case UnitType.Burst:
                    ((UnitPool<BurstUnit>)storage[(int)UnitType.Burst]).CreateUnit(pos);
                    break;
            }
            return true;
        }
        public void Update()
        {
            //muss vor unitupdate ausgeführt werden, da update als einziges units löschen kann, aber mehrere eine Death Flag (Alive = 1) setzen könnten
            defeated += getDieingUnits();
            for (int i = 0; i < storage.Length; i++)
            {
                storage[i].Update();
            }
        }
        public void Draw(SpriteBatch spriteBatch, Point offset)
        {
            for (int i = 0; i < storage.Length; i++)
            {
                storage[i].Draw(spriteBatch, offset);
            }
        }
        //Return gibt den neuen offset an
        public int WriteBytes(byte[] result, int offset)
        {
            for (int i = 0; i < storage.Length; i++)
            {
                offset = storage[i].WriteBytes(result, offset);
            }
            return offset;
        }
        public int SetFromBytes(byte[] data, int offset, int taktruckstand = 0)
        {
            for (int i = 0; i < storage.Length; i++)
            {
                offset = storage[i].SetFromBytes(data, offset, taktruckstand);
            }
            return offset;
        }
        public void Display(SpriteBatch spriteBatch)
        {
            int offset = 0;
            for (int i = 0; i < storage.Length; i++)
            {
                offset = storage[i].Display(spriteBatch, offset);
            }
        }
        public int GetBoxMatchesNumber(BasicHitBox hitbox)
        {
            int result = 0;
            for (int i = 0; i < storage.Length; i++)
            {
                result += storage[i].GetBoxMatchesNumber(hitbox);
            }
            return result;
        }
        public void selectedUnitsMoveTo(Point position) {
            for (int i = 0; i < storage.Length; i++)
            {
                storage[i].selectedUnitsMoveTo(position);
            }
        }
        public void selectedUnitsShootAt(Point positionCenter, ProjectileManager projectileFactory)
        {
            for (int i = 0; i < storage.Length; i++)
            {
                storage[i].selectedUnitsShootAt(positionCenter, projectileFactory);
            }
        }
        public void selectNumberPressedfromSelected(int pressedNumber)
        {
            int offset = 0;
            for (int i = 0; i < storage.Length; i++)
            {
                offset = storage[i].selectNumberPressedfromSelected(pressedNumber, offset);
            }
        }
        /// <summary>
        /// Selektiert alle Einheiten die das Rechteckt berühren
        /// </summary>
        /// <param name="box">Rechteck</param>
        public void selectWithinBox(Rectangle box)
        {
            for (int i = 0; i < storage.Length; i++)
            {
                storage[i].selectWithinBox(box);
            }
        }

        /// <summary>
        /// Überprüft die Kollisions mit Projektilen und schreibt die resultierenden Daten in ein Packet
        /// </summary>
        /// <param name="projectileManager">Projetilmanager des Spielers der Projektile</param>
        /// <param name="result">Das zu beschreibende Array</param>
        /// <param name="offset">Offset bei dem zu schreiben begonnen werden soll</param>
        /// <returns>Neuer Offset</returns>
        public int Hitdetection(ProjectileManager projectileManager, byte[] result, int offset = 0)
        {
            //Gleichzeitig kann upgedatet werden wieviele units gestorben sind
            int livingunits = GetLivingUnits();           
            result[0] = (byte)PacketType.Damage;            
            offset++;
            
            for (byte i = 0; i < storage.Length; i++)
            {               
                offset = storage[i].Hitdetection(projectileManager, result, offset);                
            }
            return offset;

        }
        /// <summary>
        /// Liest Daten aus dem Packet aus und pass das Leben der Einheiten entsprechend an
        /// </summary>
        /// <param name="data">Daten</param>
        /// <param name="offset">Offset bei dem zu Lesen begonnen werden soll</param>
        /// <returns>neuer offset</returns>
        public void AdjustUnitHealthToData(byte[] data)
        {            
            int offset = 1;            
            for (byte i = 0; i < storage.Length; i++)
            {                
                offset = storage[i].AdjustUnitHealthToData(data, offset);
            }
        }
        public int GetLivingUnits()
        {
            int result = 0;
            for (int i = 0; i < storage.Length; i++)
            {
                result += storage[i].getLivingUnits();
            }
            return result;
        }
        public int getDieingUnits()
        {
            int result = 0;
            for (int i = 0; i < storage.Length; i++)
            {
                result += storage[i].getDieingUnits();
            }
            return result;
        }
        public bool DefeatLimitReached()
        {
            return defeated >= defeatLimit;
        }
       
    }
}
