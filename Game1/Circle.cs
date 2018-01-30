using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TUI
{
    class Circle
    {
        List<Tuple<int, int>> path;
        Texture2D tex;
        int circle;
        Game1 game1;
        int cellSize;

        public bool isWorking = false;
        double state;
        Rectangle rect;

        public Circle() { }

        public void Init(List<Tuple<int, int>> path, Texture2D tex, int circle, int cellSize, Game1 game1)
        {
            this.path = path;
            this.tex = tex;
            this.circle = circle;
            this.game1 = game1;
            this.cellSize = cellSize;
            isWorking = true;
            state = 0;
            rect = new Rectangle(path[0].Item1 * cellSize + 5, path[0].Item2 * cellSize + 165, cellSize - 10, cellSize - 10);
        }

        public void Update(GameTime gameTime)
        {
            if (!isWorking) return;
            state += gameTime.ElapsedGameTime.Milliseconds / 50.0;

            if (state >= path.Count - 1)
            {
                isWorking = false;
                game1.circles[path[path.Count - 1].Item1, path[path.Count - 1].Item2] = circle;
                game1.moveFinished();
                return;
            }

            int istate = (int)state;
            double dstate = state - istate;

            double x = (1 - dstate) * path[istate].Item1 + (dstate) * path[istate + 1].Item1;
            double y = (1 - dstate) * path[istate].Item2 + (dstate) * path[istate + 1].Item2;

            rect = new Rectangle((int)(x * cellSize + 5), (int)(y * cellSize + 165), cellSize - 10, cellSize - 10);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isWorking) return;
            spriteBatch.Draw(tex, rect, Color.White);
        }
    }
}
