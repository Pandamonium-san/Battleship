using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battleship
{
    class Button
    {
        Rectangle rec;
        String name;
        Color color;
        Point mousePos;
        SpriteFont font;

        public Button(SpriteFont font, Vector2 rec, int recW, int recH, String name)
        {
            this.font = font;
            this.name = name;
            this.rec =  new Rectangle((int)rec.X,(int)rec.Y,recW,recH);
            color = Color.Black;
        }

        public bool ButtonClicked()
        {
            if(rec.Contains(KeyMouseReader.LeftClickPos))
                return true;
            else
                return false;
        }

        public void Update()
        {
            mousePos = new Point(KeyMouseReader.mouseState.X, KeyMouseReader.mouseState.Y);

            if (rec.Contains(mousePos))
                color = new Color(255,255,255);
            else
                color = new Color(155,155,155);
        }

        public void Draw(SpriteBatch spritebatch)
        {
            //spritebatch.Draw(Game1.colorTexture, rec, null, new Color(55,55,55,155), 0f, Vector2.Zero, SpriteEffects.None, 1f);   //Draw hitbox
            spritebatch.DrawString(font, name, new Vector2(rec.X, rec.Y), color, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
        }
    }
}
