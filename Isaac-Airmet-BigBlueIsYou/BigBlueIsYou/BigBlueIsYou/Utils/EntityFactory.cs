using BigBlueIsYou.Components;
using BigBlueIsYou.Entities;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BigBlueIsYou
{
  public static class EntityFactory
  {

    public static IEntity CreateEntity(Type entityType, ContentManager contentManager, Rectangle initalLocation, Point initalPosition)
    {
      if (!typeof(IEntity).IsAssignableFrom(entityType))
      {
        throw new ArgumentException("CreateEntity only accepts Types that implement IEntity");
      }

      IEntity entity = null;
      int num = UtilSingleton.getUtilSingleton().entitiesCreated++;
      if (entityType == typeof(BigBlueEntity))
      {
        Texture2D bigBlueTexture = contentManager.Load<Texture2D>("Images/Sprites/baba");
        int[] spriteTime = {300, 300, 300};
        entity = new BigBlueEntity(num);
        entity.AddComponent(new SpriteComponent(new AnimatedSprite(bigBlueTexture, spriteTime, initalLocation, Color.White)));
        entity.AddComponent(new PositionComponent(initalPosition));
        entity.AddComponent(new NounComponent(NounType.BigBlue));
      }
      else if (entityType == typeof(FlagEntity))
      {
        Texture2D spriteSheet = contentManager.Load<Texture2D>("Images/Sprites/flag");
        int[] spriteTime = {300, 300, 300};
        entity = new FlagEntity(num);
        entity.AddComponent(new SpriteComponent(new AnimatedSprite(spriteSheet, spriteTime, initalLocation, Color.Yellow)));
        entity.AddComponent(new PositionComponent(initalPosition));
        entity.AddComponent(new NounComponent(NounType.Flag));
      }
      else if (entityType == typeof(GrassEntity))
      {
        Texture2D spriteSheet = contentManager.Load<Texture2D>("Images/Sprites/grass");
        int[] spriteTime = {300, 300, 300};
        entity = new GrassEntity(num);
        entity.AddComponent(new SpriteComponent(new AnimatedSprite(spriteSheet, spriteTime, initalLocation, Color.GreenYellow)));
        entity.AddComponent(new PositionComponent(initalPosition));
        entity.AddComponent(new NounComponent(NounType.Grass));
      }
      else if (entityType == typeof(HedgeEntity))
      {
        Texture2D spriteSheet = contentManager.Load<Texture2D>("Images/Sprites/hedge");
        int[] spriteTime = {300, 300, 300};
        entity = new HedgeEntity(num);
        entity.AddComponent(new SpriteComponent(new AnimatedSprite(spriteSheet, spriteTime, initalLocation, Color.DarkGreen)));
        entity.AddComponent(new PositionComponent(initalPosition));
        entity.AddComponent(new NounComponent(NounType.Hedge));
      }
      else if (entityType == typeof(LavaEntity))
      {
        Texture2D spriteSheet = contentManager.Load<Texture2D>("Images/Sprites/lava");
        int[] spriteTime = {300, 300, 300};
        entity = new LavaEntity(num);
        entity.AddComponent(new SpriteComponent(new AnimatedSprite(spriteSheet, spriteTime, initalLocation, Color.OrangeRed)));
        entity.AddComponent(new PositionComponent(initalPosition));
        entity.AddComponent(new NounComponent(NounType.Lava));
      }
      else if (entityType == typeof(RockEntity))
      {
        Texture2D spriteSheet = contentManager.Load<Texture2D>("Images/Sprites/rock");
        int[] spriteTime = {300, 300, 300};
        entity = new RockEntity(num);
        entity.AddComponent(new SpriteComponent(new AnimatedSprite(spriteSheet, spriteTime, initalLocation, Color.SandyBrown)));
        entity.AddComponent(new PositionComponent(initalPosition));
        entity.AddComponent(new NounComponent(NounType.Rock));
      }
      else if (entityType == typeof(WallEntity))
      {
        Texture2D spriteSheet = contentManager.Load<Texture2D>("Images/Sprites/wall");
        int[] spriteTime = {300, 300, 300};
        entity = new WallEntity(num);
        entity.AddComponent(new SpriteComponent(new AnimatedSprite(spriteSheet, spriteTime, initalLocation, Color.DarkGray)));
        entity.AddComponent(new PositionComponent(initalPosition));
        entity.AddComponent(new NounComponent(NounType.Wall));
      }
      else if (entityType == typeof(WaterEntity))
      {
        Texture2D spriteSheet = contentManager.Load<Texture2D>("Images/Sprites/water");
        int[] spriteTime = {300, 300, 300};
        entity = new WaterEntity(num);
        entity.AddComponent(new SpriteComponent(new AnimatedSprite(spriteSheet, spriteTime, initalLocation, Color.Aqua)));
        entity.AddComponent(new PositionComponent(initalPosition));
        entity.AddComponent(new NounComponent(NounType.Water));
      }
      else if (entityType == typeof(FloorEntity))
      {
        Texture2D spriteSheet = contentManager.Load<Texture2D>("Images/Sprites/floor");
        int[] spriteTime = {300, 300, 300};
        entity = new FloorEntity(num);
        entity.AddComponent(new SpriteComponent(new AnimatedSprite(spriteSheet, spriteTime, initalLocation, Color.LightGray)));
        entity.AddComponent(new PositionComponent(initalPosition));
        entity.AddComponent(new NounComponent(NounType.Water));
      }
      else if (entityType == typeof(TextBabaEntity))
      {
        Texture2D spriteSheet = contentManager.Load<Texture2D>("Images/Sprites/word-baba");
        int[] spriteTime = {300, 300, 300};
        entity = new TextBabaEntity(num);
        entity.AddComponent(new SpriteComponent(new AnimatedSprite(spriteSheet, spriteTime, initalLocation, Color.Blue)));
        entity.AddComponent(new PositionComponent(initalPosition));
        entity.AddComponent(new PropertyComponent(Properties.Push));
        entity.AddComponent(new TextComponent(TextType.Noun));
        entity.AddComponent(new NounComponent(NounType.BigBlue));
      }
      else if (entityType == typeof(TextFlagEntity))
      {
        Texture2D spriteSheet = contentManager.Load<Texture2D>("Images/Sprites/word-flag");
        int[] spriteTime = {300, 300, 300};
        entity = new TextFlagEntity(num);
        entity.AddComponent(new SpriteComponent(new AnimatedSprite(spriteSheet, spriteTime, initalLocation, Color.Yellow)));
        entity.AddComponent(new PositionComponent(initalPosition));
        entity.AddComponent(new PropertyComponent(Properties.Push));
        entity.AddComponent(new TextComponent(TextType.Noun));
        entity.AddComponent(new NounComponent(NounType.Flag));
      }
      else if (entityType == typeof(TextIsEntity))
      {
        Texture2D spriteSheet = contentManager.Load<Texture2D>("Images/Sprites/word-is");
        int[] spriteTime = {300, 300, 300};
        entity = new TextIsEntity(num);
        entity.AddComponent(new SpriteComponent(new AnimatedSprite(spriteSheet, spriteTime, initalLocation, Color.White)));
        entity.AddComponent(new PositionComponent(initalPosition));
        entity.AddComponent(new PropertyComponent(Properties.Push));
        entity.AddComponent(new TextComponent(TextType.Adjective));
        entity.AddComponent(new NounComponent(NounType.Is));
      }
      else if (entityType == typeof(TextKillEntity))
      {
        Texture2D spriteSheet = contentManager.Load<Texture2D>("Images/Sprites/word-kill");
        int[] spriteTime = {300, 300, 300};
        entity = new TextKillEntity(num);
        entity.AddComponent(new SpriteComponent(new AnimatedSprite(spriteSheet, spriteTime, initalLocation, Color.DarkRed)));
        entity.AddComponent(new PositionComponent(initalPosition));
        entity.AddComponent(new PropertyComponent(Properties.Push));
        entity.AddComponent(new TextComponent(TextType.Verb));
        entity.AddComponent(new NounComponent(NounType.Kill));
      }
      else if (entityType == typeof(TextLavaEntity))
      {
        Texture2D spriteSheet = contentManager.Load<Texture2D>("Images/Sprites/word-lava");
        int[] spriteTime = {300, 300, 300};
        entity = new TextLavaEntity(num);
        entity.AddComponent(new SpriteComponent(new AnimatedSprite(spriteSheet, spriteTime, initalLocation, Color.OrangeRed)));
        entity.AddComponent(new PositionComponent(initalPosition));
        entity.AddComponent(new PropertyComponent(Properties.Push));
        entity.AddComponent(new TextComponent(TextType.Noun));
        entity.AddComponent(new NounComponent(NounType.Lava));
      }
      else if (entityType == typeof(TextPushEntity))
      {
        Texture2D spriteSheet = contentManager.Load<Texture2D>("Images/Sprites/word-push");
        int[] spriteTime = {300, 300, 300};
        entity = new TextPushEntity(num);
        entity.AddComponent(new SpriteComponent(new AnimatedSprite(spriteSheet, spriteTime, initalLocation, Color.Ivory)));
        entity.AddComponent(new PositionComponent(initalPosition));
        entity.AddComponent(new PropertyComponent(Properties.Push));
        entity.AddComponent(new TextComponent(TextType.Verb));
        entity.AddComponent(new NounComponent(NounType.Push));
      }
      else if (entityType == typeof(TextRockEntity))
      {
        Texture2D spriteSheet = contentManager.Load<Texture2D>("Images/Sprites/word-rock");
        int[] spriteTime = {300, 300, 300};
        entity = new TextRockEntity(num);
        entity.AddComponent(new SpriteComponent(new AnimatedSprite(spriteSheet, spriteTime, initalLocation, Color.SaddleBrown)));
        entity.AddComponent(new PositionComponent(initalPosition));
        entity.AddComponent(new PropertyComponent(Properties.Push));
        entity.AddComponent(new TextComponent(TextType.Noun));
        entity.AddComponent(new NounComponent(NounType.Rock));
      }
      else if (entityType == typeof(TextSinkEntity))
      {
        Texture2D spriteSheet = contentManager.Load<Texture2D>("Images/Sprites/word-sink");
        int[] spriteTime = {300, 300, 300};
        entity = new TextSinkEntity(num);
        entity.AddComponent(new SpriteComponent(new AnimatedSprite(spriteSheet, spriteTime, initalLocation, Color.DarkBlue)));
        entity.AddComponent(new PositionComponent(initalPosition));
        entity.AddComponent(new PropertyComponent(Properties.Push));
        entity.AddComponent(new TextComponent(TextType.Verb));
        entity.AddComponent(new NounComponent(NounType.Sink));
      }
      else if (entityType == typeof(TextStopEntity))
      {
        Texture2D spriteSheet = contentManager.Load<Texture2D>("Images/Sprites/word-stop");
        int[] spriteTime = {300, 300, 300};
        entity = new TextStopEntity(num);
        entity.AddComponent(new SpriteComponent(new AnimatedSprite(spriteSheet, spriteTime, initalLocation, Color.Magenta)));
        entity.AddComponent(new PositionComponent(initalPosition));
        entity.AddComponent(new PropertyComponent(Properties.Push));
        entity.AddComponent(new TextComponent(TextType.Verb));
        entity.AddComponent(new NounComponent(NounType.Stop));
      }
      else if (entityType == typeof(TextWallEntity))
      {
        Texture2D spriteSheet = contentManager.Load<Texture2D>("Images/Sprites/word-wall");
        int[] spriteTime = {300, 300, 300};
        entity = new TextWallEntity(num);
        entity.AddComponent(new SpriteComponent(new AnimatedSprite(spriteSheet, spriteTime, initalLocation, Color.DarkGray)));
        entity.AddComponent(new PositionComponent(initalPosition));
        entity.AddComponent(new PropertyComponent(Properties.Push));
        entity.AddComponent(new TextComponent(TextType.Noun));
        entity.AddComponent(new NounComponent(NounType.Wall));
      }
      else if (entityType == typeof(TextWaterEntity))
      {
        Texture2D spriteSheet = contentManager.Load<Texture2D>("Images/Sprites/word-water");
        int[] spriteTime = {300, 300, 300};
        entity = new TextWaterEntity(num);
        entity.AddComponent(new SpriteComponent(new AnimatedSprite(spriteSheet, spriteTime, initalLocation, Color.Aqua)));
        entity.AddComponent(new PositionComponent(initalPosition));
        entity.AddComponent(new PropertyComponent(Properties.Push));
        entity.AddComponent(new TextComponent(TextType.Noun));
        entity.AddComponent(new NounComponent(NounType.Water));
      }
      else if (entityType == typeof(TextWinEntity))
      {
        Texture2D spriteSheet = contentManager.Load<Texture2D>("Images/Sprites/word-win");
        int[] spriteTime = {300, 300, 300};
        entity = new TextWinEntity(num);
        entity.AddComponent(new SpriteComponent(new AnimatedSprite(spriteSheet, spriteTime, initalLocation, Color.Gold)));
        entity.AddComponent(new PositionComponent(initalPosition));
        entity.AddComponent(new PropertyComponent(Properties.Push));
        entity.AddComponent(new TextComponent(TextType.Verb));
        entity.AddComponent(new NounComponent(NounType.Win));
      }
      else if (entityType == typeof(TextYouEntity))
      {
        Texture2D spriteSheet = contentManager.Load<Texture2D>("Images/Sprites/word-you");
        int[] spriteTime = {300, 300, 300};
        entity = new TextYouEntity(num);
        entity.AddComponent(new SpriteComponent(new AnimatedSprite(spriteSheet, spriteTime, initalLocation, Color.Blue)));
        entity.AddComponent(new PositionComponent(initalPosition));
        entity.AddComponent(new PropertyComponent(Properties.Push));
        entity.AddComponent(new TextComponent(TextType.Noun));
        entity.AddComponent(new NounComponent(NounType.You));
      }

      return entity;
    }
  }
}
