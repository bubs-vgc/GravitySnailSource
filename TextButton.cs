using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GravitySnail
{
    /// <summary>
    /// child of button class
    /// allows a string to be drawn on top of the button texture
    /// </summary>
    internal class TextButton : Button
    {
        protected string text;
        protected SpriteFont spriteFont;
        protected Color textColor;
        protected Color textHoverColor;
        protected Vector2 pos;

        /// <summary>
        /// constructor that defaults to black as the text color
        /// </summary>
        /// <param name="position"></param>
        /// <param name="texture"></param>
        /// <param name="hover"></param>
        /// <param name="buttonText"></param>
        /// <param name="font"></param>
        public TextButton(Vector2 position, Texture2D texture, Texture2D hover, string buttonText, SpriteFont font) : base(position, texture, hover)
        {
            text = buttonText;
            spriteFont = font;
            textColor = Color.Black;
            textHoverColor = Color.Black;
            pos = new Vector2(position.X + (texture.Width - font.MeasureString(buttonText).X) / 2, position.Y + (texture.Height - font.MeasureString(buttonText).Y) / 2);
        }

        public TextButton(Vector2 position, Texture2D texture, Texture2D hover, string buttonText, SpriteFont font, Color color, Color colorHover) : base(position, texture, hover)
        {
            text = buttonText;
            spriteFont = font;
            textColor = color;
            textHoverColor = colorHover;
            pos = new Vector2(position.X + (texture.Width - font.MeasureString(buttonText).X)/2, position.Y + (texture.Height - font.MeasureString(buttonText).Y)/2);
        }

        public TextButton(Vector2 position, Texture2D texture, Texture2D hover, string buttonText, SpriteFont font, int type) : base(position, texture, hover, type)
        {
            text = buttonText;
            spriteFont = font;
            textColor = Color.Black;
            textHoverColor = Color.Black;
            pos = new Vector2(position.X + (texture.Width - font.MeasureString(buttonText).X) / 2, position.Y + (texture.Height - font.MeasureString(buttonText).Y) / 2);
        }

        public TextButton(Vector2 position, Texture2D texture, Texture2D hover, string buttonText, SpriteFont font, Color color, Color colorHover, int type) : base(position, texture, hover, type)
        {
            text = buttonText;
            spriteFont = font;
            textColor = color;
            textHoverColor = colorHover;
            pos = new Vector2(position.X + (texture.Width - font.MeasureString(buttonText).X) / 2, position.Y + (texture.Height - font.MeasureString(buttonText).Y) / 2);
        }

        public override void Draw(SpriteBatch sb, Color color)
        {
            base.Draw(sb, color);
            if(MouseOver())
            {
                sb.DrawString(spriteFont, text, new Vector2(pos.X - Reference.transformationMatrix.M41, pos.Y - Reference.transformationMatrix.M42), textHoverColor);
            }
            else
            {
                sb.DrawString(spriteFont, text, new Vector2(pos.X - Reference.transformationMatrix.M41, pos.Y - Reference.transformationMatrix.M42), textColor);
            }
        }
    }
}
