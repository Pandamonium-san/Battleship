using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battleship
{
    class Ship
    {
        Texture2D texture;
        public Rectangle rec, spriteRec;
        Vector2 pos;
        public string id = "hao";
        int size;
        Color shipColor;

        public int damageTaken = 0;
        public bool followingMouse, destroyed, battleShipSunk = false;

        int tileSize, offset;

        float rotationAngle = 0;
        Vector2 vectorOrigin;
        bool invalidPos = false;
        int invalidCount;

        public Ship(Texture2D texture, int posX, int posY, int shipNumber)
        {
            this.texture = texture;
            this.tileSize = Game1.tileSize;
            this.offset = Game1.tileSize / 2;

            if (shipNumber % 5 == 0)
            {
                spriteRec = new Rectangle(5, 5, 240, 40);
                size = 5;
                this.id = "Carrier";
                this.rec = new Rectangle(posX, posY + tileSize * shipNumber, tileSize * 5, tileSize);
            }
            if (shipNumber % 5 == 1)
            {
                spriteRec = new Rectangle(5, 55, 190, 40);
                size = 4;
                this.id = "Battleship";
                this.rec = new Rectangle(posX, posY + tileSize * shipNumber, tileSize * 4, tileSize);
            }
            if (shipNumber % 5 == 2)
            {
                spriteRec = new Rectangle(5, 105, 140, 40);
                size = 3;
                this.id = "Submarine";
                this.rec = new Rectangle(posX, posY + tileSize * shipNumber, tileSize * 3, tileSize);
            }
            if (shipNumber % 5 == 3)
            {
                spriteRec = new Rectangle(5, 155, 140, 40);
                size = 3;
                this.id = "Destroyer";
                this.rec = new Rectangle(posX, posY + tileSize * shipNumber, tileSize * 3, tileSize);
            }
            if (shipNumber % 5 == 4)
            {
                spriteRec = new Rectangle(5, 205, 90, 40);
                size = 2;
                this.id = "Boat";
                this.rec = new Rectangle(posX, posY + tileSize * shipNumber, tileSize * 2, tileSize);
            }

            vectorOrigin = new Vector2(spriteRec.Width / 2, spriteRec.Height / 2);
            pos = new Vector2(rec.X + rec.Width / 2, rec.Y + rec.Height / 2);
            shipColor = Color.Silver;
        }

        public void SnapToGrid(Rectangle fieldRec, int tileSize)
        {
            rec.X = (((rec.X - fieldRec.X%tileSize + offset) / tileSize) * tileSize) + fieldRec.X%tileSize;
            rec.Y = (((rec.Y - fieldRec.Y%tileSize + offset) / tileSize) * tileSize) + fieldRec.Y%tileSize;
        }

        public void InvalidPos()
        {
            invalidPos = true;
        }

        public void FlipShip()
        {
            int temp;
            temp = rec.Width;
            rec.Width = rec.Height;
            rec.Height = temp;
            pos = new Vector2(rec.X + rec.Width / 2, rec.Y + rec.Height / 2);
            rotationAngle += (float)Math.PI / 2;
        }

        public void Update()
        {
            
            
            if (followingMouse)
            {
                rec.X = KeyMouseReader.mouseState.X - offset;
                rec.Y = KeyMouseReader.mouseState.Y - offset;
            }

            if (damageTaken >= size)
            {
                destroyed = true;
                shipColor = Color.Gray;
            }

            if(invalidPos)
            {
                shipColor = Color.Red;
                invalidCount++;
                if (invalidCount > 5) 
                {
                    shipColor = Color.Silver;
                    invalidCount = 0;
                    invalidPos = false;
                }
            }
            else
                pos = new Vector2(rec.X + rec.Width / 2, rec.Y + rec.Height / 2);       //Texture pos follows hitbox, offset compensates for vectorOrigin
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(texture, pos, spriteRec, shipColor, rotationAngle, vectorOrigin, tileSize/50f, SpriteEffects.None, 0.04f);
            spritebatch.DrawString(Game1.font1, id, pos, Color.White, 0f, Vector2.Zero, tileSize/50f, SpriteEffects.None, 0.01f);
            //spritebatch.Draw(texture, rec, spriteRec, Color.Red, 0f, Vector2.Zero, SpriteEffects.None, 0f);  //Draw hitbox
        }

    }
}
