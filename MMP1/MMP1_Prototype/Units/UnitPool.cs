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
   /// Enthält einen Pool von Einheiten desselben Typs Managed diesen
   /// </summary>
   /// <typeparam name="T">EinheitenTyp</typeparam>
    class UnitPool<T> : I_UnitPool where T :  A_Unit, I_Hitable,  new()
        
    {                  
            
        private List<T> storage;
        private Texture2D texture;
        Color color;

        public UnitPool(Texture2D texture, Color color, int capacity = 16)
        {            
            storage = new List<T>(capacity);
            this.texture = texture;
            this.color = color;
            
        }
        public void CreateUnit(Point pos){            
            for (int i = 0; i < storage.Count; i++)
            {
                if (!storage[i].isAlive())
                {
                    storage[i].SetWithStandartValues(pos);                                      
                    return;
                }
            }
            T newUnit = new T();
            newUnit.Initialize(color,texture);
            newUnit.SetWithStandartValues(pos); 
            storage.Add(newUnit);

        }
        public void selectedUnitsMoveTo(Point position) {
            for (int i = 0; i < storage.Count; i++)
			{
                if (storage[i].isAlive() && storage[i].Selected)
                {
                    storage[i].changeTarget(position);
                }
			}
        }
        public void selectedUnitsShootAt(Point position, ProjectileManager projectileFactory)
        { 

        for (int i = 0; i < storage.Count; i++)
             {
                 if (storage[i].isAlive() && storage[i].Selected)
                 {
                     storage[i].Shoot(position, projectileFactory);                     
                 }
             }
        }
        public int selectNumberPressedfromSelected(int pressedNumber, int queueOffset = 0) {            
            
            for (int i = 0; i < storage.Count; i++)
             {
                 if (storage[i].isAlive() && storage[i].Selected)
                 {
                     if (queueOffset != pressedNumber) { storage[i].Selected = false; }
                     queueOffset++;                     
                 }
             }
            return queueOffset;
           
        }
        public void selectWithinBox(Rectangle box)
        {
            for (int i = 0; i < storage.Count; i++)
             {
                 if (storage[i].isAlive() && storage[i].Position.Intersects(box))
                 {
                     storage[i].Selected = true;                     
                 }
                 else
                 {
                     storage[i].Selected = false;                     
                 }
             }  
        }
        public void Update()
        {
            for (int i = 0; i < storage.Count; i++)
            {
                if (storage[i].isAlive())
                {
                    storage[i].Update();
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch, Point offset)
        {
            for (int i = 0; i < storage.Count; i++)
            {
                if (storage[i].isAlive())
                {
                    storage[i].Draw(spriteBatch, offset);
                }
            }
        }
         public int WriteBytes(byte[] result, int offset)
         {
             int loopcountindex = offset;
             offset++;
             byte cnt = 0;
             for (byte i = 0; i < storage.Count; i++)
             {
                 if (storage[i].isAlive())
                 {
                     //Position in der Liste
                     result[offset] = i;
                     offset++;
                     //Daten
                     offset = storage[i].WriteBytes(result,offset);
                     cnt++;
                 }
             }
             //gibt ann wieviel units folgen werden
             result[loopcountindex] = cnt;
             //neuer offset
             return offset;
         }
         public int SetFromBytes(byte[] data, int offset, int taktruckstand = 0)
         {
             int loops = data[offset];
             offset++;
             int pos;
             //Alle die Enthalten sind werden wieder auf true gesetzt
             for (int i = 0; i < storage.Count; i++)
             {
                 storage[i].Alive = false;
             }
             for (int i = 0; i < loops; i++)
             {
                 //Position in der Liste
                 pos = data[offset];
                 offset++;
                 //Liste muss erweitert werden falls es aus dem bereich hinausragen würde
                 //Erweitere bis count größer ist als pos
                 //Die While schleife wird meist nicht aufgerufen                
                 while (storage.Count <= pos)
                 {
                     storage.Add(new T());
                     storage[storage.Count - 1].Initialize(color,texture);
                 }
                 offset = storage[pos].SetFromBytes(data, offset, taktruckstand);
             }
             return offset;
         }
         public int Display(SpriteBatch spriteBatch ,int  offset = 0)
         {
             for (int i = 0; i < storage.Count; i++)
             {
                 if (storage[i].isAlive() && storage[i].Selected)
                 {
                     storage[i].Display(spriteBatch, new Rectangle(
                         offset * Utilities.DisplaySize,
                         0,
                         Utilities.DisplaySize,
                         Utilities.DisplaySize)
                         );
                     offset += 1;
                 }
             }
             return offset;
         }
         public int GetBoxMatchesNumber(BasicHitBox other)
         {
             int result = 0;
             for (int i = 0; i < storage.Count; i++)
             {
                 if (storage[i].isAlive())                     
                 {
                     if (storage[i].GetHitdetectionData().Colides(other))
                     {
                         result++;
                     }
                 }
             }
             return result;
         }
         public int Hitdetection(ProjectileManager projectileManager, byte[] result, int offset)
         {
             int damage;
             byte count = 0;
             int countIndex = offset;
             //Data[offset] == number of loops
             offset++;
             for (byte i = 0; i < storage.Count; i++)
             {
                 if (storage[i].isAlive())
                 {
                     damage = storage[i].Hitdetection(projectileManager);
                     //Falls schaden nicht 0 schreibe Index der einheit, die Momentanen LebensPunkte und die neuen LebensPunkte hinein
                     if (damage != 0)
                     {
                         result[offset] = i;                                                 
                         result[offset + 1] = (byte)Math.Max(0, storage[i].Health - damage);
                         offset += 2;
                         count++;                         
                     }                   
                 }                 
             }
             result[countIndex] = count;
             return offset;
         }
         public int AdjustUnitHealthToData(byte[] data, int offset)
         {
             //Anzahl units
             byte loops = data[offset];
             offset++;
             byte unitindex;             
             byte newHealth;
             for (int i = 0; i < loops; i++)
             {
                 unitindex = data[offset];                 
                 newHealth = data[offset + 1];
                 offset+=2;
                 //testet ob Packet auch korrekt ist
                 //zb doppeltes Packet oder verspätet
                 if (storage[unitindex].Health > newHealth)
                 {
                     //Paket ok, Leben wird angepasst
                     storage[unitindex].Health = newHealth;                     
                 }                 
             }
             return offset;
         }
         public int getLivingUnits()
         {
             int result = 0;
             for (int i = 0; i < storage.Count; i++)
             {
                 if (storage[i].isAlive())
                 {
                     result++;
                 }
             }
             return result;
         }
         public int getDieingUnits()
         {
             int result = 0;
             for (int i = 0; i < storage.Count; i++)
             {
                 if (storage[i].isDieing())
                 {
                     result++;
                 }
             }
             return result;
         }
    }
}
