using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using static NR2003DashM.NR2003Types;
using System.Runtime.InteropServices;
using System.Threading;
using NR2003DashM.Util;
using System.Linq;
using System.Collections.Generic;

namespace NR2003DashM
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private RaceInfo _raceInfo;

        // global app stuff
        bool testSpin = false;
        bool testHold = false;
        bool drawTopTextBlock = true;

        static float LargeScale = 350;
        static float MediumScale = 256;
        static float smallScale = 200;

        //bg
        Texture2D backgroundSprite;

        // Gauges
        Texture2D tach_faceSprite;
        Texture2D tach_needleSprite;
        Texture2D oil_temp_faceSprite;
        Texture2D oil_temp_needleSprite;
        Texture2D oil_pres_faceSprite;
        Texture2D oil_pres_needleSprite;
        Texture2D water_temp_faceSprite;
        Texture2D water_temp_needleSprite;
        Texture2D fuel_pres_faceSprite;
        Texture2D fuel_pres_needleSprite;
        // fonts
        SpriteFont rpmFont;
        SpriteFont OilTempFont;
        SpriteFont OilPresFont;
        SpriteFont WaterTempFont;
        SpriteFont FuelPresFont;

        // positions
        Vector2 tach_faceSpritePosition = new Vector2(0, 100);
        Vector2 tach_needleSpritePosition = new Vector2(0, 0); // this gets set by the face
        Vector2 tach_rpmFontPosition = new Vector2(0, 0);

        Vector2 oil_temp_faceSpritePosition = new Vector2(0, 0);
        Vector2 oil_temp_needleSpritePosition = new Vector2(0, 0);
        Vector2 oil_temp_FontPosition = new Vector2(0, 0);

        Vector2 oil_pres_faceSpritePosition = new Vector2(0, 0);
        Vector2 oil_pres_needleSpritePosition = new Vector2(0, 0);
        Vector2 oil_pres_FontPosition = new Vector2(0, 0);

        Vector2 water_temp_faceSpritePosition = new Vector2();
        Vector2 water_temp_needleSpritePosition = new Vector2();
        Vector2 water_temp_FontPosition = new Vector2();

        Vector2 fuel_pres_faceSpritePosition = new Vector2();
        Vector2 fuel_pres_needleSpritePosition = new Vector2();
        Vector2 fuel_pres_FontPosition = new Vector2();

        // scales
        Vector2 scale;
        Vector2 needleScale;
        Vector2 mediumNeedleScale;

        Vector2 oiltempScale;
        Vector2 oilpresScale;
        Vector2 oiltempneedleScale;
        Vector2 oilpresneedleScale;

        float tac_faceSpriteX = LargeScale;
        float tac_faceSpriteY;

        float tach_needleSpriteX;
        float tach_needleSpriteY;

        // Face Sprite Scaling
        float oil_temp_faceSpriteWidth = smallScale;
        float oil_temp_needleSpriteWidth;
        float oil_pres_faceSpriteWidth = smallScale;
        float oil_pres_needleSpriteWidth;
        float water_temp_faceSpriteWidth = smallScale;
        float water_temp_needleWpriteWidth;
        float fuel_pres_faceSpriteWidth = smallScale;
        float fuel_pres_needleWpriteWidth;

        // Default Needle Rotation 
        float tach_needleRotation = -90;
        float oil_tempneedleRoatation = -90;
        float oil_pres_needleRotation = -90;
        float water_temp_needleRotation = 0;
        float fuel_pres_needleRotation = 0;

        // Needle Origins (Where do we rotate)
        Vector2 needleOrigin;
        Vector2 oilTempNeedleOrigin;
        Vector2 oilPresNeedleOrigin;
        Vector2 waterTempNeedleOrigin;
        Vector2 fuelPresNeedleOrigin;

        string TachValueText = "";
        string TachRotationText = "";
        string oilTempRotation = "";
        string oilTempText = "";
        string oilPresText = "";
        string oilPresRotationText = "";
        string waterTempText = "";
        string fuelPresText = "";

        float rpmVal;
        float oilTempVal;
        float oilPresVal;
        float waterTempVal;
        float fuelPresVal;

        // vals from gauges.
        float gaugeRPM;
        float gaugeOilTemp;
        float gaugeOilPres;
        float gaugewWaterTemp;
        float gaugeFuel;
        float gaugeFuelPres;
        float gaugeVoltage;

        bool rpmWarning = false;
        bool oilTempWarning = false;
        bool oilPresWarning = false;
        bool waterTempWarning = false;
        bool fuelPresWarning = false;

        Color oilTempSpriteColor = Color.White;
        Color rpmSpriteColor = Color.White;
        Color oilPresSpriteColor = Color.White;
        Color waterTempSpritecolor = Color.White;
        Color fuelPresSpritecolor = Color.White;

        Color oilTempNeedleSpriteColor = Color.White;
        Color rpmNeedleSpriteColor = Color.White;
        Color oilPresNeedleSpriteColor = Color.White;
        Color waterTempNeedleSpritecolor = Color.White;
        Color fuelPresNeedleSpritecolor = Color.White;

        // Top Text Bar Section
        SpriteFont gameFont;
        SpriteFont MiddleInfoFont;
        SpriteFont RightInfoFont;

        Vector2 middleInfo_FontPosition = new Vector2(0, 0);
        Vector2 lapInfo_FontPosition = new Vector2(0, 0);
        Vector2 rightInfo_FontPosition = new Vector2(0, 0);
        Vector2 mediumFaceScale;

        string LastLapTimeText = "";
        string LapInfoText = "";
        string MiddleInfoText = "";
        string RightInfoText = "";
        float LapTime;


        // Debug Standings
        SpriteFont StandingsFont;
        Vector2 standings_FontPosition = new Vector2();
        string StandingsText = "";



        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            // Stop the threads
            NR2003Binding.EndTelemetry();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            _raceInfo = new RaceInfo();

            // get gaguedata

            Thread GaugeDataThread = new Thread(GetGagueInfoBG)
            {
                IsBackground = true
            };
            GaugeDataThread.Start();


            // getdata
            Thread GaugeUpdateThread = new Thread(GetGameInfo)
            {
                IsBackground = true
            };
            GaugeUpdateThread.Start();
        }

        // load art here
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            //bg
            backgroundSprite = Content.Load<Texture2D>("carbon");

            // Load all our Gauges
            LoadGauges();

            // Middle info section            
            LoadTextBlocks();
        }

        /// <summary>
        /// All text block content is loaded here
        /// </summary>
        private void LoadTextBlocks()
        {
            middleInfo_FontPosition.X = (_graphics.PreferredBackBufferWidth / 2) - (tac_faceSpriteX / 2);
            middleInfo_FontPosition.Y = 0;

            lapInfo_FontPosition.X = 0;
            lapInfo_FontPosition.Y = 0;

            rightInfo_FontPosition.X = (_graphics.PreferredBackBufferWidth / 2) + 60;
            rightInfo_FontPosition.Y = 0;

            standings_FontPosition.X = 0;
            standings_FontPosition.Y = 0;


            gameFont = Content.Load<SpriteFont>("faceFont");
            MiddleInfoFont = Content.Load<SpriteFont>("faceFont");
            RightInfoFont = Content.Load<SpriteFont>("faceFont");
            StandingsFont = Content.Load<SpriteFont>("faceFont");
        }

        /// <summary>
        /// All Gauge content is loaded here
        /// </summary>
        private void LoadGauges()
        {
            //tach_face.xnb
            tach_faceSprite = Content.Load<Texture2D>("tach_face");
            tach_needleSpriteX = tac_faceSpriteX / 1.28f;
            oil_temp_needleSpriteWidth = oil_temp_faceSpriteWidth / 1.28f;
            oil_pres_needleSpriteWidth = oil_pres_faceSpriteWidth / 1.28f;
            water_temp_needleWpriteWidth = water_temp_needleWpriteWidth / 1.28f;
            fuel_pres_needleWpriteWidth = fuel_pres_needleWpriteWidth / 1.28f;

            tach_faceSpritePosition.X = (_graphics.PreferredBackBufferWidth / 2) - (tac_faceSpriteX / 2);
            tach_faceSpritePosition.Y = (_graphics.PreferredBackBufferHeight / 2) - (tac_faceSpriteX / 2);
            tach_rpmFontPosition = new Vector2(tach_faceSpritePosition.X + (tac_faceSpriteX / 2) + 30, tach_faceSpritePosition.Y + (tac_faceSpriteX / 2) - 10);

            // needle.xnb
            tach_needleSprite = Content.Load<Texture2D>("bigneedle");
            // move tach needle to face
            tach_needleSpritePosition.X = tach_faceSpritePosition.X + (tac_faceSpriteX / 2);
            tach_needleSpritePosition.Y = tach_faceSpritePosition.Y + (tac_faceSpriteX / 2);

            // oil temp
            oil_temp_faceSprite = Content.Load<Texture2D>("oil_temp_face");
            oil_temp_faceSpritePosition.X = tach_faceSpritePosition.X + (tac_faceSpriteX);
            //oil_temp_faceSpritePosition.Y = tach_faceSpritePosition.Y - (oil_temp_faceSpriteWidth / 4);
            oil_temp_faceSpritePosition.Y = tach_faceSpritePosition.Y;

            oil_temp_FontPosition = new Vector2(oil_temp_faceSpritePosition.X + (oil_temp_faceSpriteWidth / 2) + 30, oil_temp_faceSpritePosition.Y + (oil_temp_faceSpriteWidth / 2) - 10);

            // oil needle
            oil_temp_needleSprite = Content.Load<Texture2D>("bigneedle");
            oil_temp_needleSpritePosition = new Vector2(oil_temp_faceSpritePosition.X + (oil_temp_faceSpriteWidth / 2), oil_temp_faceSpritePosition.Y + (oil_temp_faceSpriteWidth / 2));

            // oil pressure
            oil_pres_faceSprite = Content.Load<Texture2D>("oil_press_face");
            oil_pres_faceSpritePosition.X = tach_faceSpritePosition.X + (tac_faceSpriteX);
            //oil_pres_faceSpritePosition.Y = tach_faceSpritePosition.Y + (tac_faceSpriteX / 2);
            oil_pres_faceSpritePosition.Y = tach_faceSpritePosition.Y + (oil_temp_faceSpriteWidth + 5);
            oil_pres_FontPosition = new Vector2(oil_pres_faceSpritePosition.X + (oil_pres_faceSpriteWidth / 2) + 30, oil_pres_faceSpritePosition.Y + (oil_pres_faceSpriteWidth / 2) - 10);

            // oil press needle
            oil_pres_needleSprite = Content.Load<Texture2D>("bigneedle");
            oil_pres_needleSpritePosition = new Vector2(oil_pres_faceSpritePosition.X + (oil_pres_faceSpriteWidth / 2), oil_pres_faceSpritePosition.Y + (oil_pres_faceSpriteWidth / 2));

            // water temp
            water_temp_faceSprite = Content.Load<Texture2D>("water_temp_face");
            water_temp_faceSpritePosition.X = tach_faceSpritePosition.X - (water_temp_faceSpriteWidth);
            //water_temp_faceSpritePosition.Y = tach_faceSpritePosition.Y - (water_temp_faceSpriteWidth / 4);
            water_temp_faceSpritePosition.Y = tach_faceSpritePosition.Y;
            water_temp_FontPosition = new Vector2(water_temp_faceSpritePosition.X + (water_temp_faceSpriteWidth / 2) + 30, water_temp_faceSpritePosition.Y + (water_temp_faceSpriteWidth / 2) - 10);

            fuel_pres_faceSprite = Content.Load<Texture2D>("fuel_press_face");
            fuel_pres_faceSpritePosition.X = tach_faceSpritePosition.X - (fuel_pres_faceSpriteWidth);
            //fuel_pres_faceSpritePosition.Y = tach_faceSpritePosition.Y + (tac_faceSpriteX / 2);
            fuel_pres_faceSpritePosition.Y = tach_faceSpritePosition.Y + (oil_temp_faceSpriteWidth + 5);

            fuel_pres_FontPosition = new Vector2(fuel_pres_faceSpritePosition.X + (fuel_pres_faceSpriteWidth / 2) + 30, fuel_pres_faceSpritePosition.Y + (fuel_pres_faceSpriteWidth / 2) - 10);

            water_temp_needleSprite = Content.Load<Texture2D>("bigneedle");
            water_temp_needleSpritePosition = new Vector2(water_temp_faceSpritePosition.X + (water_temp_faceSpriteWidth / 2), water_temp_faceSpritePosition.Y + (water_temp_faceSpriteWidth / 2));

            fuel_pres_needleSprite = Content.Load<Texture2D>("bigneedle");
            fuel_pres_needleSpritePosition = new Vector2(fuel_pres_faceSpritePosition.X + (fuel_pres_faceSpriteWidth / 2), fuel_pres_faceSpritePosition.Y + (fuel_pres_faceSpriteWidth / 2));


            rpmFont = Content.Load<SpriteFont>("TachFaceFont");
            OilTempFont = Content.Load<SpriteFont>("SmallGaugeFont");
            OilPresFont = Content.Load<SpriteFont>("SmallGaugeFont");
            WaterTempFont = Content.Load<SpriteFont>("SmallGaugeFont");
            FuelPresFont = Content.Load<SpriteFont>("SmallGaugeFont");


            scale = new Vector2(tac_faceSpriteX / (float)tach_faceSprite.Width, tac_faceSpriteX / (float)tach_faceSprite.Width);
            tac_faceSpriteY = tach_faceSprite.Height * scale.Y;

            needleScale = new Vector2(tach_needleSpriteX / (float)tach_needleSprite.Width, tach_needleSpriteX / (float)tach_needleSprite.Width);
            tach_needleSpriteY = tach_needleSprite.Height * needleScale.Y;

            oiltempScale = new Vector2(oil_temp_faceSpriteWidth / (float)oil_temp_faceSprite.Width, oil_temp_faceSpriteWidth / (float)oil_temp_faceSprite.Width);
            oilpresScale = new Vector2(oil_pres_faceSpriteWidth / (float)oil_pres_faceSprite.Width, oil_pres_faceSpriteWidth / (float)oil_pres_faceSprite.Width);
            oiltempneedleScale = new Vector2(oil_temp_needleSpriteWidth / (float)oil_temp_needleSprite.Width, oil_temp_needleSpriteWidth / (float)oil_temp_needleSprite.Width);
            oilpresneedleScale = new Vector2(oil_pres_needleSpriteWidth / (float)oil_pres_needleSprite.Width, oil_pres_needleSpriteWidth / (float)oil_pres_needleSprite.Width);
            mediumNeedleScale = oiltempneedleScale;
            mediumFaceScale = oiltempScale;


            // get origin of needle
            needleOrigin = new Vector2(tach_needleSprite.Width / 2, tach_needleSprite.Height / 2);
            oilTempNeedleOrigin = new Vector2(oil_temp_needleSprite.Width / 2, oil_temp_needleSprite.Height / 2);
            oilPresNeedleOrigin = new Vector2(oil_pres_needleSprite.Width / 2, oil_pres_needleSprite.Height / 2);
            fuelPresNeedleOrigin = new Vector2(oil_pres_needleSprite.Width / 2, oil_pres_needleSprite.Height / 2);
            waterTempNeedleOrigin = new Vector2(oil_pres_needleSprite.Width / 2, oil_pres_needleSprite.Height / 2);

            rpmVal = 0;
            oil_tempneedleRoatation = 0;
        }

        // the game loop
        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            // TODO: Add your update logic here 
            if (Keyboard.GetState().IsKeyDown(Keys.D9))
            {
                testSpin = true;
                testHold = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D0))
            {
                testSpin = false;
                testHold = true;
            }
            if (testSpin)
            {
                //rotationVal += 5;
                rpmVal += 123.5f;
                if (rpmVal >= 9000)
                    rpmVal = 8600;
                oilTempVal += 1f;
                if (oilTempVal >= 315)
                    oilTempVal = 0;
                oilPresVal += 0.25f;
                if (oilPresVal >= 105)
                    oilPresVal = 0;
                waterTempVal += 0.6f;
                if (waterTempVal >= 315)
                    waterTempVal = 0;
                fuelPresVal += 0.125f;
                if (fuelPresVal >= 105)
                    fuelPresVal = 0;
            }

            // hold RPM
            if (testHold)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.D1))
                {
                    oilPresWarning = false;
                    oilTempWarning = false;
                    rpmWarning = false;
                    waterTempWarning = false;
                    fuelPresWarning = false;
                    rpmVal = 1000;
                    oilTempVal = 160;
                    oilPresVal = 10;
                    fuelPresVal = 10;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.D2))
                {
                    rpmVal = 2000;
                    oilTempVal = 200;
                    oilPresVal = 20;
                    fuelPresVal = 20;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.D3))
                {
                    rpmVal = 3500;
                    oilTempVal = 250;
                    oilPresVal = 50;
                    fuelPresVal = 30;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.D4))
                {
                    rpmVal = 9000;
                    oilTempVal = 300;
                    oilPresVal = 0;
                    fuelPresVal = 40;
                    oilPresWarning = true;
                    oilTempWarning = true;
                    rpmWarning = true;
                    waterTempWarning = true;
                    fuelPresWarning = true;
                }
                else
                {
                    rpmVal = 0;
                    oilTempVal = 0;
                    fuelPresVal = 0;
                }
            }

            // get live data
            if (!testSpin && !testHold)
            {
                rpmVal = gaugeRPM;
                oilTempVal = gaugeOilTemp;
                oilPresVal = gaugeOilPres;
                waterTempVal = gaugewWaterTemp;
                fuelPresVal = gaugeFuelPres;
            }

            float tachFormula = 1.9f + ((rpmVal / 1000) / 2) - 0.04f;
            tach_needleRotation = tachFormula;

            float oilFormula = (oilTempVal / 42.55f) + 1.6f;
            if (oilTempVal <= 100)
                oilFormula = 3.9f;
            oil_tempneedleRoatation = oilFormula;
            oilTempRotation = oilFormula.ToString();

            float waterTempFormula = (waterTempVal / 42.55f) + 1.6f;
            if (waterTempVal <= 100)
                waterTempFormula = 3.9f;
            water_temp_needleRotation = waterTempFormula;

            float oilPresFormula = 3.9f + (oilPresVal / 21.3f);
            oil_pres_needleRotation = oilPresFormula;

            float fuelPresFormula = 3.9f + (fuelPresVal / 21.3f);
            fuel_pres_needleRotation = fuelPresFormula;


            //TachValueText = rpmVal.ToString();
            TachValueText = Math.Ceiling(rpmVal).ToString();
            TachRotationText = tach_needleRotation.ToString();
            oilTempText = Math.Ceiling(oilTempVal).ToString();
            oilPresText = Math.Ceiling(oilPresVal).ToString();
            waterTempText = Math.Ceiling(waterTempVal).ToString();
            fuelPresText = Math.Ceiling(fuelPresVal).ToString();
            

            // set warnings
            rpmSpriteColor = SetSpriteWarningColors(rpmSpriteColor, rpmWarning);
            oilTempSpriteColor = SetSpriteWarningColors(oilTempSpriteColor, oilTempWarning);
            oilPresSpriteColor = SetSpriteWarningColors(oilPresSpriteColor, oilPresWarning);
            fuelPresSpritecolor = SetSpriteWarningColors(fuelPresSpritecolor, fuelPresWarning);
            

            // generate left (lap) text
            LapInfoText = string.Format("Last Lap: {0}\r\nBest Lap: {1}({2})",
                _raceInfo.LastLapTime,
                _raceInfo.Standings.fastestLap.time,
                _raceInfo.Standings.fastestLap.lap
                );


            // generate middle text
            MiddleInfoText = string.Format("Laps since pit: {0}\r\nLaps Left: {1}",
                _raceInfo.GetLapsSinceLastPit(),
                _raceInfo.SessionInfo.lapsRemaining
                );

            // generate right text
            RightInfoText = string.Format("Average Lap Time: {0}\r\n",
                _raceInfo.Standings.averageLapTime
                );


            // generate standinsg
            StandingsText = "Standings:ID time lap lapslead incidents\r\n";
            if (_raceInfo.Standings.position != null && !drawTopTextBlock)
            {
                foreach (var rec in _raceInfo.Standings.position)
                {
                    if (rec.carIdx != -1)
                        StandingsText += string.Format("{0}  {1}  {2}  {3}  {4}\r\n", rec.carIdx, rec.time, rec.lap, rec.lapsLead, rec.incidents);
                }
            }


            base.Update(gameTime);
        }


        // Set the sprite color if there is a warning or not
        private Color SetSpriteWarningColors(Color SpriteColor, bool Warning)
        {            
            if (Warning)
            {
                // Flash
                if (SpriteColor == Color.White)
                {
                    SpriteColor = Color.Red;
                }
                else
                {
                    SpriteColor = Color.White;
                }

            }
            else
            {
                SpriteColor = Color.White;
            }

            return SpriteColor;
        }

        // anything that draws on the screen goes here
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            // Background
            _spriteBatch.Draw(backgroundSprite, new Vector2(0, 0), Color.White);

            // Lap Info Block
            if (drawTopTextBlock)
            {
                _spriteBatch.DrawString(gameFont, LapInfoText, lapInfo_FontPosition, Color.White);
                _spriteBatch.DrawString(MiddleInfoFont, MiddleInfoText, middleInfo_FontPosition, Color.White);
                _spriteBatch.DrawString(RightInfoFont, RightInfoText, rightInfo_FontPosition, Color.White);
            } else
            {
                _spriteBatch.DrawString(StandingsFont, StandingsText, standings_FontPosition, Color.White);
            }


            

            // Tach
            _spriteBatch.Draw(tach_faceSprite, position: tach_faceSpritePosition, sourceRectangle: null, rpmSpriteColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            _spriteBatch.Draw(tach_needleSprite, tach_needleSpritePosition, null, rpmNeedleSpriteColor, tach_needleRotation, needleOrigin, needleScale, SpriteEffects.None, 0);
            _spriteBatch.DrawString(rpmFont, string.Format("{0}", TachValueText), tach_rpmFontPosition, Color.Black);

            // Oil Temp
            _spriteBatch.Draw(oil_temp_faceSprite, position: oil_temp_faceSpritePosition, sourceRectangle: null, oilTempSpriteColor, 0, Vector2.Zero, oiltempScale, SpriteEffects.None, 0);
            _spriteBatch.Draw(oil_temp_needleSprite, oil_temp_needleSpritePosition, null, oilTempNeedleSpriteColor, oil_tempneedleRoatation, oilTempNeedleOrigin, oiltempneedleScale, SpriteEffects.None, 0);
            _spriteBatch.DrawString(OilTempFont, string.Format("{0}", oilTempText), oil_temp_FontPosition, Color.Black);

            // Oil Pressure
            _spriteBatch.Draw(oil_pres_faceSprite, position: oil_pres_faceSpritePosition, sourceRectangle: null, oilPresSpriteColor, 0, Vector2.Zero, oilpresScale, SpriteEffects.None, 0);
            _spriteBatch.Draw(oil_pres_needleSprite, oil_pres_needleSpritePosition, null, oilPresNeedleSpriteColor, oil_pres_needleRotation, oilPresNeedleOrigin, oilpresneedleScale, SpriteEffects.None, 0);
            _spriteBatch.DrawString(OilPresFont, string.Format("{0}", oilPresText), oil_pres_FontPosition, Color.Black);

            // Water Temp
            _spriteBatch.Draw(water_temp_faceSprite, water_temp_faceSpritePosition, sourceRectangle: null, waterTempSpritecolor, 0, Vector2.Zero, mediumFaceScale, SpriteEffects.None, 0);
            _spriteBatch.Draw(water_temp_needleSprite, water_temp_needleSpritePosition, null, waterTempNeedleSpritecolor, water_temp_needleRotation, waterTempNeedleOrigin, oilpresneedleScale, SpriteEffects.None, 0);
            _spriteBatch.DrawString(WaterTempFont, string.Format("{0}", waterTempText), water_temp_FontPosition, Color.Black);

            // Fuel Pressure
            _spriteBatch.Draw(fuel_pres_faceSprite, fuel_pres_faceSpritePosition, sourceRectangle: null, Color.White, 0, Vector2.Zero, mediumFaceScale, SpriteEffects.None, 0);
            _spriteBatch.Draw(fuel_pres_needleSprite, fuel_pres_needleSpritePosition, null, fuelPresNeedleSpritecolor, fuel_pres_needleRotation, fuelPresNeedleOrigin, oilpresneedleScale, SpriteEffects.None, 0);
            _spriteBatch.DrawString(FuelPresFont, string.Format("{0}", fuelPresText), fuel_pres_FontPosition, Color.Black);

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        // util function

        // This function tells the DLL to get new data from the engine
        private void GetGagueInfoBG()
        {
            bool loaded = NR2003Binding.Setup();
            if (!loaded)
                return;
            try
            {
                var running = NR2003Binding.WaitForSimToRun();
                while (true)
                {
                    NR2003Binding.RequestData();
                    //Thread.Sleep(27);
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        // This function pulls telemetry values from memory that have been updated by the GetGagueInfoBG call
        private void GetGameInfo()
        {
            bool loaded = NR2003Binding.Setup();
            if (!loaded)
                return;
            try
            {
                // wait until we have a running sim
                var running = NR2003Binding.WaitForSimToRun();

                // make sure we are running first before continuing
                if (running)
                {
                    LapCrossing lapCache = new LapCrossing()
                    {
                        carIdx = new byte[] { 0, 0, 0, 0 },
                        lapNum = new byte[] { 0, 0, 0, 0 },
                        flags = new byte[] { 0, 0, 0, 0 },
                        crossedAt = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 },
                    };
                    TimeSpan lap = new TimeSpan(0, 0, 0, 0, 0);

                    // main loop to pull data out of memory
                    while (true)
                    {
                        // Get Gauge Data
                        IntPtr gd = NR2003Binding.GetGaugeData();
                        if (gd != IntPtr.Zero)
                        {
                            GaugeData gauge = (GaugeData)Marshal.PtrToStructure(gd, typeof(GaugeData));

                            gaugeRPM = gauge.rpm;
                            gaugeOilTemp = UtilFunctions.CelsiusToFarenheit(gauge.oilTemp);
                            gaugeOilPres = UtilFunctions.KPAToPSI(gauge.oilPress);
                            gaugewWaterTemp = UtilFunctions.CelsiusToFarenheit(gauge.waterTemp);
                            gaugeFuelPres = UtilFunctions.KPAToPSI(gauge.fuelPress);
                            gaugeVoltage = gauge.voltage;
                            //gauge.fuelPress

                            rpmWarning = Misc.GetBit(gauge.warnings, 0);
                            waterTempWarning = Misc.GetBit(gauge.warnings, 1);
                            oilPresWarning = Misc.GetBit(gauge.warnings, 2);
                            fuelPresWarning = Misc.GetBit(gauge.warnings, 3);
                        }

                        // Get LapCrossing Data
                        IntPtr lc = NR2003Binding.GetLapCrossing();
                        {
                            if (lc != IntPtr.Zero)
                            {
                                LapCrossing _lap = (LapCrossing)Marshal.PtrToStructure(lc, typeof(LapCrossing));

                                if (_lap != lapCache)
                                {
                                    double crossed = BitConverter.ToDouble(_lap.crossedAt, 0);
                                    int seconds = Convert.ToInt32(Math.Truncate(crossed));
                                    int milliseconds = Convert.ToInt32((crossed - seconds) * 1000);
                                    TimeSpan crossedAt = new TimeSpan(0, 0, 0, seconds, milliseconds);
                                    int carIdx = BitConverter.ToInt32(_lap.carIdx, 0);
                                    //carIdx is player
                                    if (carIdx == 0 && crossedAt > lap)
                                    {
                                        LapTime = (float)((crossedAt - lap).TotalSeconds);
                                        _raceInfo.LastLapTime = LapTime;
                                        LastLapTimeText = string.Format("{0:0.000}", (crossedAt - lap).TotalSeconds);
                                        lap = crossedAt;
                                        lapCache = _lap;
                                    }
                                    else if (carIdx == 0)
                                    {
                                        LapTime = (float)((crossedAt - lap).TotalSeconds);
                                        _raceInfo.LastLapTime = LapTime;
                                        LastLapTimeText = string.Format("{0:0.000}", (crossedAt - lap).TotalSeconds);
                                        lap = crossedAt;
                                        lapCache = _lap;
                                    }
                                }
                            }
                        }

                        // Get Standings info
                        IntPtr se = NR2003Binding.GetStandings();
                        if (se != IntPtr.Zero)
                        {
                            Standings _standing = (Standings)Marshal.PtrToStructure(se, typeof(Standings));
                            _raceInfo.Standings = _standing;
                            _raceInfo.BestLapTime = _standing.fastestLap.time;
                            _raceInfo.BestLapNumber = _standing.fastestLap.lap;
                        }

                        // Get Session Info
                        IntPtr si = NR2003Binding.GetSessionInfo();
                        if (si != IntPtr.Zero)
                        {
                            SessionInfo _sessionInfo = (SessionInfo)Marshal.PtrToStructure(si, typeof(SessionInfo));
                            _raceInfo.SessionInfo = _sessionInfo;
                        }

                        IntPtr ps = NR2003Binding.GetPitStop();
                        if (ps != IntPtr.Zero)
                        {
                            PitStop _pitstop = (PitStop)Marshal.PtrToStructure(ps, typeof(PitStop));
                            _raceInfo.PitLaps.Add(_raceInfo.CurrentLap);
                        }

                        IntPtr od = NR2003Binding.GetOpponentCarData();
                        if (od != IntPtr.Zero)
                        {
                            _raceInfo.OpponentCarDatas = UtilFunctions.GetStructArrayFromIntPtr<OpponentCarData>(od, 48).ToList();
                        }

                        // driver entry
                        // this needs work...
                        /*
                        IntPtr de = NR2003Binding.GetDriverEntry();
                        if (de != IntPtr.Zero)
                        {
                            var DriverEntries = UtilFunctions.GetStructArrayFromIntPtr<DriverEntry>(de, 48).ToList();
                        }
                        */

                        IntPtr di = NR2003Binding.GetDriverInput();
                        if (di != IntPtr.Zero)
                        {
                            _raceInfo.DriverInput = (DriverInput)Marshal.PtrToStructure(di, typeof(DriverInput));
                        }

                        //game only provides data at 36 hz
                        Thread.Sleep(25);
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}
