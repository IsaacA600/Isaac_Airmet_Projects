using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BigBlueIsYou
{
  public class AnimatedSprite
  {
    private Texture2D m_spriteSheet;
    public Rectangle spriteRect;
    private Color color;
    private int[] m_spriteTime;

    private TimeSpan m_animationTime;
    private int m_subImageIndex;
    private int m_subImageWidth;
    private float rotation = 0.0f;

    public AnimatedSprite(Texture2D spriteSheet, int[] spriteTime, Rectangle initalRect, Color color)
    {
      this.m_spriteSheet = spriteSheet;
      this.m_spriteTime = spriteTime;
      this.spriteRect = initalRect;
      this.color = color;

      m_subImageWidth = spriteSheet.Width / spriteTime.Length;
    }

    public void update(GameTime gameTime, Point position)
    {
      m_animationTime += gameTime.ElapsedGameTime;
      if (m_animationTime.TotalMilliseconds >= m_spriteTime[m_subImageIndex])
      {
        m_animationTime -= TimeSpan.FromMilliseconds(m_spriteTime[m_subImageIndex]);
        m_subImageIndex++;
        m_subImageIndex = m_subImageIndex % m_spriteTime.Length;
      }
      UtilSingleton utilSingleton = UtilSingleton.getUtilSingleton();
      spriteRect = new Rectangle(position.Y * spriteRect.Width + utilSingleton.gridOffsetX, position.X * spriteRect.Height + utilSingleton.gridOffsetY, spriteRect.Width, spriteRect.Height);
    }

    public void draw(SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(
          m_spriteSheet,
          new Rectangle(spriteRect.Center.X - m_subImageWidth / 2, spriteRect.Center.Y - m_spriteSheet.Height / 2, spriteRect.Size.X, spriteRect.Size.Y), // Destination rectangle
          new Rectangle(m_subImageIndex * m_subImageWidth, 0, m_subImageWidth, m_spriteSheet.Height), // Source sub-texture
          color,
          rotation, // Angular Rotation
          new Vector2(m_subImageWidth / 2, m_spriteSheet.Height / 2), // Center point of Rotation
          SpriteEffects.None, 
          0
      );
    }
  }
}
