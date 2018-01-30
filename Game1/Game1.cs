using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace TUI
{
    public class Game1 : Game
    {

        class MyButton
        {
            Texture2D tex;
            Rectangle rec;

            public MyButton(Texture2D tex, Rectangle rec)
            {
                this.tex = tex;
                this.rec = rec;
            }
            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(tex, rec, Color.White);
            }
            public bool inRec(int x, int y)
            {
                return (rec.X < x) && x < (rec.X + rec.Width) && (rec.Y < y) && y < (rec.Y + rec.Height);
            }
        }

        public int gameState;

        Texture2D[] mButtons;
        Texture2D[] sButtons;
        Texture2D[] rButtons;
        Texture2D text1, text2;

        MyButton[] menuButtons;
        MyButton[] settingsButtons;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int fieldSize = 9;
        int circleTypeCount = 7;
        Texture2D background;
        Texture2D[] circleTextures;
        Texture2D[] numbers;
        Texture2D dots;
        int cellSize;
        int width = 486;
        int height = 646;
        public int circlesPerMove = 3;

        public int[,] circles;
        bool prevButtonPressed = false;
        bool selected = false;
        int selectedX, selectedY;
        int score = 0;
        int maxScore = 0;
        int ellapsedMilliseconds = 0;
        bool[,] isBlock;

        bool isDemonstration = false;
        bool prevButtonPres = false;
        int destroyMode = 0;

        double demonstrationPause = 0;

        Random rand = new Random();

        bool[,] canGo;
        Tuple<int, int>[,] prevGo;

        Circle circle = new Circle();
        DestroyCircle destroyCircle = new DestroyCircle();
        SelectedCircle selectedCircle = new SelectedCircle();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            cellSize = 486 / fieldSize;
            numbers = new Texture2D[10];
            circleTextures = new Texture2D[circleTypeCount];
            circles = new int[fieldSize, fieldSize];
            canGo = new bool[fieldSize, fieldSize];
            isBlock = new bool[fieldSize, fieldSize];
            prevGo = new Tuple<int, int>[fieldSize, fieldSize];
            for (int i = 0; i < fieldSize; i++) for (int j = 0; j < fieldSize; j++) circles[i, j] = -1;

            mButtons = new Texture2D[4];
            mButtons[0] = Content.Load<Texture2D>("play");
            mButtons[1] = Content.Load<Texture2D>("demonstration");
            mButtons[2] = Content.Load<Texture2D>("settings");
            mButtons[3] = Content.Load<Texture2D>("exit");

            sButtons = new Texture2D[7];
            sButtons[0] = Content.Load<Texture2D>("lines");
            sButtons[1] = Content.Load<Texture2D>("squares");
            sButtons[2] = Content.Load<Texture2D>("blocks");
            sButtons[3] = Content.Load<Texture2D>("back");

            rButtons = new Texture2D[3];
            rButtons[0] = Content.Load<Texture2D>("button3");
            rButtons[1] = Content.Load<Texture2D>("button4");
            rButtons[2] = Content.Load<Texture2D>("button5");

            text1 = Content.Load<Texture2D>("balls");
            text2 = Content.Load<Texture2D>("regime");

            menuButtons = new MyButton[4];

            int d = height / 25;
            int w = (int)(width * 0.8);
            int x = (int)(width * 0.1);

            for (int i = 0; i < 4; i++)
            {
                menuButtons[i] = new MyButton(mButtons[i], new Rectangle(x, d + i * d * 6, w, d * 5));
            }

            settingsButtons = new MyButton[7];

            d = height / 36;
            int a = width / 7;
            int x1 = (int)(width * 0.1);
            int dl = (int)(width * 0.8);

            for (int i = 0; i < 3; i++)
            {
                settingsButtons[i] = new MyButton(rButtons[i], new Rectangle(a + i * 2 * a, 4 * d, 3 * d, 3 * d));
            }
            for (int i = 0; i < 3; i++)
            {
                settingsButtons[3 + i] = new MyButton(sButtons[i], new Rectangle(x1, 11 * d + 6 * i * d, dl, 5 * d));
            }
            settingsButtons[6] = new MyButton(sButtons[3], new Rectangle(x1, 31 * d, dl, 5 * d));

            gameState = 1;


            moveFinished();
            moveFinished();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background = Content.Load<Texture2D>("background");
            for (int i = 0; i < circleTypeCount; i++)
            {
                circleTextures[i] = Content.Load<Texture2D>("circle" + i);
            }

            for (int i = 0; i < 10; i++)
            {
                numbers[i] = Content.Load<Texture2D>("" + i);
            }

            dots = Content.Load<Texture2D>("dots");
        }

        protected override void UnloadContent()
        {
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var mouseState = Mouse.GetState();
            bool buttonPres = mouseState.LeftButton == ButtonState.Pressed;

            if (gameState == 1)
            {
                int d = height / 25;
                if (buttonPres && !prevButtonPres)
                {
                    int x = mouseState.X;
                    int y = mouseState.Y;
                    for (int i = 0; i < 4; i++)
                    {
                        if (menuButtons[i].inRec(x, y))
                        {
                            if (i == 0)
                            {
                                gameState = 3;
                                isDemonstration = false;
                                NewGame();
                            }
                            if (i == 1)
                            {
                                gameState = 3;
                                isDemonstration = true;
                                NewGame();
                            }
                            if (i == 2)
                            {
                                gameState = 2;
                            }
                            if (i == 3)
                            {
                                Exit();
                            }
                        }
                    }
                }
            }
            else if (gameState == 2)
            {
                int d = height / 25;
                if (buttonPres && !prevButtonPres)
                {
                    int x = mouseState.X;
                    int y = mouseState.Y;
                    for (int i = 0; i < 7; i++)
                    {
                        if (settingsButtons[i].inRec(x, y))
                        {
                            if (i == 0)
                            {
                                gameState = 1;
                                circlesPerMove = 3;
                            }
                            if (i == 1)
                            {
                                gameState = 1;
                                circlesPerMove = 4;
                            }
                            if (i == 2)
                            {
                                gameState = 1;
                                circlesPerMove = 5;
                            }
                            if (i == 3)
                            {
                                gameState = 1;
                                destroyMode = 0;
                            }
                            if (i == 4)
                            {
                                gameState = 1;
                                destroyMode = 1;
                            }
                            if (i == 5)
                            {
                                gameState = 1;
                                destroyMode = 2;
                            }
                            if (i == 6)
                            {
                                gameState = 1;
                            }
                        }
                    }
                }
            }
            else if (gameState == 3) Update2(gameTime);

            prevButtonPres = buttonPres;

            base.Update(gameTime);
        }
        protected void Update2(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var mouseState = Mouse.GetState();
            bool buttonPressed = mouseState.LeftButton == ButtonState.Pressed;

            ellapsedMilliseconds += gameTime.ElapsedGameTime.Milliseconds;

            circle.Update(gameTime);
            destroyCircle.Update(gameTime);
            selectedCircle.Update(gameTime);

            if (buttonPressed && !prevButtonPressed && !circle.isWorking && !destroyCircle.isWorking && !isDemonstration)
            {
                int x = mouseState.X;
                int y = mouseState.Y;
                int nx = x / cellSize;
                int ny = (y - 160) / cellSize;
                if (nx < 0 || ny < 0 || nx >= fieldSize || ny >= fieldSize) return;
                if (!selected)
                {
                    if (circles[nx, ny] == -1 || circles[nx, ny] >= circleTypeCount) return;
                    selected = true;
                    selectedX = nx;
                    selectedY = ny;
                    calcCanGo(selectedX, selectedY);
                    selectedCircle.Init(nx, ny, circleTextures[circles[nx, ny]], cellSize);
                }
                else
                {
                    if (circles[nx, ny] != -1 && circles[nx, ny] < circleTypeCount)
                    {
                        selectedX = nx;
                        selectedY = ny;
                        calcCanGo(nx, ny);
                        selectedCircle.Init(nx, ny, circleTextures[circles[nx, ny]], cellSize);
                        return;
                    }

                    if (!canGo[nx, ny]) return;

                    var path = getPath(nx, ny);

                    circle.Init(path, circleTextures[circles[selectedX, selectedY]], circles[selectedX, selectedY], cellSize, this);
                    selectedCircle.isWorking = false;

                    circles[selectedX, selectedY] = -1;
                    selected = false;
                }
            }

            if (isDemonstration && !circle.isWorking && !destroyCircle.isWorking)
            {
                demonstrationPause += gameTime.ElapsedGameTime.Milliseconds / 1000.0;
                if (demonstrationPause < 1) return;
                demonstrationPause = 0;
                int bestx1 = 0, besty1 = 0, bestx2 = 0, besty2 = 0, bestv = -1000;
                for (int x1 = 0; x1 < fieldSize; x1++)
                {
                    for (int y1 = 0; y1 < fieldSize; y1++)
                    {
                        if (circles[x1, y1] == -1 || circles[x1, y1] >= circleTypeCount) continue;
                        calcCanGo(x1, y1);
                        for (int x2 = 0; x2 < fieldSize; x2++)
                        {
                            for (int y2 = 0; y2 < fieldSize; y2++)
                            {
                                if (!canGo[x2, y2] || (x1 == x2 && y1 == y2)) continue;

                                int pBest = getEffectivity(x1, y1, 1, 0);
                                pBest = Math.Max(getEffectivity(x1, y1, 0, 1), pBest);
                                pBest = Math.Max(getEffectivity(x1, y1, 1, 1), pBest);
                                pBest = Math.Max(getEffectivity(x1, y1, 1, -1), pBest);

                                int curCircle = circles[x1, y1];
                                int nCircle = circles[x2, y2];
                                circles[x1, y1] = -1;
                                circles[x2, y2] = curCircle;
                                int curBest = getEffectivity(x2, y2, 1, 0);
                                curBest = Math.Max(getEffectivity(x2, y2, 0, 1), curBest);
                                curBest = Math.Max(getEffectivity(x2, y2, 1, 1), curBest);
                                curBest = Math.Max(getEffectivity(x2, y2, 1, -1), curBest);
                                if (curBest - pBest > bestv)
                                {
                                    bestv = curBest - pBest;
                                    bestx1 = x1;
                                    bestx2 = x2;
                                    besty1 = y1;
                                    besty2 = y2;
                                }

                                circles[x1, y1] = curCircle;
                                circles[x2, y2] = nCircle;
                            }
                        }
                    }
                }

                selectedX = bestx1;
                selectedY = besty1;
                calcCanGo(bestx1, besty1);
                circle.Init(getPath(bestx2, besty2), circleTextures[circles[bestx1, besty1]], circles[bestx1, besty1], cellSize, this);
                circles[bestx1, besty1] = -1;
            }


            prevButtonPressed = buttonPressed;
        }

        List<Tuple<int, int>> getPath(int x, int y)
        {
            var path = new List<Tuple<int, int>>();
            int curX = x, curY = y;
            while (curX != selectedX || curY != selectedY)
            {
                path.Add(new Tuple<int, int>(curX, curY));
                int ncurX = prevGo[curX, curY].Item1;
                int ncurY = prevGo[curX, curY].Item2;
                curX = ncurX;
                curY = ncurY;
            }
            path.Add(new Tuple<int, int>(selectedX, selectedY));

            for (int i = 0; i < path.Count / 2; i++)
            {
                var t = path[i];
                path[i] = path[path.Count - 1 - i];
                path[path.Count - 1 - i] = t;
            }

            return path;
        }

        Tuple<int, int> getNthFree(int n)
        {
            int k = 0;
            for (int i = 0; i < fieldSize; i++) for (int j = 0; j < fieldSize; j++)
                {
                    if (circles[i, j] != -1) continue;
                    if (k == n) return new Tuple<int, int>(i, j);
                    k++;
                    if (k == n) return new Tuple<int, int>(i, j);
                }
            return null;
        }

        int getFreeCount()
        {
            int n = 0;
            for (int i = 0; i < fieldSize; i++) for (int j = 0; j < fieldSize; j++) if (circles[i, j] == -1) n++;
            return n;
        }

        public void moveFinished()
        {
            for (int i = 0; i < fieldSize; i++) for (int j = 0; j < fieldSize; j++) if (circles[i, j] >= circleTypeCount)
                        circles[i, j] -= circleTypeCount;

            var cells = getDestroying();

            if ((destroyMode == 0 && cells.Count >= 5) || (destroyMode != 0 && cells.Count > 0))
            {
                if (destroyMode == 1 && cells.Count == 8)
                {
                    score += 6;
                }
                else
                {
                    score += cells.Count;
                }

                maxScore = Math.Max(maxScore, score);

                destroyCircle.Init(cells, circleTextures[circles[cells[0].Item1, cells[0].Item2]], cellSize);
                for (int i = 0; i < cells.Count; i++)
                {
                    var cell = cells[i];
                    circles[cell.Item1, cell.Item2] = -1;
                }
            }

            if (getFreeCount() < circlesPerMove)
            {
                gameState = 1;
                return;
            }
            for (int i = 0; i < circlesPerMove; i++)
            {
                int num = rand.Next(getFreeCount());
                var cell = getNthFree(num);
                circles[cell.Item1, cell.Item2] = rand.Next(circleTypeCount) + circleTypeCount;
            }
        }

        List<Tuple<int, int>> getLine(int i, int j, int dx, int dy)
        {
            int x, y;
            List<Tuple<int, int>> curCells;

            curCells = new List<Tuple<int, int>>();
            int curCircle = circles[i, j];
            if (curCircle == -1 || curCircle >= circleTypeCount) return curCells;
            x = i;
            y = j;
            while (x >= 0 && y >= 0 && x < fieldSize && y < fieldSize && circles[x, y] == curCircle)
            {
                curCells.Add(new Tuple<int, int>(x, y));
                x += dx;
                y += dy;
            }

            return curCells;
        }

        int getEffectivity(int x, int y, int dx, int dy)
        {
            int ans = 0;
            int curCircle = circles[x, y];
            int curX = x, curY = y;
            while (curX >= 0 && curX < fieldSize && curY >= 0 && curY < fieldSize)
            {
                int curAns = getLine(curX, curY, dx, dy).Count;
                if (curAns <= ans) return ans;
                ans = curAns;

                curX -= dx;
                curY -= dy;
            }
            return ans * ans * ans * ans * ans;
        }

        List<Tuple<int, int>> getDestroying()
        {
            var cells = new List<Tuple<int, int>>();

            if (destroyMode == 0)
            {
                List<Tuple<int, int>> curCells;

                for (int i = 0; i < fieldSize; i++)
                {
                    for (int j = 0; j < fieldSize; j++)
                    {
                        curCells = getLine(i, j, 1, 0);
                        if (curCells.Count > cells.Count)
                        {
                            cells = curCells;
                        }

                        curCells = getLine(i, j, 0, 1);
                        if (curCells.Count > cells.Count)
                        {
                            cells = curCells;
                        }

                        curCells = getLine(i, j, 1, 1);
                        if (curCells.Count > cells.Count)
                        {
                            cells = curCells;
                        }

                        curCells = getLine(i, j, 1, -1);
                        if (curCells.Count > cells.Count)
                        {
                            cells = curCells;
                        }
                    }
                }
            }
            else if (destroyMode == 1)
            {
                for (int i = 0; i < fieldSize; i++)
                {
                    for (int j = 0; j < fieldSize; j++)
                    {
                        if (i == fieldSize - 1) continue;
                        if (j == fieldSize - 1) continue;
                        int curCircles = circles[i, j];
                        if (curCircles == -1 || curCircles >= circleTypeCount) continue;
                        if (curCircles == circles[i + 1, j] && curCircles == circles[i, j + 1] && curCircles == circles[i + 1, j + 1])
                        {
                            cells.Add(new Tuple<int, int>(i, j));
                            cells.Add(new Tuple<int, int>(i + 1, j));
                            cells.Add(new Tuple<int, int>(i, j + 1));
                            cells.Add(new Tuple<int, int>(i + 1, j + 1));
                        }
                    }
                }
            }
            else if (destroyMode == 2)
            {
                for (int i = 0; i < fieldSize; i++)
                {
                    for (int j = 0; j < fieldSize; j++)
                    {
                        if (circles[i, j] == -1) continue;
                        var curCells = getBlock(i, j);
                        if (curCells.Count >= 7)
                        {
                            return curCells;
                        }
                    }
                }
            }

            return cells;
        }

        List<Tuple<int, int>> getBlock(int bx, int by)
        {
            for (int i = 0; i < fieldSize; i++) for (int j = 0; j < fieldSize; j++) isBlock[i, j] = false;
            isBlock[bx, by] = true;
            Queue<Tuple<int, int>> q = new Queue<Tuple<int, int>>();
            q.Enqueue(new Tuple<int, int>(bx, by));
            int curCircle = circles[bx, by];
            while (q.Count != 0)
            {
                var cur = q.Dequeue();
                int x = cur.Item1;
                int y = cur.Item2;
                int nx, ny;

                nx = x + 1;
                ny = y;
                if (nx >= 0 && ny >= 0 && nx < fieldSize && ny < fieldSize && !isBlock[nx, ny] && circles[nx, ny] == curCircle)
                {
                    isBlock[nx, ny] = true;
                    q.Enqueue(new Tuple<int, int>(nx, ny));
                }

                nx = x - 1;
                ny = y;
                if (nx >= 0 && ny >= 0 && nx < fieldSize && ny < fieldSize && !isBlock[nx, ny] && circles[nx, ny] == curCircle)
                {
                    isBlock[nx, ny] = true;
                    q.Enqueue(new Tuple<int, int>(nx, ny));
                }

                nx = x;
                ny = y + 1;
                if (nx >= 0 && ny >= 0 && nx < fieldSize && ny < fieldSize && !isBlock[nx, ny] && circles[nx, ny] == curCircle)
                {
                    isBlock[nx, ny] = true;
                    q.Enqueue(new Tuple<int, int>(nx, ny));
                }

                nx = x;
                ny = y - 1;
                if (nx >= 0 && ny >= 0 && nx < fieldSize && ny < fieldSize && !isBlock[nx, ny] && circles[nx, ny] == curCircle)
                {
                    isBlock[nx, ny] = true;
                    q.Enqueue(new Tuple<int, int>(nx, ny));
                }
            }

            var cells = new List<Tuple<int, int>>();
            for (int i = 0; i < fieldSize; i++)
            {
                for (int j = 0; j < fieldSize; j++)
                {
                    if (isBlock[i, j])
                    {
                        cells.Add(new Tuple<int, int>(i, j));
                    }
                }
            }
            return cells;
        }

        bool getCanGo(int x, int y)
        {
            if (x < 0 || y < 0 || x >= fieldSize || y >= fieldSize) return false;
            return (circles[x, y] == -1 || circles[x, y] >= circleTypeCount) && (!canGo[x, y]);
        }

        void calcCanGo(int bx, int by)
        {
            for (int i = 0; i < fieldSize; i++) for (int j = 0; j < fieldSize; j++) canGo[i, j] = false;
            canGo[bx, by] = true;
            Queue<Tuple<int, int>> q = new Queue<Tuple<int, int>>();
            q.Enqueue(new Tuple<int, int>(bx, by));
            while (q.Count != 0)
            {
                var cur = q.Dequeue();
                int x = cur.Item1;
                int y = cur.Item2;
                int nx, ny;

                nx = x + 1;
                ny = y;
                if (getCanGo(nx, ny))
                {
                    canGo[nx, ny] = true;
                    q.Enqueue(new Tuple<int, int>(nx, ny));
                    prevGo[nx, ny] = new Tuple<int, int>(x, y);
                }

                nx = x - 1;
                ny = y;
                if (getCanGo(nx, ny))
                {
                    canGo[nx, ny] = true;
                    q.Enqueue(new Tuple<int, int>(nx, ny));
                    prevGo[nx, ny] = new Tuple<int, int>(x, y);
                }

                nx = x;
                ny = y + 1;
                if (getCanGo(nx, ny))
                {
                    canGo[nx, ny] = true;
                    q.Enqueue(new Tuple<int, int>(nx, ny));
                    prevGo[nx, ny] = new Tuple<int, int>(x, y);
                }

                nx = x;
                ny = y - 1;
                if (getCanGo(nx, ny))
                {
                    canGo[nx, ny] = true;
                    q.Enqueue(new Tuple<int, int>(nx, ny));
                    prevGo[nx, ny] = new Tuple<int, int>(x, y);
                }
            }
        }

        List<Texture2D> get3SmallTextures()
        {
            List<Texture2D> ans = new List<Texture2D>();
            for (int i = 0; i < fieldSize; i++)
            {
                for (int j = 0; j < fieldSize; j++)
                {
                    if (circles[i, j] >= circleTypeCount)
                    {
                        ans.Add(circleTextures[circles[i, j] - circleTypeCount]);
                    }
                    if (ans.Count >= 3) return ans;
                }
            }
            return ans;
        }

        void DrawText(Rectangle rect, string s)
        {
            int width1 = rect.Width / s.Length;
            for (int i = 0; i < s.Length; i++)
            {
                Texture2D tex;
                if (s[i] >= '0' && s[i] <= '9')
                {
                    tex = numbers[s[i] - '0'];
                }
                else
                {
                    tex = dots;
                }
                spriteBatch.Draw(tex, new Rectangle(rect.X + width1 * i, rect.Y, width1, rect.Height), Color.White);
            }
        }

        void NewGame()
        {
            prevButtonPressed = false;
            selected = false;
            demonstrationPause = 0;
            circle = new Circle();
            destroyCircle = new DestroyCircle();
            selectedCircle = new SelectedCircle();
            score = 0;
            ellapsedMilliseconds = 0;

            for (int i = 0; i < fieldSize; i++) for (int j = 0; j < fieldSize; j++) circles[i, j] = -1;
            moveFinished();
            moveFinished();
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            spriteBatch.Begin();

            if (gameState == 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    menuButtons[i].Draw(spriteBatch);
                }
            }
            else if (gameState == 2)
            {
                int d = height / 36;
                int x1 = (int)(width * 0.1);
                int dl = (int)(width * 0.8);
                spriteBatch.Draw(text1, new Rectangle(x1, d, dl, 2 * d), Color.White);
                spriteBatch.Draw(text2, new Rectangle(x1, 8 * d, dl, 2 * d), Color.White);
                for (int i = 0; i < 7; i++)
                {
                    settingsButtons[i].Draw(spriteBatch);
                }
            }
            else if (gameState == 3) Draw2(gameTime);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        protected void Draw2(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);



            for (int i = 0; i < fieldSize; i++)
            {
                for (int j = 0; j < fieldSize; j++)
                {
                    spriteBatch.Draw(background, new Rectangle(i * cellSize, j * cellSize + 160, cellSize, cellSize), Color.White);
                    if (circles[i, j] != -1)
                    {
                        if (selected && selectedX == i && selectedY == j) continue;
                        if (circles[i, j] < circleTypeCount)
                        {
                            spriteBatch.Draw(circleTextures[circles[i, j]],
                                new Rectangle(i * cellSize + 5, j * cellSize + 160 + 5, cellSize - 10, cellSize - 10), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(circleTextures[circles[i, j] - circleTypeCount],
                                new Rectangle(i * cellSize + cellSize / 4 + 2, j * cellSize + 160 + cellSize / 4 + 2,
                                    cellSize / 2 - 4, cellSize / 2 - 4), Color.White);
                        }
                    }
                }
            }

            circle.Draw(spriteBatch);
            destroyCircle.Draw(spriteBatch);
            selectedCircle.Draw(spriteBatch);

            var upCircles = get3SmallTextures();
            int size = (int)(486 / 9 * 0.75);
            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(upCircles[i],
                    new Rectangle(width / 3 + (i + 1) * width / 9 - width / 18 - size / 2, 160 / 4 - size / 2, size, size), Color.White);
            }


            string scoreString = "" + score;
            while (scoreString.Length < 5) scoreString = "0" + scoreString;

            string maxScoreString = "" + maxScore;
            while (maxScoreString.Length < 5) maxScoreString = "0" + maxScoreString;

            string hoursString = "" + ellapsedMilliseconds / 3600000;
            string minutesString = "" + ellapsedMilliseconds % 3600000 / 60000;
            while (minutesString.Length < 2) minutesString = "0" + minutesString;
            string secondsString = "" + ellapsedMilliseconds % 60000 / 1000;
            while (secondsString.Length < 2) secondsString = "0" + secondsString;

            DrawText(new Rectangle(0, (160 - width * 2 / 15) / 2, width / 3, width * 2 / 15), maxScoreString);
            DrawText(new Rectangle(width * 2 / 3, (160 - width * 2 / 15) / 2, width / 3, width * 2 / 15), scoreString);
            DrawText(new Rectangle(width / 3, 80 + (80 - width * 2 / 21) / 2, width / 3, width * 2 / 21),
                hoursString + ":" + minutesString + ":" + secondsString);


        }
    }
}
