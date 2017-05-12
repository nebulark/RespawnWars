using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMP1_Prototype
{
    class RocketProjectilePool : I_ProjectilePool
    {
        //8 Pos
        //8 End
        //1 alive
        //1 exploding 

        //1 Index
        public const int projectileDamage = 50;
        public const int SendingSize = 19;
        private List<RocketProjectile> storage;
        Texture2D[] textures;
        
        public RocketProjectilePool(Texture2D[] textures,int capacity = 16)
        {
            storage = new List<RocketProjectile>(capacity);
            this.textures = textures;
        }
        public void CreateProjectile(Point fromCenter, Point toCenter)
        {
            //überschreib die nächste freie stelle           
            for (int i = 0; i < storage.Count; i++)
            {
                if (!storage[i].IsInUse())
                {
                    storage[i].Set(fromCenter, toCenter);
                    return;
                }
            }
            //Falls keine Freie Stelle gefunden wurde, erzeuge ein neues Object und initialisiere es
            //Fallunterscheidung für projectiltypen
            RocketProjectile neueProjectil = null;
            neueProjectil = new RocketProjectile(textures[(int)ProjectileTexture.Rocket], textures[(int)ProjectileTexture.Explosion]);           
            neueProjectil.Set(fromCenter, toCenter);
            storage.Add(neueProjectil);
        }
        public void Update()
        {
            for (int i = 0; i < storage.Count; i++)
            {
                if (storage[i].IsInUse())
                {
                    storage[i].Update();
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            for (int i = 0; i < storage.Count; i++)
            {
                if (storage[i].IsInUse())
                {
                    storage[i].Draw(spriteBatch, offset);
                }
            }
        }
       
        //returned neuen offset
        public int WriteBytes(byte[] result, int offset)
        {
            byte cnt = 0;
            for (byte i = 0; i < storage.Count; i++)
            {
                //Allererste stelle muss freigehalten werden, erklärung folgt weiter unten
                if (storage[i].IsInUse())
                {
                    //Position in der Liste
                    result[offset + 1 + cnt * (SendingSize)] = i;
                    //Datén
                    storage[i].WriteBytes(result, offset + 2 + cnt * (SendingSize));
                    cnt++;
                }
            }
            //Anzahl der Elemente die Folgen, kann erst im nachhinein bestimmt werden
            result[offset] = cnt;
            return offset + 1 + cnt * (SendingSize);
        }
        public int SetFromBytes(byte[] data, int offset)
        {
            int loops = data[offset];
            int pos;
            for (int i = 0; i < loops; i++)
            {
                //Position in der Liste
                pos = data[offset + 1 + i * (SendingSize)];
                //Liste muss erweitert werden falls es aus dem bereich hinausragen würde
                //Erweitere bis count größer ist als pos
                //Die While schleife wird meist nicht aufgerufen                
                while (storage.Count <= pos)
                {
                    storage.Add(
                        new RocketProjectile(textures[(int)ProjectileTexture.Rocket],
                        textures[(int)ProjectileTexture.Explosion])
                        );
                }                                              
                storage[pos].SetFromBytes(data, offset + 2 + i * (SendingSize));                
            }
            return offset + 1 + loops * SendingSize;
        }
        public int Hitdetection(BasicHitBox HitBox)
        {
            int counter = 0;
            for (int i = 0; i < storage.Count; i++)
            {
                //Hitdetection lässt gegebenfalls die Rakete Explodieren
                if (storage[i].IsInUse() && storage[i].Hitdetection(HitBox))
                {
                    counter++;
                }                
            }
            return counter * projectileDamage;
        }
        
    }
}
