using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMP1_Prototype
{
    interface I_UnitPool
    {
        /// <summary>
        /// Zeigt alle ausgewählten einheiten oben links am Bildschirm an
        /// </summary>
        /// <param name="spriteBatch">Spritebatch zum Zeichen</param>
        /// <param name="offset">Positionsoffset</param>
        /// <returns></returns>
        int Display(SpriteBatch spriteBatch, int offset = 0);
        void Update();
        void Draw(SpriteBatch spritebatch, Point offset);
        int WriteBytes(byte[] result, int offset);
        int SetFromBytes(byte[] data, int offset, int taktruckstand = 0);
        int GetBoxMatchesNumber(BasicHitBox other);
        void selectedUnitsMoveTo(Point position);
        void selectedUnitsShootAt(Point position, ProjectileManager projectileFactory);
        int selectNumberPressedfromSelected(int pressedNumber, int queueOffset = 0);
        void selectWithinBox(Rectangle box);
        /// <summary>
        /// Überprüft die Kollisions mit Projektilen und schreibt die resultierenden Daten in ein Packet
        /// </summary>
        /// <param name="projectileManager">Projetilmanager des Spielers der Projektile</param>
        /// <param name="result">Das zu beschreibende Array</param>
        /// <param name="offset">Offset bei dem zu schreiben begonnen werden soll</param>
        /// <returns>Neuer Offset</returns>
        int Hitdetection(ProjectileManager projectileManager, byte[] result, int offset = 0);
        /// <summary>
        /// Liest Daten aus dem Packet aus und pass das Leben der Einheiten entsprechend an
        /// </summary>
        /// <param name="data">Daten</param>
        /// <param name="offset">Offset bei dem zu Lesen begonnen werden soll</param>
        /// <returns>neuer offset</returns>
        int AdjustUnitHealthToData(byte[] data, int offset);
        int getLivingUnits();
        int getDieingUnits();
    }
}
