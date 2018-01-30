using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TUI
{
    class DestroyCircle
    {
        List<Tuple<int, int>> cells;
        Texture2D tex;
        int cellSize;
        int curSize = 0;

        public bool isWorking = false;
        double state;

        public DestroyCircle() { }

        public void Init(List<Tuple<int, int>> cells, Texture2D tex, int cellSize)
        {
            this.cells = cells;
            this.tex = tex;
            this.cellSize = cellSize;
            isWorking = true;
            state = 0;
            curSize = cellSize - 10;
        }

        public void Update(GameTime gameTime)
        {
            if (!isWorking) return;
            state += gameTime.ElapsedGameTime.Milliseconds / 200.0;

            if (state >= 1)
            {
                isWorking = false;
                return;
            }

            curSize = (int)((cellSize - 10) * (1 - state));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isWorking) return;
            for (int i = 0; i < cells.Count; i++)
            {
                int x = cells[i].Item1 * cellSize;
                int y = cells[i].Item2 * cellSize + 160;

                Rectangle rect = new Rectangle(x + (cellSize - curSize) / 2, y + (cellSize - curSize) / 2, curSize, curSize);

                spriteBatch.Draw(tex, rect, Color.White);
            }
        }
    }
}
