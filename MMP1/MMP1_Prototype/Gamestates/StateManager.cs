using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMP1_Prototype
{   
    public enum Gamestate
    {
        Ingame,
        Menu,
        EnterName,
        IPSelection,
        NetworkLobby,
        MatchRequest,
        WaitForMatch,
        NoChange,
        Reload,
        Victory,
        Defeat
    }
    /// <summary>
    /// Kümmert sich um erstellen, zeichen, updaten und ändern des momentanen Gamestates
    /// </summary>
    public class StateManager
    {
        I_State currentState;
        Game1 game;
        public StateManager(Gamestate InitialState, Game1 game)
        {
            this.game = game;
            currentState = GetState(InitialState);
        }
        //Updatet gamestate falls es keine änderungen gibt
        
        public void Draw(SpriteBatch spriteBatch)
        {
            currentState.Draw(spriteBatch);
        }
        public void Update(GameTime gameTime)
        {
            //Updated momentanen State und ändert ihn falls nötig
            UpdateState(currentState.Update(gameTime));
        }

        public void UpdateState(Gamestate gamestate)
        {
            if (gamestate != Gamestate.NoChange)
            {
                currentState = GetState(gamestate);
            }
        }

        /// <summary>
        /// Erzeugt den angegebenen GameState
        /// </summary>
        /// <param name="state">GeamstateTyp</param>       
        /// <returns>Das erzeugte object</returns>
        private I_State GetState(Gamestate state)
        {

            I_State result;
            switch (state)
            {
                case Gamestate.Ingame:
                    result = new IngameState(game);
                    break;
                case Gamestate.Menu:
                    result = new MenuState(game);
                    break;
                case Gamestate.EnterName:
                    result = new OptionsState(game);
                    break;
                case Gamestate.IPSelection:
                    result = new IPSelectionState(game);
                    break;
                case Gamestate.NetworkLobby:
                    result = new NetworkLobbyState(game);
                    break;
                case Gamestate.MatchRequest:
                    result = new MatchRequestState(game);
                    break;
                case Gamestate.WaitForMatch:
                    result = new WaitingForMatchState(game);
                    break;
                case Gamestate.Victory:
                    result = new VictoryState(game);
                    break;
                case Gamestate.Defeat:
                    result = new DefeatState(game);
                    break;
                default:
                    result = null;
                    break;
            }
            Debug.Assert(result != null);
            return result;
        }
    }
}
