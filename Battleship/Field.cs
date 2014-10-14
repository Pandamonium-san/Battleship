using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battleship
{
    class Field
    {
        public Tile[,] grid;
        public Rectangle tileRec, fieldRec;
        public List<Ship> ships;
        public Vector2 fieldSize, fieldPos;

        public bool nextPhase, hideShips, isHoldingShip, showError;

        int tileSize, simultaneousSoundEffects;
        public double hitCount, missCount;
        public double accuracy;

        public bool bombed = false;

        public Field(int a, int b, int posX, int posY, Texture2D spriteSheet, int tileSize, int numberOfShips)
        {
            this.grid = new Tile[a,b];
            this.ships = new List<Ship>();
            this.fieldRec = new Rectangle(posX, posY, tileSize * a, tileSize * b);
            this.fieldSize = new Vector2(tileSize * a, tileSize * b);
            this.fieldPos = new Vector2(posX, posY);

            this.tileSize = Game1.tileSize;

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    tileRec = new Rectangle(posX + tileSize * i, posY + tileSize * j, tileSize, tileSize);
                    grid[i, j] = new Tile(spriteSheet, tileRec);
                }
            }

            for (int i = 0; i < numberOfShips; i++)
            {
                ships.Add(new Ship(spriteSheet, posX, posY, i));
            }

        }

        private bool IsPosValid(Ship ship1)       //Check if ship has valid position
        {
            if (!fieldRec.Contains(ship1.rec))
                return false;

            foreach (Ship ship2 in ships)
            {
                if (ship1.rec.Intersects(ship2.rec) && ship1 != ship2)
                {
                    return false;
                }
            }
            return true;
        }

        public void MoveShip(Point mouseClick)
        {
            foreach (Ship ship in ships)
            {
                if (ship.rec.Contains(mouseClick) && !isHoldingShip)     //Hold ship
                {
                    ship.followingMouse = true;
                    isHoldingShip = true;
                }
                else if (KeyMouseReader.LeftClick() && ship.followingMouse)  //Release ship and check if position is valid
                {
                    ship.SnapToGrid(fieldRec, tileSize);
                    if (IsPosValid(ship))
                    {
                        ship.followingMouse = false;
                        isHoldingShip = false;
                    }
                    else
                    {
                        ship.InvalidPos();
                        showError = true;
                    }
                }
                else if (KeyMouseReader.RightClick() && ship.followingMouse)    //Flip ship
                {
                    ship.FlipShip();
                }
            }
        }

        public void Hit(Ship ship, Tile tile)
        {
            tile.hit = true;
            if (simultaneousSoundEffects < 4)       //Prevents major earrape
            {
                Game1.hitSound.Play(0.10f, 0f, 0f);
                simultaneousSoundEffects++;
            }
            ship.damageTaken++;
            nextPhase = true;
        }

        public void Miss(Tile tile)
        {
            tile.miss = true;
            if (simultaneousSoundEffects < 4)
            {
                Game1.missSound.Play(0.02f, 0f, 0f);
                simultaneousSoundEffects++;
            }
            nextPhase = true;
        }

        public void FiredAt(Point mouseClick)
        {
            foreach (Tile tile in grid)
            {
                if (tile.hit || tile.miss)
                    continue;
                foreach (Ship ship in ships)
                {
                    if (ship.rec.Contains(mouseClick) && tile.rec.Contains(mouseClick))
                    {
                        Hit(ship, tile);
                    }
                }
                foreach (Ship ship in ships)
                {
                    if (!ship.rec.Contains(mouseClick) && tile.rec.Contains(mouseClick) && !tile.hit && !tile.miss)
                    {
                        Miss(tile);
                    }
                }
            }
        }

        public double CalculateAccuracy()
        {
            hitCount = 0;
            missCount = 0;
            foreach (Tile tile in grid)
            {
                if (tile.hit)
                    hitCount++;
                if (tile.miss)
                    missCount++;
            }
            if(hitCount != 0 || missCount != 0)
            {
            accuracy = (hitCount / (hitCount + missCount)) * 100;   //Calculate accuracy in %
            }
            accuracy = Math.Round(accuracy, 2);
            return accuracy;
        }

        public bool ShipIsFollowingMouse()     //Checks if any ship is currently picked up
        {
            foreach (Ship ship in ships)
                if (ship.followingMouse)
                    return true;
            return false;
        }

        public void Update()
        {
            hideShips = Game1.hideShips;
            
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i,j].Update();
                }
            }

            foreach (Ship ship in ships)
            {
                ship.Update();
            }
            simultaneousSoundEffects = 0;
        }

        public void Draw(SpriteBatch spriteBatch, float alpha)
        {
                foreach (Ship ship in ships)
                {
                    if(ship.destroyed)
                        ship.Draw(spriteBatch);
                }

            foreach (Tile tile in grid)
                tile.Draw(spriteBatch, alpha);
            spriteBatch.Draw(Game1.colorTexture, fieldRec, null, Color.DarkBlue * alpha, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            
        }

        public void DrawShips(SpriteBatch spriteBatch)
        {
            if (!hideShips)
            {
                foreach (Ship ship in ships)
                {
                    ship.Draw(spriteBatch);
                }
            }
        }

        public bool IsBattleShipSunk()
        {
            foreach (Ship ship in ships)
                if (ship.id == "Battleship" && ship.destroyed && !ship.battleShipSunk)
                {
                    ship.battleShipSunk = true;
                    return true;
                }
            return false;
        }
        
    }
}
