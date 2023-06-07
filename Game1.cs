using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace _1_5_summative
{
    public class Game1 : Game
    {
        //TO DO: make puffle touhou
        //two phases, intro and exit screen
        // if you move the mouse too fast, the collision does not kick in.
        //     - I assume this is because the update method is not called fast enough for the game to notice the mouse's change in location
        //     - As a solution, we can find the distance the mouse crossed in between update methods, and the distance puffles crossed between update methods, and if the puffles intersected with the mouse
        //     - As this is not my final project, I will not do this
        private static GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        //texture
        public static Texture2D peng, hBar, bg, mania1, mania2, intro, cursor, start;
        public Texture2D[] colours;
        //numbers
        public double startTime = 0, timeDiff;
        public int counter = 0;
        public static int hp = 300;
        //Rectangles
        public static Rectangle pengR;
        public static Rectangle puffR;
        public static List<Puffle> puffles = new List<Puffle>();

        //misc
        public Random rs = new Random();
        public static MouseState mouse;
        bool positive = true;
        public SpriteFont text;
        public string[] iText = { "Using the mouse        ,", "dodge the puffles", "for as long as possible." };

        enum Screen
        {
            Intro,
            Puffles,
            Exit
        }

        Screen screen;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.PreferredBackBufferWidth = 900;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            this.Window.Title = "Touhou?";
            Window.AllowUserResizing = false;
            for (int i = 0; i < 300; i++)
            {
                Puffle puffle = new Puffle();

                int b = rs.Next(1, 3);
                if (b == 1)
                    positive = true;
                else
                    positive = false;
                puffle.speedX = rs.Next(1, 4);
                puffle.speedY = rs.Next(1, 4);
                puffle.loco.X = rs.Next(0, 874);
                puffle.loco.Y = rs.Next(-100, 0);
                if (!positive)
                    puffle.speedX = -puffle.speedX;

                puffles.Add(puffle);
            }

            pengR = new Rectangle(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2, 25, 25);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            screen = Screen.Intro;
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            colours = new Texture2D[8];
            colours[0] = Content.Load<Texture2D>("red");
            colours[1] = Content.Load<Texture2D>("green");
            colours[2] = Content.Load<Texture2D>("black");
            colours[3] = Content.Load<Texture2D>("pink");
            colours[4] = Content.Load<Texture2D>("purple");
            colours[5] = Content.Load<Texture2D>("white");
            colours[6] = Content.Load<Texture2D>("yellow");
            colours[7] = Content.Load<Texture2D>("blue");
            peng = Content.Load<Texture2D>("peng");
            hBar = Content.Load<Texture2D>("health");
            bg = Content.Load<Texture2D>("dojo bg");
            intro = Content.Load<Texture2D>("intropeng");
            mania1 = Content.Load<Texture2D>("mania1");
            mania2 = Content.Load<Texture2D>("mania2");
            text = Content.Load<SpriteFont>("text");
            cursor = Content.Load<Texture2D>("3119186");
            for (int i = 0; i < puffles.Count; i++)
            {
                puffles[i].tex = colours[i % 8];
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (screen == Screen.Puffles)
            {
                timeDiff = gameTime.TotalGameTime.TotalSeconds - startTime;
                if (timeDiff > 6)
                {
                    for (int i = 0; i < 300; i++)
                    {
                        Puffle puffle = new Puffle();

                        int b = rs.Next(1, 3);
                        if (b == 1)
                            positive = true;
                        else
                            positive = false;
                        puffle.speedX = rs.Next(1, 4);
                        puffle.speedY = rs.Next(1, 4);
                        puffle.loco.X = rs.Next(0, 874);
                        puffle.loco.Y = rs.Next(-100, 0);
                        if (!positive)
                            puffle.speedX = -puffle.speedX;
                        puffle.tex = colours[i % 8];
                        puffles.Add(puffle);
                    }
                    startTime = gameTime.TotalGameTime.TotalSeconds;
                }
                Debug.WriteLine((int)gameTime.TotalGameTime.TotalSeconds);
                mouse = Mouse.GetState();

                foreach (Puffle puffle in puffles)
                {
                    puffle.loco.X += puffle.speedX;
                    puffle.loco.Y += puffle.speedY;
                }

                BoundaryCheck();
                CollisionCheck();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(bg, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
            if (screen == Screen.Intro)
            {
                _spriteBatch.Draw(intro, new Rectangle(-150, 200, 700, 640), Color.White);
                if ((int)gameTime.TotalGameTime.TotalSeconds % 2 == 0)
                    _spriteBatch.Draw(mania1, new Rectangle(20, 50, 450, 153), Color.White);
                else
                    _spriteBatch.Draw(mania2, new Rectangle(20, 50, 450, 153), Color.White);
                for (int i = 0; i < 3; i++)
                {
                    _spriteBatch.DrawString(text, iText[i], new Vector2(450, 250 + 75 * i), Color.Red);
                }
            }
            if (screen == Screen.Puffles)
                {
                    foreach (Puffle puffle in puffles)
                    {
                        _spriteBatch.Draw(puffle.tex, puffle.loco, Color.White);
                    }
                    _spriteBatch.Draw(peng, pengR, Color.White);
                    _spriteBatch.Draw(hBar, new Rectangle(550, 50, hp, 20), Color.Red);
                }

                _spriteBatch.End();

                base.Draw(gameTime);
            
        }

        public static void BoundaryCheck()
        {
            if (mouse.X > _graphics.PreferredBackBufferWidth - 25)
                pengR.X = _graphics.PreferredBackBufferWidth - 25;
            else if (mouse.X < 0)
                pengR.X = 0;
            else
                pengR.X = mouse.X;

            if (mouse.Y > _graphics.PreferredBackBufferHeight - 25)
                pengR.Y = _graphics.PreferredBackBufferHeight - 25;
            else if (mouse.Y < 0)
                pengR.Y = 0;
            else
                pengR.Y = mouse.Y;

            List<Puffle> validPuffles = new List<Puffle>();
            foreach (Puffle puffle in puffles)
            {
                if (puffle.loco.Y > _graphics.PreferredBackBufferHeight)
                {
                    continue;
                }
                if (puffle.loco.X > _graphics.PreferredBackBufferWidth || puffle.loco.X + 25 < 0)
                {
                    continue;
                }
                validPuffles.Add(puffle);
            }
            puffles = validPuffles;
        }

        public static void CollisionCheck()
        {
            List<Puffle> validPuffles = new List<Puffle>();
            foreach (Puffle puffle in puffles)
            {
                if (puffle.loco.Intersects(pengR))
                {
                    hp -= 15;
                    continue;
                }
                validPuffles.Add(puffle);
            }
            puffles = validPuffles;
        }
    }

        public class Puffle
        {
            public int speedY, speedX;
            public Rectangle loco;
            public Texture2D tex;

            public Puffle()
            {
                loco = new Rectangle(0, 0, 25, 25);
            }
        }
    }