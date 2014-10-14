using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battleship
{
    class Tile
    {
        public Texture2D texture;
        public Rectangle rec;
        public Color color;

        public bool hit = false, miss = false;

        Point mousePos;

        public Tile(Texture2D texture, Rectangle rec)
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
            { 
                color = Color.Green; 
            }

            if (miss)
            { 
                color = Color.Blue;

            }
        }

        public void Draw(SpriteBatch spritebatch, float alpha)
        {
            spritebatch.Draw(texture, rec, new Rectangle(200, 200, 50, 50), color * alpha * 0.5f, 0f, Vector2.Zero, SpriteEffects.None, 0.02f);

            if (hit)
            {
                spritebatch.Draw(texture, rec, new Rectangle(155, 105, 40, 40), Color.White * alpha * 0.5f, 0f, Vector2.Zero, SpriteEffects.None, 0.02f);
                miss = false;
            }   
            if (miss)
                spritebatch.Draw(texture, rec, new Rectangle(210, 110, 30, 30), Color.White * alpha * 0.5f, 0f, Vector2.Zero, SpriteEffects.None, 0.02f);
        } 
    }
}
