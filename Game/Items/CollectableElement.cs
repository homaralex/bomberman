﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Bomberman.Game.Items
{
    abstract class CollectableElement : GameElement, ICollectable
    {
        public abstract void Collect(GameManager gameManager);

        public CollectableElement(Vector2 position)
        {
            this.Position = position;
        }
    }
}