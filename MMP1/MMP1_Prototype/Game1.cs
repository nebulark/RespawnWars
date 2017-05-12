#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using System.Diagnostics;
using System.Net;




#endregion

namespace MMP1_Prototype
{
    
   
    public class Game1 : Game
    {
        public GraphicsDeviceManager graphics;
        public UdpVerbindung Udp; 
        public SpriteFont font1;    
        private SpriteBatch spriteBatch;              
        private UI ui;
        private StateManager stateManager;
           

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            
            graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";
            Utilities.LocalAdress = IPAddress.Any;
           
        }
       
        
       

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            ui = new UI();
            Udp = new UdpVerbindung(IPAddress.Broadcast);
            Udp.BeginReceive();
           
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font1 = Content.Load<SpriteFont>("fonts\\gameFont");
            ui.LoadContent(font1, new Vector2(100, 100), Content.Load<Texture2D>("maus"));
            stateManager = new StateManager(Gamestate.Menu, this);          
        }       
        protected override void UnloadContent()
        {            
        }

        protected override void Update(GameTime gameTime)
        {
           
            Utilities.UpdateInput();
            stateManager.Update(gameTime);          
            ui.Update(gameTime);
            base.Update(gameTime);
        }

        //Wird für jedes Frame aufgerufen
        //Reihenfolge des Zeichnens ist wichtig da immer überzeichent wird
        //Am anfang wird immer das gesammte Bild "gelöscht"
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            stateManager.Draw(spriteBatch);
            ui.Draw(spriteBatch);            
            spriteBatch.End();            
        }
    }
}
