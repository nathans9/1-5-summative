using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace _1_5_summative
{
    public class Game1 : Game
    {
        // if you move the mouse too fast, the collision does not kick in.
        //     - I assume this is because the update method is not called fast enough for the game to notice the mouse's change in location
        //     - As a solution, we can find the distance the mouse crossed in between update methods, and the distance puffles crossed between update methods, and if the puffles intersected with the mouse
        //     - As this is not my final project, I will not do this
        private static GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        //texture
        public static Texture2D peng, hBar, bg, mania1, mania2, intro, cursor, start, dark, light;
        public Texture2D[] end;
        public Texture2D[] colours;
        //numbers
        public static double startTime = 0, timeDiff, puffleTime, finishTime;
        public static int highscore;
        public static int hp = 300;
        //Rectangles
        public static Rectangle pengR, startR, puffR, exitR;
        public static List<Puffle> puffles = new List<Puffle>();

        //misc
        public Random rs = new Random();
        public static SoundEffect bonk;
        public Song game, idle;
        public static MouseState mouse;
        bool positive = true, hs = false, scheck = true;
        public SpriteFont text;
        public string[] iText = { "Using the mouse        ,", "dodge the puffles", "for as long as possible." };
        public string[] eText = { "NEW HIGHSCORE!", $"You survived for {finishTime} seconds!", $"Highscore: {highscore} seconds" };

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
            screen = Screen.Intro;
            this.Window.Title = "Puffle Mania";
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
            startR = new Rectangle(550, 100, 272, 152);
            pengR = new Rectangle(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2, 25, 25);
            exitR = new Rectangle(500, 50, 350, 200);
            base.Initialize();
        }

        protected override void LoadContent()
        {
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
            end = new Texture2D[9];
            end[0] = Content.Load<Texture2D>("worst");
            end[1] = Content.Load<Texture2D>("worse");
            end[2] = Content.Load<Texture2D>("worseish");
            end[3] = Content.Load<Texture2D>("ok");
            end[4] = Content.Load<Texture2D>("goodish");
            end[5] = Content.Load<Texture2D>("good");
            end[6] = Content.Load<Texture2D>("goodest");
            end[7] = Content.Load<Texture2D>("best");
            end[8] = Content.Load<Texture2D>("highsc");
            peng = Content.Load<Texture2D>("peng");
            hBar = Content.Load<Texture2D>("health");
            bg = Content.Load<Texture2D>("dojo bg");
            intro = Content.Load<Texture2D>("intropeng");
            mania1 = Content.Load<Texture2D>("mania1");
            mania2 = Content.Load<Texture2D>("mania2");
            text = Content.Load<SpriteFont>("text");
            cursor = Content.Load<Texture2D>("3119186");
            start = Content.Load<Texture2D>("start");
            dark = Content.Load<Texture2D>("dark");
            light = Content.Load<Texture2D>("light");
            bonk = Content.Load<SoundEffect>("bonk");
            idle = Content.Load<Song>("idle");
            game = Content.Load<Song>("game");
            for (int i = 0; i < puffles.Count; i++)
            {
                puffles[i].tex = colours[i % 8];
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            mouse = Mouse.GetState();
            MediaPlayer.IsRepeating = true;
            if (screen == Screen.Intro)
            {
                if(scheck)
                {
                    MediaPlayer.Play(idle);
                    scheck = false;
                }
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    if (startR.Contains(mouse.X, mouse.Y))
                    {
                        screen = Screen.Puffles;
                        scheck = true;
                        MediaPlayer.Stop();
                        startTime = gameTime.TotalGameTime.TotalSeconds;
                        puffleTime = gameTime.TotalGameTime.TotalSeconds;
                    }
                }
                }
            else if (screen == Screen.Puffles)
            {
                if(scheck)
                {
                    MediaPlayer.Play(game);
                    scheck = false;
                }    
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
                

                foreach (Puffle puffle in puffles)
                {
                    puffle.loco.X += puffle.speedX;
                    puffle.loco.Y += puffle.speedY;
                }

                BoundaryCheck();
                CollisionCheck();

                if (hp <= 0)
                {
                    screen = Screen.Exit;
                    MediaPlayer.Stop();
                    scheck = true;
                    finishTime = gameTime.TotalGameTime.TotalSeconds - puffleTime;
                }
            }
            else if (screen == Screen.Exit)
            {
                if (scheck)
                {
                    MediaPlayer.Play(idle);
                    foreach (string line in File.ReadLines("highscore.txt", Encoding.UTF8))
                    {
                        highscore = Convert.ToInt32(line);
                    }
                    if (Convert.ToInt32(finishTime) > highscore)
                    {
                        StreamWriter writer = new StreamWriter("highscore.txt");
                        writer.WriteLine(Convert.ToInt32(finishTime));
                        writer.Close();
                        hs = true;
                        finishTime = highscore;
                    }
                    scheck = false;
                }
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    if (exitR.Contains(mouse.X, mouse.Y))
                    {
                        MediaPlayer.Stop();
                        Environment.Exit(0);
                    }
                    
                }
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
                    _spriteBatch.DrawString(text, iText[i], new Vector2(450, 350 + 75 * i), Color.Red);
                }
                _spriteBatch.Draw(cursor, new Rectangle(750, 340, 60, 60), Color.White);
                _spriteBatch.Draw(colours[0], new Rectangle(780, 425, 60, 60), Color.White);
                _spriteBatch.Draw(start, startR, Color.White);
            }
            else if (screen == Screen.Puffles)
            {
                    foreach (Puffle puffle in puffles)
                    {
                        _spriteBatch.Draw(puffle.tex, puffle.loco, Color.White);
                    }
                    _spriteBatch.Draw(peng, pengR, Color.White);
                    _spriteBatch.Draw(hBar, new Rectangle(550, 50, hp, 20), Color.Red);
                    _spriteBatch.DrawString(text, Convert.ToString((int)(gameTime.TotalGameTime.TotalSeconds - puffleTime)), new Vector2(850, 45), Color.Red);
            }
            else if (screen == Screen.Exit)
            {
                eText[1] = $"You survived for {Convert.ToInt32(finishTime)} seconds!";
                eText[2] = $"Highscore: {highscore} seconds";
                if (exitR.Contains(mouse.X, mouse.Y))
                    _spriteBatch.Draw(light, exitR, Color.White);
                else
                    _spriteBatch.Draw(dark, exitR, Color.White);
                if (hs)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        _spriteBatch.DrawString(text, eText[i], new Vector2(400, 350 + 75 * i), Color.Red);
                    }
                    _spriteBatch.Draw(end[8], new Rectangle(75, 200, 313, 217), Color.White);
                }
                else
                {
                    for (int i = 1; i < 3; i++)
                    {
                        _spriteBatch.DrawString(text, eText[i], new Vector2(400, 350 + 75 * i), Color.Red);
                    }
                    if (finishTime < 6)
                    {
                        _spriteBatch.Draw(end[0], new Rectangle(75, 200, 313, 217), Color.White);
                    }
                    else if (6 < finishTime && finishTime < 12)
                    {
                        _spriteBatch.Draw(end[1], new Rectangle(75, 200, 177, 278), Color.White);
                    }
                    else if (12 < finishTime && finishTime < 18)
                    {
                        _spriteBatch.Draw(end[2], new Rectangle(75, 200, 219, 221), Color.White);
                    }
                    else if (18 < finishTime && finishTime < 24)
                    {
                        _spriteBatch.Draw(end[3], new Rectangle(75, 200, 211, 225), Color.White);
                    }
                    else if (24 < finishTime && finishTime < 30)
                    {
                        _spriteBatch.Draw(end[4], new Rectangle(75, 200, 209, 223), Color.White);
                    }
                    else if (30 < finishTime && finishTime < 40)
                    {
                        _spriteBatch.Draw(end[5], new Rectangle(75, 200, 264, 231), Color.White);
                    }
                    else if (40 < finishTime && finishTime < 60)
                    {
                        _spriteBatch.Draw(end[6], new Rectangle(75, 200, 206, 228), Color.White);
                    }
                    else if (60 < finishTime)
                    {
                        _spriteBatch.Draw(end[7], new Rectangle(75, 200, 210, 309), Color.White);
                    }
                }
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
                    bonk.Play();
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