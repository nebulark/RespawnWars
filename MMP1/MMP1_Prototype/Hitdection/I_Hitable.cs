using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMP1_Prototype
{
    public interface I_Hitable
    {
        /// <summary>
        /// Muss von Jedem Object mit Hitbox implementiert werden
        /// </summary>
        /// <returns>Die eigene Hitbox</returns>
        BasicHitBox GetHitdetectionData();        
    }
}
