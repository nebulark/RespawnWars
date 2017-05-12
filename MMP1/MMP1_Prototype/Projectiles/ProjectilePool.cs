using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMP1_Prototype
{
    /// <summary>
    /// Verwaltet einen Pool der nur aus einer Projectilklasse besteht
    /// </summary>
    /// <typeparam name="T">ProjectilArt</typeparam>
    public class ProjectilePool <T>: I_ProjectilePool where T : A_Projectile, new()
    {        
        private List<T> storage;
        Texture2D[] textures;
        Color color;
        public ProjectilePool(Color color,params Texture2D[] textures)
        {
            storage = new List<T>(16);
            this.textures = textures;
            this.color = color;
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
           
            T neueProjectil = null;
            neueProjectil = new T();
            neueProjectil.Initialise(color, textures);
            neueProjectil.Set(fromCenter, toCenter);
            storage.Add(neueProjectil);
        }

        public void CreateProjectile(Point fromCenter, Vector2 direction)
        {
            //überschreib die nächste freie stelle           
            for (int i = 0; i < storage.Count; i++)
            {
                if (!storage[i].IsInUse())
                {
                    storage[i].Set(fromCenter, direction);
                    return;
                }
            }
            //Falls keine Freie Stelle gefunden wurde, erzeuge ein neues Object und initialisiere es
            //Fallunterscheidung für projectiltypen
            T neueProjectil = null;
            neueProjectil = new T();
            neueProjectil.Initialise(color,textures);           
            neueProjectil.Set(fromCenter, direction);
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
        public void Draw(SpriteBatch spriteBatch, Point offset)
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
            int loopcountindex = offset;
            offset++;
            byte cnt = 0;
            for (byte i = 0; i < storage.Count; i++)
            {
                //Allererste stelle muss freigehalten werden, erklärung folgt weiter unten
                if (storage[i].IsInUse())
                {
                    //Position in der Liste
                    result[offset] = i;
                    offset++;
                    //Datén
                    offset=storage[i].WriteBytes(result, offset);
                    cnt++;
                }
            }
            //Anzahl der Elemente die Folgen, kann erst im nachhinein bestimmt werden
            result[loopcountindex] = cnt;
            return offset;
        }
        public int SetFromBytes(byte[] data, int offset)
        {
            int loops = data[offset];
            offset++;
            int pos;
            //Alle die am Leben sind, sind im Packet enthalten        
            for (int i = 0; i < storage.Count; i++)
            {
                storage[i].Alive = false;
            }
            for (int i = 0; i < loops;  i++)
            {
                //Position in der Liste
                pos = data[offset];


                offset++;
                //Liste muss erweitert werden falls es aus dem bereich hinausragen würde
                //Erweitere bis count größer ist als pos
                //Die While schleife wird meist nicht aufgerufen                
                while (storage.Count <= pos)
                {
                    T newItem = new T();
                    newItem.Initialise(color, textures);
                    storage.Add(newItem);
                }
                // object gefunden erhöhe loopcount                              
                offset = storage[pos].SetFromBytes(data, offset);

            }
            return offset;
        }
        public int Hitdetection(BasicHitBox HitBox)
        {
            int TotalDamage = 0;
            for (int i = 0; i < storage.Count; i++)
            {
                if (storage[i].IsInUse())
                {
                    //fall treffer wird schaden dazuadiert und das Projektil gelöscht/ausgelöst
                    TotalDamage += storage[i].Hitdetection(HitBox);
                }
                
            }            
            return TotalDamage;
        }
    }
}
