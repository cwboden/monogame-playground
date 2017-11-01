using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidEngine
{
    public static class Collision
    {
        public static bool HasRectangularCollision(Sprite a, Sprite b)
        {
            return HasRectangularCollision(a.Rectangle, b.Rectangle);
        }

        public static bool HasRectangularCollision(Rectangle a, Rectangle b)
        {
            return a.Intersects(b);
        }

        public static bool HasPerPixelCollision(Sprite a, Sprite b)
        {
            // relativeToB * transformB = relativeToA * transformA
            // relativeToB = relativeToA * transformA * Invert(relativeToB)
            // aTob = transformA * Invert(transformB)
            // relativeToB = relativeToA * aTob

            Matrix aTob = a.Transform * Matrix.Invert(b.Transform);

            var sourceColors = new Color[a.Texture.Width * a.Texture.Height];
            a.Texture.GetData(sourceColors);

            var targetColors = new Color[b.Texture.Width * b.Texture.Height];
            b.Texture.GetData(targetColors);

            // Unit step vectors
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, aTob);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, aTob);

            Vector2 targetPosition = Vector2.Transform(Vector2.Zero, aTob);

            for (int x = 0; x < a.Texture.Width; ++x)
            {
                Vector2 currentTargetPosition = targetPosition;
                for (int y = 0; y < a.Texture.Height; ++y)
                {
                    int targetX = (int)currentTargetPosition.X;
                    int targetY = (int)currentTargetPosition.Y;

                    if (0 <= targetX && targetX < b.Texture.Width &&
                        0 <= targetY && targetY < b.Texture.Height)
                    {
                        Color sourceColor = sourceColors[x + (y * a.Texture.Width)];
                        Color targetColor = targetColors[targetX + (targetY * b.Texture.Width)];

                        if (sourceColor.A != 0 && targetColor.A != 0)
                        {
                            return true;
                        }
                    }

                    currentTargetPosition += stepY;
                }

                targetPosition += stepX;
            }

            return false;
        }

        //public static Vector2 GetNormal(RectangleCollisionPoint point, bool isInside = false)
        //{
        //    switch (point)
        //    {
        //        case RectangleCollisionPoint.Top:
        //            return new Vector2(0, isInside ? 1 : -1);
        //        case RectangleCollisionPoint.Left:
        //            return new Vector2(isInside ? 1 : -1, 0);
        //        case RectangleCollisionPoint.Bottom:
        //            return new Vector2(0, isInside ? -1 : 1);
        //        case RectangleCollisionPoint.Right:
        //            return new Vector2(isInside ? -1 : 1, 0);
        //        case RectangleCollisionPoint.TopLeftCorner:
        //            return GetCornerNormal(new Vector2(isInside ? -1 : 1, isInside ? -1 : 1));
        //        case RectangleCollisionPoint.TopRightCorner:
        //            return GetCornerNormal(new Vector2(isInside ? 1 : -1, isInside ? -1 : 1));
        //        case RectangleCollisionPoint.BottomLeftCorner:
        //            return GetCornerNormal(new Vector2(isInside ? -1 : 1, isInside ? 1 : -1));
        //        case RectangleCollisionPoint.BottomRightCorner:
        //            return GetCornerNormal(new Vector2(isInside ? 1 : -1, isInside ? 1 : -1));
        //        default:
        //            return Vector2.Zero;
        //    }
        //}

        public static Vector2 GetCornerNormal(Vector2 delta)
        {
            Vector2 n = delta;
            n.Normalize();

            return n;
        }
    }
}
