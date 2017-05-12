using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;


namespace MMP1_Prototype
{
    interface I_State
    {
        
        
        /// <summary>
        /// Updatet den Gamestate und gibt zurück falls der State geändert werden muss
        /// </summary>
        /// <param name="gameTime">Zeit aus Game1</param>
        /// <returns>Der nächste Gamestate oder Gamestate.NoChange</returns>
        Gamestate Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
        

    }
}
