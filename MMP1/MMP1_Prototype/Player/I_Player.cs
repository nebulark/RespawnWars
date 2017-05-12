using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MMP1_Prototype
{
    interface I_Player
    {
        void Initialize(byte nr);
        void Update(GameTime gameTime, Point offset);
        void Draw(SpriteBatch spritebatch, Point offset);
    }
}
