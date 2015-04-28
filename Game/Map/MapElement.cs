﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bomberman.Game.Map
{
    abstract class MapElement : DestroyableElement
    {
        public const int WIDTH = 900 / 15; //TODO: no magic numbers!!
        public const int HEIGHT = 900 / 15;

        public List<DestroyableElement> OccupyingElements;
        public abstract bool IsWalkingTerrain { get; }
        public abstract bool IsFlyingTerrain { get; }
        public abstract bool IsContainerTerrain { get; }
        public abstract bool CanBeDestroyed { get; }

        public bool IsOccupied { get { return OccupyingElements.Count > 0; } }
        public void Occupy(DestroyableElement element) { OccupyingElements.Add(element); }
        public void Leave(DestroyableElement element) { OccupyingElements.Remove(element); }
        public void AddCollectable(CollectableElement collectable) { this.Collectable = collectable; }
        public CollectableElement Collectable { get; private set; }
        public CollectableElement PickCollectable()
        {
            var collectable = this.Collectable;
            this.Collectable = null;
            return collectable;
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            base.Draw(spriteBatch);
            if (this.Collectable != null && !this.CanBeDestroyed)
                this.Collectable.Draw(spriteBatch);
        }
        public MapElement(int x, int y)
        {
            this.X = x;
            this.Y = y;

            this.Position = new Vector2(x * MapElement.WIDTH, (y + 1) * MapElement.HEIGHT);
            this.OccupyingElements = new List<DestroyableElement>();
        }
        public MapElement() { this.OccupyingElements = new List<DestroyableElement>(); }

        public override int Destroy()
        {
            int pointsScored = GetValue();

            foreach (var destroyable in OccupyingElements)
            {
                pointsScored += destroyable.Destroy();
            }
            OccupyingElements.Clear();

            return pointsScored;
        }
    }

    class Ground : MapElement
    {
        public const string ASSET_NAME = "textures/map_objects/earth";
        private static Texture2D TEXTURE;
        private static int POINTS_VALUE = 0;

        public override int GetValue()
        {
            return Ground.POINTS_VALUE;
        }

        public override bool IsWalkingTerrain { get { return true; } }
        public override bool IsFlyingTerrain
        {
            get { return true; }
        }
        public override bool CanBeDestroyed { get { return false; } }
        public override void LoadContent(ContentManager content)
        {
            Ground.TEXTURE = content.Load<Texture2D>(Ground.ASSET_NAME);
        }
        public override Texture2D GetTexture()
        {
            return Ground.TEXTURE;
        }

        public Ground(int x, int y) : base(x, y) { }

        public override bool IsContainerTerrain
        {
            get { return false; }
        }
    }

    class Woods : MapElement
    {
        public const string ASSET_NAME = "textures/map_objects/woods";
        private static Texture2D TEXTURE;
        private static int POINTS_VALUE = 10;

        public override bool IsWalkingTerrain { get { return false; } }
        public override bool IsFlyingTerrain
        {
            get { return true; }
        }
        public override bool CanBeDestroyed { get { return true; } }
        public override void LoadContent(ContentManager content)
        {
            TEXTURE = content.Load<Texture2D>(Woods.ASSET_NAME);
        }
        public override Texture2D GetTexture()
        {
            return Woods.TEXTURE;
        }
        public Woods(int x, int y) : base(x, y) { }

        public override int GetValue()
        {
            return Woods.POINTS_VALUE;
        }

        public override bool IsContainerTerrain
        {
            get { return true; }
        }
    }

    class Rock : MapElement
    {
        public const string ASSET_NAME = "textures/map_objects/rock";
        private static Texture2D TEXTURE;

        public override bool IsWalkingTerrain { get { return false; } }
        public override bool IsFlyingTerrain
        {
            get { return false; }
        }
        public override bool CanBeDestroyed { get { return false; } }
        public override void LoadContent(ContentManager content)
        {
            TEXTURE = content.Load<Texture2D>(Rock.ASSET_NAME);
        }
        public override Texture2D GetTexture()
        {
            return Rock.TEXTURE;
        }
        public Rock(int x, int y) : base(x, y) { }

        public override int GetValue()
        {
            return 0;
        }

        public override bool IsContainerTerrain
        {
            get { return false; }
        }
    }
}
