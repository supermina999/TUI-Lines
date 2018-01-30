using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TUI
{
    class SelectedCircle
    {
        Texture2D tex;
        int cellSize;
        int x, y;
        Rectangle rect;


        public bool isWorking = false;
        double state;

        public SelectedCircle() { }

        public void Init(int x, int y, Texture2D tex, int cellSize)
        {
            this.x = x;
            this.y = y;
            this.tex = tex;
            this.cellSize = cellSize;
            isWorking = true;
            state = 0;
            rect = new Rectangle(x * cellSize + 5, y * cellSize + 165, cellSize - 10, cellSize - 10);
        }

        public void Update(GameTime gameTime)
        {
            if (!isWorking) return;
            state += gameTime.ElapsedGameTime.Milliseconds / 200.0;
            if (state >= 4) state -= 4;
            if (state <= 1)
            {
                rect = new Rectangle(x * cellSize + 5, y * cellSize + 165, cellSize - 10, (int)(cellSize - 10 - state * 0.2 * cellSize));
            }
            else if (state <= 2)
            {
                rect = new Rectangle(x * cellSize + 5, y * cellSize + 165, cellSize - 10, (int)(cellSize - 10 - (2 - state) * 0.2 * cellSize));
            }
            else if (state <= 3)
            {
                rect = new Rectangle(x * cellSize + 5, (int)(y * cellSize + 165 + (state - 2) * 0.2 * cellSize),
                    cellSize - 10, (int)(cellSize - 10 - (state - 2) * 0.2 * cellSize));
            }
            else if (state <= 4)
            {
                rect = new Rectangle(x * cellSize + 5, (int)(y * cellSize + 165 + (4 - state) * 0.2 * cellSize),
                    cellSize - 10, (int)(cellSize - 10 - (4 - state) * 0.2 * cellSize));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isWorking) return;
            spriteBatch.Draw(tex, rect, Color.White);
        }
    }
}
