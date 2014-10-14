using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Battleship
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static Texture2D spriteSheet, colorTexture;
        public static SpriteFont font1, titlefont, menufont;
        public static SoundEffect battleship, hitSound, missSound;

        Field[] fields;
        int fields0X, fields0Y, fields1X, fields1Y, fieldSizeX, fieldSizeY, numberOfShips;
        Vector2 fields0Pos, fields1Pos;

        Button playButton, hideShipsButton, nextPhaseButton;
        Button[] menuButtons;

        public static int tileSize;
        public static bool hideShips;
        Texture2D titleBackground;
        string phaseInfo, winText, loseText, playerInfo;
        bool showPlayerInfo, useBomb;
        float playerInfoTime;
        int turnCount;
        Random rand;

        enum GameState { TitleScreen, P1PlaceShips, P1TargetPhase, P2PlaceShips, P2TargetPhase, GameOver };
        GameState currentGameState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            fieldSizeX = 10;
            fieldSizeY = 10;
            fields0X = 10;
            fields0Y = 10;
            fields1X = 10;
            fields1Y = 10;
            numberOfShips = 5;
            tileSize = 50;
            turnCount = 1;
            playerInfoTime = 0;
            showPlayerInfo = false;
            useBomb = false;
            currentGameState = GameState.TitleScreen;
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteSheet = Content.Load<Texture2D>("battleship_spritesheet");
            titleBackground = Content.Load<Texture2D>("bg");
            colorTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            colorTexture.SetData<Color>(new Color[] { Color.White });
            font1 = Content.Load<SpriteFont>("font1");
            titlefont = Content.Load<SpriteFont>("bigfont");
            menufont = Content.Load<SpriteFont>("menufont");
            hitSound = Content.Load<SoundEffect>("boom4");
            missSound = Content.Load<SoundEffect>("se_water");
            battleship = Content.Load<SoundEffect>("Untitled");
            fields = new Field[2];
            CreateFields();

            playButton = new Button(menufont, new Vector2(Window.ClientBounds.Width / 2 - 50, Window.ClientBounds.Height / 2), 80, 45, "Play");
            hideShipsButton = new Button(font1, new Vector2(Window.ClientBounds.Width - 175, Window.ClientBounds.Height - 30), 150, 25, "Always hide ships");
            nextPhaseButton = new Button(font1, new Vector2(Window.ClientBounds.Width/2 - 20, Window.ClientBounds.Height - 50), 40, 25, "Next");

            menuButtons = new Button[6];
            for (int i = 0; i < 3; i++)
            {
                menuButtons[i] = new Button(menufont, new Vector2(Window.ClientBounds.Width / 2 + 90, 500 + 70 * i), 25, 40, "+");
            }
            for (int i = 3; i < 6; i++)
            {
                menuButtons[i] = new Button(menufont, new Vector2(Window.ClientBounds.Width / 2 - 110, 495 + 70 * (i - 3)), 15, 50, "-");
            }

            winText = "Victory!";
            loseText = "Defeat!";
            currentGameState = GameState.TitleScreen;
        }

        private void CreateFields()     //Create field and place it appropriately on the screen depending on size
        {

            fields0X = fieldSizeX;
            fields0Y = fieldSizeY;

            fields1X = fieldSizeX;
            fields1Y = fieldSizeY;

            tileSize = fieldSizeX > fieldSizeY ? 500 / fieldSizeX : 500 / fieldSizeY;   //500 is total field height and width in pixels. Takes the higher of fieldSizeX and fieldSizeY and scales tileSize accordingly

            fields0Pos = new Vector2(Window.ClientBounds.Width / 4 - (fields0X * tileSize) / 2,
                                    Window.ClientBounds.Height / 2 - (fields0Y * tileSize) / 2);

            fields1Pos = new Vector2(Window.ClientBounds.Width / 4 * 3 - (fields1X * tileSize) / 2,
                                    Window.ClientBounds.Height / 2 - (fields1Y * tileSize) / 2);

            fields[0] = new Field(fields0X, fields0Y, (int)fields0Pos.X, (int)fields0Pos.Y, spriteSheet, tileSize, numberOfShips);
            fields[1] = new Field(fields1X, fields1Y, (int)fields1Pos.X, (int)fields1Pos.Y, spriteSheet, tileSize, numberOfShips);
        }

        public void DisplayPlayerInfo(string info)
        {
            playerInfo = info;
            showPlayerInfo = true;
            playerInfoTime = 0;
        }

        public void PlayerInfoUpdate(GameTime gameTime)
        {
            if (showPlayerInfo)
                playerInfoTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (playerInfoTime > 4)
            {
                showPlayerInfo = false;
                playerInfoTime = 0;
            }
        }

        public void UpdateShowError()
        {
            for (int i = 0; i < fields.Length; ++i)
            {
                if (fields[i].showError)
                {
                    DisplayPlayerInfo("Cannot place ship there");
                    fields[i].showError = false;
                }
            }
        }

        private bool ShipIsFollowingMouse()
        {
            foreach (Field field in fields)
                if (field.ShipIsFollowingMouse())
                    return true;
            return false;
        }

        private bool CheckAllShipsDestroyed(Field field)
        {
            foreach (Ship ship in field.ships)
            {
                if (!ship.destroyed)
                    return false;
            }
            return true;
        }

        private void UpdateMenuButtons()
        {
            foreach (Button button in menuButtons)
            {
                button.Update();
            }

            if (menuButtons[0].ButtonClicked() && fieldSizeX < 20)
                fieldSizeX++;
            if (menuButtons[1].ButtonClicked() && fieldSizeY < 20)
                fieldSizeY++;
            if (menuButtons[2].ButtonClicked() && numberOfShips < fieldSizeY)
                numberOfShips++;
            if (menuButtons[3].ButtonClicked() && fieldSizeX > 5)
                fieldSizeX--;
            if (menuButtons[4].ButtonClicked() && fieldSizeY > 5)
                fieldSizeY--;
            if (menuButtons[5].ButtonClicked() && numberOfShips > 1)
                numberOfShips--;
            if (numberOfShips > fieldSizeY)
                numberOfShips = fieldSizeY;
        }

        private void UpdateBombAt(Field field)
        {
            if (useBomb)
            {
                if (!field.bombed)
                {
                    Rectangle bombX = new Rectangle(KeyMouseReader.mouseState.X, 0, 1, Window.ClientBounds.Height);
                    Rectangle bombY = new Rectangle(0, KeyMouseReader.mouseState.Y, Window.ClientBounds.Width, 1);

                    if (KeyMouseReader.LeftClick() && !field.nextPhase)
                    {
                        foreach (Tile tile in field.grid)
                        {
                            if (bombX.Intersects(tile.rec) || bombY.Intersects(tile.rec))
                                field.FiredAt(new Point(tile.rec.X, tile.rec.Y));
                            field.bombed = true;
                            field.nextPhase = true;
                            useBomb = false;
                        }
                    }
                    foreach (Tile tile in field.grid)
                    {
                        if (bombX.Intersects(tile.rec) || bombY.Intersects(tile.rec))
                            tile.color = Color.Red;
                    }
                    DisplayPlayerInfo("LET'S SET THE WORLD ON FIRE");
                }
                if (field.bombed)
                    DisplayPlayerInfo("Sorry, the world is already burning");
            }
        }

        private void UpdateTargetPhase(Field field)
        {
            if (!field.nextPhase)
                field.FiredAt(KeyMouseReader.LeftClickPos);
            if (field.nextPhase)
                DisplayPlayerInfo("Press space to go to next turn or click the next button below");
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.R))
                currentGameState = GameState.TitleScreen;

            KeyMouseReader.Update();
            PlayerInfoUpdate(gameTime);
            fields[0].Update();
            fields[1].Update();
            UpdateShowError();

            if (fields[0].IsBattleShipSunk() || fields[1].IsBattleShipSunk())
            {
                rand = new Random();
                int b = rand.Next(1, 5);
                if(b==3)
                battleship.Play();
            }

            if (turnCount == 2)
                DisplayPlayerInfo("Getting bored yet? Press right click to blow stuff up");

            if (CheckAllShipsDestroyed(fields[0]) || CheckAllShipsDestroyed(fields[1]))
                currentGameState = GameState.GameOver;

            if (currentGameState == GameState.P1TargetPhase || currentGameState == GameState.P2TargetPhase)
            {
                hideShipsButton.Update();
                if (hideShipsButton.ButtonClicked())
                    hideShips = !hideShips;

                if (KeyMouseReader.RightClick())
                {
                    useBomb = !useBomb;
                }
            }

            switch(currentGameState)
            {
                case GameState.TitleScreen:
                    phaseInfo = "";
                    playButton.Update();
                    UpdateMenuButtons();

                    if (playButton.ButtonClicked())
                    {
                        CreateFields();
                        currentGameState = GameState.P1PlaceShips;
                        DisplayPlayerInfo("Press R to return to the menu at any time");
                    }
                    break;

                case GameState.P1PlaceShips:
                    phaseInfo = "Player 1 Place Ships";
                    fields[0].MoveShip(KeyMouseReader.LeftClickPos);
                    nextPhaseButton.Update();

                    if ((nextPhaseButton.ButtonClicked() || KeyMouseReader.KeyPressed(Keys.Space)) && !ShipIsFollowingMouse())
                        currentGameState = GameState.P2PlaceShips;
                    break;

                case GameState.P1TargetPhase:
                    phaseInfo = "Player 1 Shoot";
                    fields[0].nextPhase = false;
                    nextPhaseButton.Update();
                    UpdateBombAt(fields[1]);
                    UpdateTargetPhase(fields[1]);
                    
                    if(fields[1].nextPhase && (nextPhaseButton.ButtonClicked() || KeyMouseReader.KeyPressed(Keys.Space)))
                        currentGameState = GameState.P2TargetPhase;
                    break;

                case GameState.P2PlaceShips:
                    phaseInfo = "Player 2 Place Ships";
                    fields[1].MoveShip(KeyMouseReader.LeftClickPos);
                    nextPhaseButton.Update();

                    if ((nextPhaseButton.ButtonClicked() || KeyMouseReader.KeyPressed(Keys.Space)) && !ShipIsFollowingMouse())
                        currentGameState = GameState.P1TargetPhase;
                    break;

                case GameState.P2TargetPhase:
                    phaseInfo = "Player 2 Shoot";
                    fields[1].nextPhase = false;
                    nextPhaseButton.Update();
                    UpdateBombAt(fields[0]);
                    UpdateTargetPhase(fields[0]);

                    if (fields[0].nextPhase && (nextPhaseButton.ButtonClicked() || KeyMouseReader.KeyPressed(Keys.Space)))
                    {
                        ++turnCount;
                        currentGameState = GameState.P1TargetPhase;
                    }
                    break;

                case GameState.GameOver:
                    phaseInfo = "GameOver";
                    DisplayPlayerInfo("Press space to return to the menu");
                    fields[0].hideShips = false;
                    fields[1].hideShips = false;

                    if (KeyMouseReader.KeyPressed(Keys.Space) || nextPhaseButton.ButtonClicked())
                        Initialize();
                    break;
            }

            base.Update(gameTime);
        }

        private void DrawVictoryScreen()
        {
            if (CheckAllShipsDestroyed(fields[1]))
            {
                spriteBatch.DrawString(titlefont, winText, fields[0].fieldPos + fields[0].fieldSize / 2 - titlefont.MeasureString(winText) / 2, Color.Green, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(titlefont, loseText, fields[1].fieldPos + fields[1].fieldSize / 2 - titlefont.MeasureString(loseText) / 2, Color.Red, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
            }
            if (CheckAllShipsDestroyed(fields[0]))
            {
                spriteBatch.DrawString(titlefont, winText, fields[1].fieldPos + fields[1].fieldSize / 2 - titlefont.MeasureString(winText) / 2, Color.Green, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(titlefont, loseText, fields[0].fieldPos + fields[0].fieldSize / 2 - titlefont.MeasureString(loseText) / 2, Color.Red, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
            }
        }

        private void DrawFields()
        {
            float field1Alpha = 1f, field2Alpha = 1f;   //Defaults to this value unless conditions are met

            if (currentGameState == GameState.P1PlaceShips || currentGameState == GameState.P1TargetPhase)
            {
                field2Alpha = 0.5f;
                fields[0].DrawShips(spriteBatch);
            }
            else if (currentGameState == GameState.P2PlaceShips || currentGameState == GameState.P2TargetPhase)
            {
                field1Alpha = 0.5f;
                fields[1].DrawShips(spriteBatch);
            }
            else if(currentGameState == GameState.GameOver)
            {
                fields[0].DrawShips(spriteBatch);
                fields[1].DrawShips(spriteBatch);
            }
            if (currentGameState != GameState.TitleScreen)      //Draw statistics
            {
                spriteBatch.DrawString(Game1.font1, "Hits " + fields[0].hitCount.ToString(), new Vector2(fields[1].fieldRec.X + 50, fields[1].fieldRec.Y + fields[1].fieldRec.Height + 20), Color.White * field2Alpha);
                spriteBatch.DrawString(Game1.font1, "Misses " + fields[0].missCount.ToString(), new Vector2(fields[1].fieldRec.X + 50, fields[1].fieldRec.Y + fields[1].fieldRec.Height + 40), Color.White * field2Alpha);
                spriteBatch.DrawString(Game1.font1, "Accuracy " + fields[0].CalculateAccuracy() + "%", new Vector2(fields[1].fieldRec.X + 50, fields[1].fieldRec.Y + fields[1].fieldRec.Height + 60), Color.White * field2Alpha);
                                
                spriteBatch.DrawString(Game1.font1, "Hits " + fields[1].hitCount.ToString(), new Vector2(fields[0].fieldRec.X + 50, fields[0].fieldRec.Y + fields[0].fieldRec.Height + 20), Color.White * field1Alpha);
                spriteBatch.DrawString(Game1.font1, "Misses " + fields[1].missCount.ToString(), new Vector2(fields[0].fieldRec.X + 50, fields[0].fieldRec.Y + fields[0].fieldRec.Height + 40), Color.White * field1Alpha);
                spriteBatch.DrawString(Game1.font1, "Accuracy " + fields[1].CalculateAccuracy() + "%", new Vector2(fields[0].fieldRec.X + 50, fields[0].fieldRec.Y + fields[0].fieldRec.Height + 60), Color.White * field1Alpha);

                fields[0].Draw(spriteBatch, field1Alpha);
                fields[1].Draw(spriteBatch, field2Alpha);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            if (currentGameState != GameState.TitleScreen)
            {
                nextPhaseButton.Draw(spriteBatch);
                spriteBatch.DrawString(font1, "Turn " + turnCount.ToString(), new Vector2(5,Window.ClientBounds.Height-25), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                hideShipsButton.Draw(spriteBatch);
            }

            if(currentGameState == GameState.TitleScreen)
            {
                    spriteBatch.DrawString(titlefont, "BattleShips", new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2 - 100) - titlefont.MeasureString("BattleShips")/2, Color.White);
                    spriteBatch.DrawString(menufont, "Grid width", new Vector2(Window.ClientBounds.Width / 2 - 240, Window.ClientBounds.Height/2 + 160) - menufont.MeasureString("P1 Field Size") / 2, Color.White);
                    spriteBatch.DrawString(menufont, "Grid height", new Vector2(Window.ClientBounds.Width / 2 - 240, Window.ClientBounds.Height/2 + 230) - menufont.MeasureString("P2 Field Size") / 2, Color.White);
                    spriteBatch.DrawString(menufont, "Fleet size", new Vector2(Window.ClientBounds.Width / 2 - 268, Window.ClientBounds.Height/2 + 300) - menufont.MeasureString("Fleet Size") / 2, Color.White);
                    spriteBatch.DrawString(menufont, fieldSizeX.ToString(), new Vector2(Window.ClientBounds.Width / 2 + 12, 500 + 57) - titlefont.MeasureString(fieldSizeX.ToString()) / 2, Color.White);
                    spriteBatch.DrawString(menufont, fieldSizeY.ToString(), new Vector2(Window.ClientBounds.Width / 2 + 12, 500 + 127) - titlefont.MeasureString(fieldSizeY.ToString()) / 2, Color.White);
                    spriteBatch.DrawString(menufont, numberOfShips.ToString(), new Vector2(Window.ClientBounds.Width / 2 + 12, 500 + 197) - titlefont.MeasureString(numberOfShips.ToString()) / 2, Color.White);
                    playButton.Draw(spriteBatch);
                    spriteBatch.Draw(titleBackground, Vector2.Zero, null, new Color(60,60,60), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    foreach(Button button in menuButtons)
                    {
                        button.Draw(spriteBatch);
                    }
            }

            if(currentGameState == GameState.GameOver)
            {
                DrawVictoryScreen();
            }

            DrawFields();

            spriteBatch.DrawString(font1, phaseInfo, Vector2.Zero, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            if (showPlayerInfo)
                spriteBatch.DrawString(font1, playerInfo, new Vector2(Window.ClientBounds.Width/2, 10) - font1.MeasureString(playerInfo)/2, Color.White, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
