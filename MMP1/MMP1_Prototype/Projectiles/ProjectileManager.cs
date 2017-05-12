using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMP1_Prototype
{
    public enum ProjectileType{
        Rocket,
        PseudoSniper,
        Burst
    }
    enum ProjectileTexture
    {
        Rocket,
        Explosion,
        Sniper,
        Burst
    }
    /// <summary>   
    /// Wählt immer den Passenden objectpool und gibt alle aufgaben an diesen weiter    
    /// </summary>
    public class ProjectileManager
    {
        const int projectileTypeCount = 3;
        I_ProjectilePool [] pool;
        Texture2D[] textures;
        public ProjectileManager(Texture2D[] textures, Color color)
        {
            this.textures = textures;
            pool = new I_ProjectilePool[projectileTypeCount];
            pool[(int)ProjectileType.Rocket] = new ProjectilePool<RocketProjectile>(color,textures[(int)ProjectileTexture.Rocket],textures[(int)ProjectileTexture.Explosion]);
            pool[(int)ProjectileType.PseudoSniper] = new ProjectilePool<PseudoSniper>(color,textures[(int)ProjectileTexture.Sniper]);
            pool[(int)ProjectileType.Burst] = new ProjectilePool<BurstProjectile>(color,textures[(int)ProjectileTexture.Burst]);

        }
        public void Set(Point fromCenter, Vector2 direction, ProjectileType type)
        {
            switch (type)
            {
                case ProjectileType.Rocket:
                    ((ProjectilePool<RocketProjectile>)pool[(int)ProjectileType.Rocket]).CreateProjectile(fromCenter, direction);
                    break;
                case ProjectileType.PseudoSniper:
                    ((ProjectilePool<PseudoSniper>)pool[(int)ProjectileType.PseudoSniper]).CreateProjectile(fromCenter, direction);
                    break;
                case ProjectileType.Burst:
                    ((ProjectilePool<BurstProjectile>)pool[(int)ProjectileType.Burst]).CreateProjectile(fromCenter, direction);
                    break;
            }     
        }
        public void CreateProjectile(Point fromCenter, Point toCenter, ProjectileType type)
        {                                
            switch (type)
            {
                case ProjectileType.Rocket:
                    ((ProjectilePool<RocketProjectile>)pool[(int)ProjectileType.Rocket]).CreateProjectile(fromCenter, toCenter);
                     break;
                case ProjectileType.PseudoSniper:
                     ((ProjectilePool<PseudoSniper>)pool[(int)ProjectileType.PseudoSniper]).CreateProjectile(fromCenter, toCenter);
                     break;
                case ProjectileType.Burst:
                     ((ProjectilePool<BurstProjectile>)pool[(int)ProjectileType.Burst]).CreateProjectile(fromCenter, toCenter);
                     break;
            }           
        }
        public void Update()
        {
            for (int i = 0; i < pool.Length; i++)
            {
                pool[i].Update();       
            }
        }
        public void Draw(SpriteBatch spriteBatch, Point offset)
        {
            for (int i = 0; i < pool.Length; i++)
            {
                pool[i].Draw(spriteBatch, offset); 
            }
        }
        //Return gibt den neuen offset an
        public int WriteBytes(byte[] result, int offset)
        {
            for (int i = 0; i < pool.Length; i++)
            {
                offset = pool[i].WriteBytes(result, offset);
            }
            return offset;  
        }
        public int SetFromBytes(byte[] data, int offset)
        {
            for (int i = 0; i < pool.Length; i++)
            {
                offset = pool[i].SetFromBytes(data, offset);
            }
            return offset;  
        }
        public int Hitdetection(BasicHitBox unitHitBox)
        {            
            //gesammtschaden
            int result = 0;
            for (int i = 0; i < pool.Length; i++)
            {
                result += pool[i].Hitdetection(unitHitBox);
            }
            return result;        
        }


    }
}
