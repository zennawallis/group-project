using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace Astorids
{
    /// <summary>
    /// This is the main type for your game.C:\Users\jack.hassett\Desktop\JackHasset Asteroids\MyTestGame\MyTestGame\Program.cs
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Song backroundMusic;

        enum EGameStates
        {
            GamePlay,
            GameOver,
            Restart,
        }

        List<ExplosionsClass> Explosions;

        Texture2D ShipExplosionTexture;
        Texture2D AsteroidExplosionTexture;

        Texture2D MissleTexture;
        List<MissleClass> MyMissles;

        TimeSpan LastShot = new TimeSpan(0, 0, 0, 0, 0);

        TimeSpan ShotCoolDown = new TimeSpan(0, 0, 0, 0, 100);

        public Texture2D AsteroidTexture;
        List<AsteroidClass> MyAsteroids;
        const int NUM_ASTEROIDS = 2;
        
        Random RandNum;

        ShipClass Ship;
        int PlayerLives = 10;
        SpriteFont ScoreText;
        SpriteFont GameOverText;
        Texture2D backgroundTexture;
        int Score = 0;
        EGameStates gameState = EGameStates.GamePlay;

        float gameOverTimer;





        public Game1()

        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            RandNum = new Random();
            Ship = new ShipClass();
            StartGame();

            base.Initialize();

        }

        public void ShipInitialise()
        {


            Ship.Position = new Vector2(
                graphics.PreferredBackBufferWidth / 2
                , graphics.PreferredBackBufferHeight / 2);
            Ship.Velocity = new Vector2(0, 0);
            Ship.Acceleration = 0;
            Ship.Rotation = 0;
            Ship.RotationDelta = 0;

            Ship.m_spawnPosition = Ship.Position;

            Ship.Size = new Vector2(32.0f, 32.0f);
            Ship.MaxLimit = new Vector2(graphics.PreferredBackBufferWidth + (Ship.Size.X / 2),
                graphics.PreferredBackBufferHeight + (Ship.Size.Y / 2));
            Ship.MinLimit = new Vector2(0 - (Ship.Size.X / 2), 0 - (Ship.Size.Y / 2));

            Ship.Dead = false;
            Ship.Visible = true;
            Ship.Vulnerable = false;

            Ship.m_respawnTime = 0f;
            Ship.m_invulnerableTime = 2.0f;
            Ship.m_respawnTimer = 1.0f;
            Ship.m_invulnerableTimer = 3.0f;

        }


        public void StartGame()
        {
            MyAsteroids = new List<AsteroidClass>();
            InitializeAsteroids();
            MyMissles = new List<MissleClass>();
            Explosions = new List<ExplosionsClass>();

            ShipInitialise();

            PlayerLives = 3;

            Score = 0;
        }


        public void InitializeAsteroids()
        {
            for (int i = 0; i < NUM_ASTEROIDS; ++i)
            {
                CreateAsteroid();
            }

        }

        public void CreateAsteroid()
        {
            AsteroidClass Asteroid = new AsteroidClass();
            Asteroid.Position = new Vector2(RandNum.Next(graphics.PreferredBackBufferWidth),
                RandNum.Next(10));
            Asteroid.Velocity = new Vector2(RandNum.Next(-3, 3), RandNum.Next(-3, 3));
            Asteroid.RotationDelta = RandNum.Next(1, 3);

            int RandSize = RandNum.Next(32, 56);
            Asteroid.Size = new Vector2(RandSize, RandSize);

            Asteroid.MaxLimit = new Vector2(graphics.PreferredBackBufferWidth + (Asteroid.Size.X + 10),
                graphics.PreferredBackBufferHeight + (Asteroid.Size.Y + 1));
            Asteroid.MinLimit = new Vector2(0 - (Asteroid.Size.X - 15), 0 - (Asteroid.Size.Y - 10));

            MyAsteroids.Add(Asteroid);

        }

        private bool CricleCollisionCheck(Vector2 Object1Pos, float Object1Radius,
            Vector2 Object2Pos, float Object2Radius)
        {
            float DistanceBetweenObjects = (Object1Pos - Object2Pos).Length();
            float SumOfRadii = Object1Radius + Object2Radius;

            if (DistanceBetweenObjects < SumOfRadii)
            {
                return true;
            }
            return false;
        }

        private void CheckCollisons()
        {
            List<AsteroidClass> AsteroidDeathRow = new List<AsteroidClass>();
            List<MissleClass> MissleDeathRow = new List<MissleClass>();

            foreach (AsteroidClass Asteroid in MyAsteroids)
            {
                bool PlayerCollisionCheck = CricleCollisionCheck(Ship.Position, Ship.Size.X / 2,
                    Asteroid.Position, Asteroid.Size.X / 2);
                if (PlayerCollisionCheck)
                {
                    Ship.Die();
                    AsteroidDeathRow.Add(Asteroid);
                    PlayerLives--;
                    CreateExplosion(Ship.Position, ExplosionType.SHIP);
                    CreateExplosion(Asteroid.Position, ExplosionType.ASTEROID);
                }

                foreach (MissleClass Missle in MyMissles)
                {
                    bool MissleCollisionCheck = CricleCollisionCheck(Missle.Position, Missle.Size.X / 2,
                        Asteroid.Position, Asteroid.Size.X / 2);
                  
                        Missle.Timer ++;

                    Missle.Position = WrapScreen(Missle.Position);
                    
                    if (Missle.Timer > 75)
                    {
                        MissleDeathRow.Add(Missle);
                    } 


                    if (MissleCollisionCheck)
                    {
                        Score++;
                        MissleDeathRow.Add(Missle);
                        AsteroidDeathRow.Add(Asteroid);
                        CreateExplosion(Asteroid.Position, ExplosionType.ASTEROID);
                    }
                }
            }

            foreach (AsteroidClass Asteroid in AsteroidDeathRow)
            {
                MyAsteroids.Remove(Asteroid);
               CreateAsteroid();

            }
            foreach (MissleClass Missle in MissleDeathRow)
            {
                MyMissles.Remove(Missle);
            }



        }
        protected void CreateExplosion(Vector2 SpawnPosition, ExplosionType SpawnedExplosionType)
        {
            ExplosionsClass NewExplosion = new ExplosionsClass();

            NewExplosion.CurrentFrame = 0;
            NewExplosion.FrameCount = 12;
            NewExplosion.FrameWidth = 128;
            NewExplosion.FrameHeight = 128;
            NewExplosion.Size = new Vector2(128, 128);

            NewExplosion.Interval = 1000.0f / 30.0f;
            NewExplosion.Position = SpawnPosition;
            NewExplosion.CurrentSprite = new Rectangle(0, 0, 128, 128);
            NewExplosion.MyType = SpawnedExplosionType;

            Explosions.Add(NewExplosion);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            Ship.Texture = Content.Load<Texture2D>("ship2");
            AsteroidTexture = Content.Load<Texture2D>("asteroid2");
            MissleTexture = Content.Load<Texture2D>("bullet");
            ScoreText = Content.Load<SpriteFont>("scoreFont");
            GameOverText = Content.Load<SpriteFont>("scoreFont");
            ShipExplosionTexture = Content.Load<Texture2D>("explosion");
            AsteroidExplosionTexture = Content.Load<Texture2D>("explosion2");
            backgroundTexture = Content.Load<Texture2D>("blue-space-1");
            backroundMusic = Content.Load<Song>("backroundSong");
            MediaPlayer.Play(backroundMusic);
            MediaPlayer.IsRepeating = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            // TODO: Add your update logic here
            switch (gameState)
            {
                case EGameStates.GamePlay:
                    // run  game
                    PlayGameUpdate(gameTime);
                    break;
                case EGameStates.GameOver:
                    PlayGameOverUpdate(gameTime);
                    // run game over
                    break;
                case EGameStates.Restart:
                    RestartUpdate(gameTime);
                    // run restart
                    break;
                default:
                    break;
            }

            base.Update(gameTime);

        }

        public bool Toggle(bool Var)
        {
            
            if( Var == true)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public void RestartUpdate(GameTime gameTime)
        {
            StartGame();
            // reset game vaules for new game
            gameState = EGameStates.GamePlay;
        }

        public void PlayGameUpdate(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Ship.Dead && Ship.m_invulnerableTime == 10f)
            {
                Ship.m_invulnerableTimer = Ship.m_invulnerableTime;
                Ship.m_respawnTimer = Ship.m_respawnTime;
            }

            if (Ship.m_respawnTimer > 0)
            {
                Ship.m_respawnTimer -= delta;
            }
            else if (Ship.m_respawnTimer < 0)
            {
                Ship.m_respawnTimer = 0;
                Ship.Respawn();
            }

            if (Ship.m_invulnerableTimer > 0)
            {
                Ship.m_invulnerableTimer -= delta;
            }
            else if (Ship.m_invulnerableTimer < 0)
            {
                Ship.m_invulnerableTimer = 0;
                Ship.Vulnerable = true;
            }

            Ship.Acceleration = 0f;
            Ship.RotationDelta = 0f;

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                Ship.Acceleration = -0.05f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                Ship.Acceleration = 0.10f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                Ship.RotationDelta = -0.10f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                Ship.RotationDelta = 0.10f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                Toggle(Ship.Saber);
            }


            if (Ship.Saber = true)
            {
               
            }
             else
            {
                
            }





            Ship.Rotation += Ship.RotationDelta;

            Ship.Velocity = Ship.Velocity / 1.02f;
            

            Matrix playerRotationMatrix = Matrix.CreateRotationZ(Ship.Rotation);

            Ship.Velocity += Vector2.Transform(new Vector2(0, Ship.Acceleration)
                , playerRotationMatrix);
            float speed = Ship.Velocity.Length();
            if (speed > Ship.MaxSpeed){
                Ship.Velocity.Normalize();
                Ship.Velocity = Ship.Velocity * Ship.MaxSpeed;
            }
            

            Ship.Position += Ship.Velocity;

            if (Ship.Position.X > Ship.MaxLimit.X)
            {
                Ship.Position.X = Ship.MinLimit.X;
            }
            else if (Ship.Position.X < Ship.MinLimit.X)
            {
                Ship.Position.X = Ship.MaxLimit.X;
            }

            if (Ship.Position.Y > Ship.MaxLimit.Y)
            {
                Ship.Position.Y = Ship.MinLimit.Y;
            }
            else if (Ship.Position.Y < Ship.MinLimit.Y)
            {
                Ship.Position.Y = Ship.MaxLimit.Y;
            }

            foreach (AsteroidClass Asteroid in MyAsteroids)
            {
                Asteroid.Rotation += Asteroid.RotationDelta;
                Asteroid.Position += Asteroid.Velocity;
                Asteroid.Position = WrapScreen(Asteroid.Position);

            }

            foreach (MissleClass Missle in MyMissles)
            {
                Missle.Position += Missle.Velocity;
                if (Missle.Position.X > Missle.MaxLimit.X)
                {
                    Missle.Velocity.X *= 5;
                }
                else if (Missle.Position.X < Missle.MinLimit.X)
                {
                    Missle.Velocity.X *= 5;
                }
            }
            Ship.Position = WrapScreen(Ship.Position);

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                TimeSpan timeSincelastShot = gameTime.TotalGameTime - LastShot;

                if (timeSincelastShot.Milliseconds > (ShotCoolDown.Milliseconds * Ship.FireRate))
                {
                    MissleClass Missle = new MissleClass();

                    Missle.Position = Ship.Position;

                    Missle.Rotation = Ship.Rotation;

                    Matrix MissleRotationMatrix = Matrix.CreateRotationZ(Missle.Rotation);
                    Missle.Velocity = new Vector2(0, -10);
                    Missle.Velocity = Vector2.Transform(Missle.Velocity, MissleRotationMatrix);
                    Missle.Velocity = Missle.Velocity + Ship.Velocity;

                    Missle.Size = new Vector2(16, 16);

                    Missle.MaxLimit = new Vector2(graphics.PreferredBackBufferWidth + 500,
                        graphics.PreferredBackBufferHeight + 500);
                    Missle.MinLimit = new Vector2(-500, -500);

                    MyMissles.Add(Missle);

                    LastShot = gameTime.TotalGameTime;
                }

            }


            if (Ship.Dead == true)
            {
                Ship.Respawn();
                //Score--;
            }
            if (Ship.Visible == true)
            {
                Ship.Vulnerable = true;
            }

            if (Ship.Acceleration > 1) {
                Ship.Acceleration = 1;
            }



            CheckCollisons();
            UpdateExplosions(gameTime);

            if (PlayerLives == 0)
            {
                gameState = EGameStates.GameOver;
                gameOverTimer = 3.0f;

            }
        }
    

        public Vector2 WrapScreen(Vector2 position)
        {
            if (position.X < 0)
            {
                position.X = (graphics.GraphicsDevice.Viewport.Width - 0);
            }
            if (position.X > graphics.GraphicsDevice.Viewport.Width)
            {
                position.X = 0;
            }
            if (position.Y < 0)
            {
                position.Y = (graphics.GraphicsDevice.Viewport.Height - 0);
            }
            if (position.Y > graphics.GraphicsDevice.Viewport.Height)
            {
                position.Y = 0;
            }
            return position;
        }

    
    

        public void PlayGameOverUpdate(GameTime gameTime)
        {
            if (gameOverTimer > 0)
            {
                gameOverTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (gameOverTimer == 0)
                {
                    gameState = EGameStates.Restart;
                }

            }
            if (gameOverTimer < 0)
            {
                gameState = EGameStates.Restart;
                gameOverTimer = 0;
            }
        }



        private void UpdateExplosions(GameTime gameTime)
        {
            List<ExplosionsClass> ToRemove = new List<ExplosionsClass>();

            foreach (ExplosionsClass Explosion in Explosions)
            {
                if (Explosion.CurrentFrame > Explosion.FrameCount - 1)
                {
                    ToRemove.Add(Explosion);
                    continue;
                }
                else
                {
                    Explosion.Timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (Explosion.Timer > Explosion.Interval)
                    {
                        ++Explosion.CurrentFrame;

                        Explosion.CurrentSprite = new Rectangle(
                            Explosion.CurrentFrame * Explosion.FrameWidth, 0,
                            Explosion.FrameWidth, Explosion.FrameHeight);

                        Explosion.Timer = 0;

                    }
                }
            }

            foreach (ExplosionsClass Explosion in ToRemove)
            {
                Explosions.Remove(Explosion);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();


            switch (gameState)
            {
                case EGameStates.GamePlay:
                    // run  game
                    PlayGameDraw(gameTime);
                    break;
                case EGameStates.GameOver:
                    GameOverDraw(gameTime);
                    // run game over
                    break;
                case EGameStates.Restart:
                    RestartDraw(gameTime);
                    // run restart
                    break;
                default:
                    break;
            }




            spriteBatch.End();
            base.Draw(gameTime);
        }


        public void RestartDraw(GameTime gameTime)
        {
        }
        public void PlayGameDraw(GameTime gameTime)
        {
            spriteBatch.Draw(backgroundTexture,
                   new Rectangle(0, 0, backgroundTexture.Width,
                   backgroundTexture.Height), Color.Blue);




            if (Ship.Visible)
            {


                spriteBatch.Draw(Ship.Texture
                    , Ship.Position
                    , null
                    , Color.White
                    , Ship.Rotation
                    , new Vector2(Ship.Texture.Width / 2, Ship.Texture.Height / 2)
                    , new Vector2(Ship.Size.X / Ship.Texture.Width, Ship.Size.Y / Ship.Texture.Height)
                    , SpriteEffects.None
                    , 0);

            }


            foreach (AsteroidClass Asteroid in MyAsteroids)
            {
                spriteBatch.Draw(AsteroidTexture,
                    Asteroid.Position,
                    null,
                    Color.LightYellow,
                    Asteroid.Rotation,
                    new Vector2(AsteroidTexture.Width / 2, AsteroidTexture.Height / 2),
                    new Vector2(Asteroid.Size.X / AsteroidTexture.Width, Asteroid.Size.Y / AsteroidTexture.Height),
                    SpriteEffects.None,
                    0);
            }

            foreach (MissleClass Missle in MyMissles)
            {
                spriteBatch.Draw(MissleTexture,
                    Missle.Position,
                    null,
                    Color.White,
                    Missle.Rotation,
                    new Vector2(MissleTexture.Width / 2, AsteroidTexture.Height / 2),
                    new Vector2(Missle.Size.X / AsteroidTexture.Width, Missle.Size.Y / AsteroidTexture.Height),
                    SpriteEffects.None,
                    0);
            }

            DrawLives();
            DrawScore();
            DrawExplosion();
        }
        public void GameOverDraw(GameTime gameTime)
        {
            GameOver();
        }

        private void DrawLives()
        {
            for (int i = 0; i < PlayerLives; ++i)
            {
                spriteBatch.Draw(Ship.Texture,
                    new Vector2(Ship.Size.X * (i + 1), Ship.Size.Y),
                    null,
                    Color.White,
                    0,
                    new Vector2(Ship.Texture.Width / 2, Ship.Texture.Height / 2),
                        Ship.Size.Y / Ship.Texture.Height,
                    SpriteEffects.None,
                    0);

            }
        }

        private void DrawScore()
        {
            spriteBatch.DrawString(ScoreText, "SCORE : " + Score, new Vector2(10, 40), Color.White);
            spriteBatch.DrawString(ScoreText, Ship.Score.ToString(), new Vector2(-10, 10), Color.White);

        }

        private void GameOver()
        {
            spriteBatch.DrawString(GameOverText, "GAME OVER YOUR SCORE WAS : " + Score, new Vector2(250, 200), Color.White);
        }


        private void DrawExplosion()
        {
            Texture2D TempText;

            foreach (ExplosionsClass Explosion in Explosions)
            {
                if (Explosion.MyType == ExplosionType.ASTEROID)
                {
                    TempText = AsteroidExplosionTexture;
                }
                else
                {
                    TempText = ShipExplosionTexture;
                }

                spriteBatch.Draw(TempText,
                    Explosion.Position,
                    Explosion.CurrentSprite,
                    Color.White, 0,
                    new Vector2(Explosion.FrameWidth / 2, Explosion.FrameHeight / 2),
                    new Vector2(Explosion.Size.X / Explosion.FrameWidth, Explosion.Size.Y / Explosion.FrameHeight),
                    SpriteEffects.None,
                    0);
            }
        }
    }
}
