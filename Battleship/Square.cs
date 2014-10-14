using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battleship
{
    class Square
    {
        Texture2D texture;
        Rectangle rec;
        Color color;

        public bool hit = false;
        public bool miss = false;

        Point mousePos;

        public Square(Texture2D texture, Rectangle rec)
        {
            this.texture = texture;
            this.rec = rec;
        }

        public void Update()
        {
            mousePos = new Point(KeyMouseReader.mouseState.X, KeyMouseReader.mouseState.Y);
            if (rec.Contains(mousePos))
                color = Color.Red;
            else
                color = Color.White;

            if (hit)
                color = Color.Green;
            if (miss)
                color = Color.Blue;

        }

        public bool HitCheck(Point mouseClick)
        {
            if (rec.Contains(mouseClick))
                return true;
            else
                return false;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(texture, rec, new Rectangle(200, 200, 50, 50), color);

            if (hit)
            {
                spritebatch.Draw(texture, rec, new Rectangle(155, 105, 40, 40), Color.White);
                miss = false;
            }   
            if (miss)
                spritebatch.Draw(texture, rec, new Rectangle(210, 110, 30, 30), Color.White);
        } 
    }
}
