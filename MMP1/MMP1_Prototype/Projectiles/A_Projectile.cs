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
    /// Enthält alle Grundfunktionen der Projectile
    /// </summary>
    abstract public class A_Projectile
    {
        int movementSpeed;              
        public Rectangle Position;
        public Point Target;
        private Vector2 direction;
        private float rotationAngle;
        public bool Alive;
        private int range;
        public BasicHitBox Hitbox = new BasicHitBox(new Rectangle(0,0,0,0));
        Color color;
        protected byte damage;       
        
        public A_Projectile() { }        
        public void Initialise(Vector2 size, int speed, int range, Color color, byte damage, bool living = false)
        {
            Position = new Rectangle(0, 0, (int)(size.X + 0.5f), (int)(size.Y + 0.5f));
            movementSpeed = speed;
            Alive = living;
            this.range = range;
            this.color = color;
            this.damage = damage;
        }
        public void Initialise(Rectangle posSize, int speed, int range, Color color, bool living = false)
        {
            Position = posSize;
            movementSpeed = speed;
            Alive = living;
            this.range = range;
            this.color = color;
        }
        public abstract void Initialise(Color color, params Texture2D[] texture);
        
        public virtual void Set(Point fromCenter, Point toCenter, bool alive = true)
        {
            Position.Offset(fromCenter - Position.Center);            
            this.Alive = alive;                     
            direction = Vector2.Normalize((toCenter - fromCenter).ToVector2());
            Target = fromCenter + (direction*range).ToPoint();   
            rotationAngle = (float)Utilities.CalculateAngleFromVector(direction);                        
        }
        public virtual void Set(Point fromCenter, Vector2 direction, bool alive = true)
        {
            Position.Offset(fromCenter - Position.Center);
            this.Alive = alive;
            this.direction = direction;
            Target = fromCenter + (direction * range).ToPoint();
            rotationAngle = (float)Utilities.CalculateAngleFromVector(direction);
        }
        public virtual bool IsInUse()
        {
            return Alive;
        }
       
        public virtual void Update()
        {
            if (!Alive || Position.Center == Target) { return; }
            Vector2 distance = (Target - Position.Center).ToVector2();
            if (distance.LengthSquared() < Math.Pow(movementSpeed, 2))
            {
                Position.Offset(Target - Position.Center);
                return;
            }
            Position.Offset(Vector2.Normalize(distance) * movementSpeed);
            Hitbox = GetHitBoxFromPosition();           
        }
        public void Draw(SpriteBatch spriteBatch, Texture2D texture, Point offset)
        {
            if (!Alive) { return; }
            Vector2 spriteOrigin = texture.Bounds.Center.ToVector2();
            Rectangle locCenter = Position;
            locCenter.Offset(Position.Center - Position.Location);
            spriteBatch.Draw(texture, locCenter, null, color, rotationAngle, spriteOrigin, SpriteEffects.None, 0.0f);
            
           
            //spriteBatch.Draw(texture, Position, null, Color.White);
       }
        public virtual int WriteBytes(byte[] result, int offset)
        {
            result[0 + offset] = Convert.ToByte(Alive);            
            //result [1 bis 5] = Pos.X
            Array.Copy(BitConverter.GetBytes(Position.X), 0, result, 1 + offset, 4);
            //analog auch die anderen Werte            
            Array.Copy(BitConverter.GetBytes(Position.Y), 0, result, 5 + offset, 4);
            Array.Copy(BitConverter.GetBytes(Target.X), 0, result, 9 + offset, 4);
            Array.Copy(BitConverter.GetBytes(Target.Y), 0, result, 13 + offset, 4);
            
            return offset + 17;
        }
        /// <summary>
        /// Setzt die Werte nach den Werten des Packetes
        /// </summary>
        /// <param name="data">Packet</param>
        /// <param name="offset">Offset bei dem zu lesen begonnen werden soll</param>
        /// <returns>Neuer offset</returns>
        public virtual int SetFromBytes(byte[] data, int offset)
        {            
            Alive = Convert.ToBoolean( data[0 + offset]);                              
            Position.X = BitConverter.ToInt32(data, 1 + offset);
            Position.Y = BitConverter.ToInt32(data, 5 + offset);
            Target.X = BitConverter.ToInt32(data,  9 + offset);
            Target.Y = BitConverter.ToInt32(data, 13 + offset);
            direction = Vector2.Normalize((Target - Position.Center).ToVector2());           
            rotationAngle = (float)Utilities.CalculateAngleFromVector(direction);
            
            return offset + 17;     
        }
        public virtual BasicHitBox GetHitdetectionData()
        {
            return Hitbox;
        }
        public abstract void Draw(SpriteBatch spriteBatch, Point offset);
        public BasicHitBox GetHitBoxFromPosition()
        {
            return new BasicHitBox(Position);
        }

        public virtual byte Hitdetection(BasicHitBox OtherHitBox)
        {
            if (OtherHitBox.Colides(GetHitdetectionData()))
            {
                Alive = false;
                return damage;
            }
            return 0;
        }
        

    }
}
