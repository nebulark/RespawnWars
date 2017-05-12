using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMP1_Prototype
{
    interface I_ProjectilePool
    {
        void Update();
        void Draw(SpriteBatch spritebatch, Point offset);
        /// <summary>
        /// Schreibt die eignen Werte und die der Poolelemte ins Bytearray
        /// </summary>
        /// <param name="result">Das zu beschreibende Array</param>
        /// <param name="offset">Offset bei dem zu schreiben begonnen werden soll</param>
        /// <returns>Neuer offset</returns>
        int WriteBytes(byte[] result, int offset);
        /// <summary>
        /// Setzt die eignen Werte und die der Poolelemte abhängib vom Bytearry
        /// </summary>
        /// <param name="data">Packet</param>
        /// <param name="offset">Offset bei dem zu lesen begonnen werden soll</param>
        /// <returns>Neuer offset</returns>
        int SetFromBytes(byte[] data, int offset);
        /// <summary>
        /// Macht eine Kollisiondetection mit der angebenen Hitbox, löscht gegebenfalls Projectile und gibt den Gesammtschaden zurück 
        /// </summary>
        /// <param name="HitBox">Hitbox einer Einheit</param>
        /// <returns>Schaden den die Einheit erleidet</returns>
        int Hitdetection(BasicHitBox HitBox);
    }
}
