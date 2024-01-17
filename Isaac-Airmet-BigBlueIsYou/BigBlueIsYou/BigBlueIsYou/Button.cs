using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BigBlueIsYou
{
  public class MenuButtonObject
  {
    public Guid ButtonId { get; }

    public bool IsHighlighted;

    public string Label;
    public string ActiveLabel;

    public Color LabelColor;

    public Color HighlightColor;

    public SpriteFont LabelFont;

    public Rectangle ObjectRectangle;

    public Color RectangleFillColor;

    public Texture2D ObjectTexture;

    public MenuButtonObject()
    {
      ButtonId = Guid.NewGuid();
    }

    public MenuButtonObject(string label, Color labelColor, Color highlightColor, Color buttonFillColor) : this()
    {
      Label = label;
      ActiveLabel = label;
      LabelColor = labelColor;
      HighlightColor = highlightColor;
      RectangleFillColor = buttonFillColor;
    }

    public void RenderObject(SpriteBatch spriteBatch)
    {
      Color textColor = IsHighlighted ? HighlightColor : LabelColor;
      float buttonOpacity = IsHighlighted ? 1.0f : 0.75f;
      Vector2 textSize = LabelFont.MeasureString(ActiveLabel);
      spriteBatch.Begin();
      spriteBatch.Draw(ObjectTexture, ObjectRectangle, RectangleFillColor * buttonOpacity);
      spriteBatch.DrawString(LabelFont, ActiveLabel, new Vector2(ObjectRectangle.Center.X - textSize.X / 2, ObjectRectangle.Center.Y - textSize.Y / 2), textColor);
      spriteBatch.End();
    }

    public void loadContent(SpriteFont font, Rectangle rect, Texture2D texture)
    {
      LabelFont = font;
      ObjectRectangle = rect;
      ObjectTexture = texture;
    }

    public void loadContent(SpriteFont font, Texture2D texture)
    {
      LabelFont = font;
      ObjectTexture = texture;
    }

    public void setContainer(Rectangle rect)
    {
      ObjectRectangle = rect;
    }

    public void updateActiveLabel(string label, Color color)
    {
      ActiveLabel = label;
      LabelColor = color;
    }
  }
}
