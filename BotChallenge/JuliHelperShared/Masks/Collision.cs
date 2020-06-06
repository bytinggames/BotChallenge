using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace JuliHelper
{
    public static class Collision
    {
        public static float minDist = 0.1f;

        public static bool perPixelCollision = false;

        //public static bool test = false;

        #region Vector

        public static bool ColVectorRectangle(Vector2 vec, M_Rectangle rect)
        {
            return (vec.X >= rect.pos.X && vec.X < rect.pos.X + rect.size.X
                 && vec.Y >= rect.pos.Y && vec.Y < rect.pos.Y + rect.size.Y);
        }

        public static CollisionResult DistVectorRectangle(Vector2 vec, M_Rectangle rect)
        {
            float distX = rect.pos.X - vec.X;
            if (distX >= 0)
                return new CollisionResult();
            else if (distX + rect.size.X < rect.size.X / 2f)
            {
                distX = rect.size.X + distX;
                if (distX <= 0)
                    return new CollisionResult();
            }

            float distY = rect.pos.Y - vec.Y;
            if (distY >= 0)
                return new CollisionResult();
            else if (distY + rect.size.Y < rect.size.Y / 2f)
            {
                distY = rect.size.Y + distY;
                if (distY <= 0)
                    return new CollisionResult();
            }

            if (Math.Abs(distX) <= Math.Abs(distY))
                return new CollisionResult() { collision = true, distance = Math.Abs(distX), axisCol = new Vector2(Math.Sign(distX), 0) };
            else
                return new CollisionResult() { collision = true, distance = Math.Abs(distY), axisCol = new Vector2(0, Math.Sign(distY)) };
        }

        public static CollisionResult DistVectorRectangle(Vector2 vec, M_Rectangle rect, Vector2 dir)
        {
            return DistPolygonPolygon(new M_Polygon(vec, new List<Vector2>() { Vector2.Zero }), rect.ToPolygon(), dir);
        }


        public static bool ColVectorPolygon(Vector2 vec, M_Polygon polygon)
        {
            List<Vector2> edges = polygon.GetEdges();
            List<Vector2> axes = new List<Vector2>();
            axes.AddRange(GetAxes(edges));

            if (axes.Count == 0)
            {
                if (polygon.vertices.Count == 1)
                    return polygon.pos + polygon.vertices[0] == vec;
                return false;
            }

            for (int i = 0; i < axes.Count; i++)
            {
                float projection1 = vec.X * axes[i].X + vec.Y * axes[i].Y;
                float[] projection2 = GetProjection(axes[i], polygon.pos, polygon.vertices, edges);

                float dirDist1 = projection2[1] - projection1 - minDist;
                float dirDist2 = projection2[0] - projection1 - minDist;

                if (dirDist1 < dirDist2)
                    dirDist2 = projection2[0] - projection1 + minDist;
                else
                {
                    dirDist1 = dirDist2;
                    dirDist2 = projection2[1] - projection1 + minDist;
                }

                if (Math.Sign(dirDist1) == Math.Sign(dirDist2))
                    return false;
            }
            return true;
        }

        public static CollisionResult DistVectorPolygon(Vector2 vec, M_Polygon polygon)
        {
            CollisionResult cr = new CollisionResult();

            List<Vector2> edges = polygon.GetEdges();
            List<Vector2> axes = new List<Vector2>();
            axes.AddRange(GetAxes(edges));

            if (axes.Count == 0)
            {
                if (polygon.vertices.Count == 1)
                    cr.collision = vec == polygon.pos + polygon.vertices[0];
                return cr;
            }

            for (int i = 0; i < axes.Count; i++)
            {
                float projection1 = vec.X * axes[i].X + vec.Y * axes[i].Y;
                float[] projection2 = GetProjection(axes[i], polygon.pos, polygon.vertices, edges);

                float dirDist1 = projection2[1] - projection1 - minDist;
                float dirDist2 = projection2[0] - projection1 - minDist;

                if (dirDist1 < dirDist2)
                    dirDist2 = projection2[0] - projection1 + minDist;
                else
                {
                    dirDist1 = dirDist2;
                    dirDist2 = projection2[1] - projection1 + minDist;
                }

                if (Math.Sign(dirDist1) == Math.Sign(dirDist2))
                    return new CollisionResult();

                if (!cr.distance.HasValue || Math.Abs(dirDist1) < Math.Abs(cr.distance.Value))
                {
                    cr.axisCol = -axes[i];
                    cr.distance = -dirDist1;
                }
                if (Math.Abs(dirDist2) < Math.Abs(cr.distance.Value))
                {
                    cr.axisCol = axes[i];
                    cr.distance = dirDist2;
                }
            }

            cr.collision = true;

            return cr;
        }

        public static CollisionResult DistVectorPolygon(Vector2 vec, M_Polygon polygon, Vector2 dir)
        {
            CollisionResult cr = new CollisionResult();

            if (dir == Vector2.Zero)
            {
                cr.collision = ColVectorPolygon(vec, polygon);
                return cr;
            }

            List<Vector2> edges = polygon.GetEdges();
            List<Vector2> axes = new List<Vector2>();
            axes.AddRange(GetAxes(edges));

            float[][] dirDists = new float[axes.Count][];

            for (int i = 0; i < axes.Count; i++)
            {
                float projection1 = vec.X * axes[i].X + vec.Y * axes[i].Y;
                float[] projection2 = GetProjection(axes[i], polygon.pos, polygon.vertices, edges);

                float dotDir = dir.X * axes[i].X + dir.Y * axes[i].Y;

                if (dotDir == 0f)
                {
                    //no move direction on this axis, if no collision on this axis, then there is no distance on this direction
                    if (projection1 <= projection2[0] || projection1 >= projection2[1])
                        return new CollisionResult();
                }
                else
                {
                    float dirDist1 = (projection2[1] - projection1 - minDist * Math.Sign(dotDir)) / dotDir;
                    float dirDist2 = (projection2[0] - projection1 - minDist * Math.Sign(dotDir)) / dotDir;

                    if (dirDist1 < dirDist2)
                        dirDists[i] = (new float[] { dirDist1, (projection2[0] - projection1 + minDist * Math.Sign(dotDir)) / dotDir });
                    else
                        dirDists[i] = (new float[] { dirDist2, (projection2[1] - projection1 + minDist * Math.Sign(dotDir)) / dotDir });

                    if (!cr.distance.HasValue || dirDists[i][0] > cr.distance)
                    {
                        cr.axisCol = axes[i];
                        cr.distance = dirDists[i][0];
                    }

                    if (!cr.distanceReversed.HasValue || dirDists[i][1] < cr.distanceReversed)
                    {
                        cr.axisColReversed = axes[i];
                        cr.distanceReversed = dirDists[i][1];
                    }
                }
            }

            if (cr.distance.HasValue)
            {
                for (int i = 0; i < dirDists.Length; i++)
                {
                    if (dirDists[i] != null && dirDists[i][1] <= cr.distance)
                        return new CollisionResult();
                }

                cr.SetCollisionFromDist();

                if (Vector2.Dot(dir, cr.axisCol) > 0)
                    cr.axisCol = -cr.axisCol;
                if (Vector2.Dot(dir, cr.axisColReversed) < 0)
                    cr.axisColReversed = -cr.axisColReversed;
            }

            return cr;
        }


        public static bool ColVectorCircle(Vector2 vec, M_Circle circle)
        {
            Vector2 dist = circle.pos - vec;
            return (dist.X * dist.X + dist.Y * dist.Y < circle.radius * circle.radius);
        }

        public static CollisionResult DistVectorCircle(Vector2 vec, M_Circle circle)
        {
            Vector2 dist = circle.pos - vec;
            float distLength = (float)Math.Sqrt(dist.X * dist.X + dist.Y * dist.Y);

            if (distLength >= circle.radius)
                return new CollisionResult();
            else
            {
                CollisionResult cr = new CollisionResult();
                cr.collision = true;
                cr.distance = circle.radius - distLength;
                cr.axisCol = -Vector2.Normalize(dist);
                return cr;
            }
        }

        public static CollisionResult DistVectorCircle(Vector2 vec, M_Circle circle, Vector2 dir)
        {
            CollisionResult cr = new CollisionResult();

            Vector2 dist = circle.pos - vec;

            float radius = circle.radius + minDist;

            float?[] results = ABCFormula(dir.X * dir.X + dir.Y * dir.Y
                                         , -2 * (dist.X * dir.X + dist.Y * dir.Y)
                                         , dist.X * dist.X + dist.Y * dist.Y - radius * radius);

            if (results[0] != null)
            {
                cr.distance = results[1].Value;
                cr.distanceReversed = results[0].Value;

                cr.SetCollisionFromDist();

                Vector2 colPos = vec + dir * cr.distance.Value;
                cr.axisCol = Vector2.Normalize(colPos - circle.pos);

                colPos = vec + dir * cr.distanceReversed.Value;
                cr.axisColReversed = Vector2.Normalize(colPos - circle.pos);
            }

            return cr;
        }


        public static bool ColVectorSprite(Vector2 vec, Sprite sprite) //TODO: test
        {
            M_Rectangle rect = sprite.GetRectangle();

            if (ColVectorRectangle(vec, rect))
            {
                Vector2 transformedPos = Vector2.Transform(vec, Matrix.Invert(sprite.GetMatrix()));
                int x = (int)transformedPos.X;
                int y = (int)transformedPos.Y;
                if (x >= 0 && x < sprite.sizex && y >= 0 && y < sprite.sizey)
                {
                    int index = y * sprite.sizex + x;
                    //if (index >= 0 && index < sprite1.colorData.Length) //not necessary
                    return sprite.colorData[index].A != 0;
                }
            }
            return false;
        }

        #endregion

        #region Rectangle

        public static bool ColRectangleRectangle(M_Rectangle rect1, M_Rectangle rect2)
        {
            return (rect1.pos.X + rect1.size.X > rect2.pos.X && rect1.pos.X < rect2.pos.X + rect2.size.X
                 && rect1.pos.Y + rect1.size.Y > rect2.pos.Y && rect1.pos.Y < rect2.pos.Y + rect2.size.Y);
        
        }

        public static CollisionResult DistRectangleRectangle(M_Rectangle rect1, M_Rectangle rect2)
        {
            float distRight = rect2.Left - rect1.Right;
            if (distRight < 0)
            {
                float distLeft = rect1.size.X + rect2.size.X + distRight;
                if (distLeft > 0)
                {
                    float distDown = rect2.Top - rect1.Bottom;
                    if (distDown < 0)
                    {
                        float distUp = rect1.size.Y + rect2.size.Y + distDown;

                        if (distUp > 0)
                        {
                            distRight = Calculate.MinAbs(distRight, distLeft); //distX
                            distDown = Calculate.MinAbs(distDown, distUp); //distY

                            if (Math.Abs(distRight) <= Math.Abs(distDown))
                                return new CollisionResult() { collision = true, distance = Math.Abs(distRight), axisCol = new Vector2(Math.Sign(distRight), 0) };
                            else
                                return new CollisionResult() { collision = true, distance = Math.Abs(distDown), axisCol = new Vector2(0, Math.Sign(distDown)) };
                        }
                    }
                }
            }

            return new CollisionResult();
        }

        public static CollisionResult DistRectangleRectangle(M_Rectangle rect1, M_Rectangle rect2, Vector2 dir) //TODO: improve (not hard)
        {
            return DistPolygonPolygon(rect1.ToPolygon(), rect2.ToPolygon(), dir);
        }


        public static bool ColRectanglePolygon(M_Rectangle rect, M_Polygon polygon)
        {
            return ColPolygonPolygon(rect.ToPolygon(), polygon);
        }

        public static CollisionResult DistRectanglePolygon(M_Rectangle rect, M_Polygon polygon)
        {
            return DistPolygonPolygon(rect.ToPolygon(), polygon);
        }

        public static CollisionResult DistRectanglePolygon(M_Rectangle rect, M_Polygon polygon, Vector2 dir)
        {
            return DistPolygonPolygon(rect.ToPolygon(), polygon, dir);
        }


        public static bool ColRectangleCircle(M_Rectangle rect, M_Circle circle)
        {
            Vector2 dist;
            dist.X = Math.Abs(circle.pos.X - (rect.pos.X + rect.size.X / 2));
            dist.Y = Math.Abs(circle.pos.Y - (rect.pos.Y + rect.size.Y / 2));

            //Check if bounding boxes collide
            if (dist.X >= circle.radius + rect.size.X / 2 || dist.Y >= circle.radius + rect.size.Y / 2)
                return false;

            //Check if Circle center in rectangle
            if (dist.X <= rect.size.X / 2 || dist.Y <= rect.size.Y / 2)
                return true;

            //Calc corner distance
            float cornerDistPow = (float)(Math.Pow(dist.X - rect.size.X / 2, 2) + Math.Pow(dist.Y - rect.size.Y / 2, 2));

            return cornerDistPow < Math.Pow(circle.radius, 2);
        }

        public static CollisionResult DistRectangleCircle(M_Rectangle rect, M_Circle circle)
        {
            return DistPolygonCircle(rect.ToPolygon(), circle);
        }

        public static CollisionResult DistRectangleCircle(M_Rectangle rect, M_Circle circle, Vector2 dir)
        {
            return DistPolygonCircle(rect.ToPolygon(), circle, dir);
        }


        public static bool ColRectangleSprite(M_Rectangle rect, Sprite sprite) //TODO: fix precision error (bottom of rectangle is 1 px smaller than given)
        {
            M_Rectangle rect2 = sprite.GetRectangle();

            if (ColRectangleRectangle(rect2, rect))
            {
                Matrix transform1 = Matrix.Invert(sprite.GetMatrix());

                float xStart = Math.Max(0, rect2.pos.X - rect.pos.X);
                float xEnd = Math.Min(rect.size.X, rect2.pos.X + rect2.size.X - rect.pos.X);
                float yStart = Math.Max(0, rect2.pos.Y - rect.pos.Y);
                float yEnd = Math.Min(rect.size.Y, rect2.pos.Y + rect2.size.Y - rect.pos.Y);

                Vector2 pos1InY = Vector2.Transform(rect.pos + new Vector2(xStart, yStart), transform1);
                Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transform1);
                Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transform1);
                for (float y2 = yStart; ;)
                {
                    Vector2 pos1 = pos1InY;
                    for (float x2 = xStart; ;)
                    {
                        if (pos1.X >= 0 && pos1.X < sprite.sizex && pos1.Y >= 0 && pos1.Y < sprite.sizey)
                        {
                            if (sprite.colorData[(int)pos1.Y * sprite.sizex + (int)pos1.X].A != 0)
                                return true;
                        }
                        pos1 += stepX;

                        x2++;
                        if (x2 == xEnd && pos1.X % 1 == 0)
                            break;
                        else if (x2 > xEnd)
                            break;
                    }
                    pos1InY += stepY;

                    y2++;
                    if (y2 == yEnd && pos1InY.Y % 1 == 0)
                        break;
                    else if (y2 > yEnd)
                        break;
                }
            }

            return false;
        }

        #endregion

        #region Polygon

        public static bool ColPolygonPolygon(M_Polygon poly1, M_Polygon poly2)
        {
            List<Vector2> edges1 = poly1.GetEdges();
            List<Vector2> edges2 = poly2.GetEdges();
            List<Vector2> axes = new List<Vector2>();
            axes.AddRange(GetAxes(edges1));
            axes.AddRange(GetAxes(edges2));

            if (axes.Count == 0)
            {
                if (poly1.vertices.Count == 1 && poly2.vertices.Count == 1)
                    return poly1.pos + poly1.vertices[0] == poly2.pos + poly2.vertices[0];
                return false;
            }

            for (int i = 0; i < axes.Count; i++)
            {
                float[] projection1 = GetProjection(axes[i], poly1.pos, poly1.vertices, edges1);
                float[] projection2 = GetProjection(axes[i], poly2.pos, poly2.vertices, edges2);

                if (projection1[0] >= projection2[1] - minDist || projection1[1] <= projection2[0] + minDist)
                    return false;

                ////minDist inclusion
                //if (projection1[0] > projection2[1] + minDist || projection1[1] < projection2[0] - minDist)
                //    return false;
            }
            return true;
        }

        public static CollisionResult DistPolygonPolygon(M_Polygon poly1, M_Polygon poly2)
        {
            CollisionResult cr = new CollisionResult();

            List<Vector2> edges1 = poly1.GetEdges();
            List<Vector2> edges2 = poly2.GetEdges();
            List<Vector2> axes = new List<Vector2>();
            axes.AddRange(GetAxes(edges1));
            axes.AddRange(GetAxes(edges2));

            if (axes.Count == 0)
            {
                if (poly1.vertices.Count == 1 && poly2.vertices.Count == 1)
                {
                    cr.collision = poly1.pos + poly1.vertices[0] == poly2.pos + poly2.vertices[0];
                }
                return cr;
            }

            for (int i = 0; i < axes.Count; i++)
            {
                float[] projection1 = GetProjection(axes[i], poly1.pos, poly1.vertices, edges1);
                float[] projection2 = GetProjection(axes[i], poly2.pos, poly2.vertices, edges2);

                float dirDist1 = projection2[1] - projection1[0] - minDist;
                float dirDist2 = projection2[0] - projection1[1] - minDist;

                if (dirDist1 < dirDist2)
                    dirDist2 = projection2[0] - projection1[1] + minDist;
                else
                {
                    dirDist1 = dirDist2;
                    dirDist2 = projection2[1] - projection1[0] + minDist;
                }

                if (Math.Sign(dirDist1) == Math.Sign(dirDist2))
                    return new CollisionResult();

                if (!cr.distance.HasValue || Math.Abs(dirDist1) < Math.Abs(cr.distance.Value))
                {
                    cr.axisCol = -axes[i];
                    cr.distance = -dirDist1;
                }
                if (Math.Abs(dirDist2) < Math.Abs(cr.distance.Value))
                {
                    cr.axisCol = axes[i];
                    cr.distance = dirDist2;
                }
            }

            cr.collision = true;

            return cr;
        }

        public static CollisionResult DistPolygonPolygon(M_Polygon poly1, M_Polygon poly2, Vector2 dir)
        {
            if (poly1.closed && poly2.closed)
                return DistPolygonPolygonClosed(poly1, poly2, dir);
            else
                return DistPolygonPolygonOpened(poly1, poly2, dir);
        }

        public static CollisionResult DistPolygonPolygonClosed(M_Polygon poly1, M_Polygon poly2, Vector2 dir)
        {
            CollisionResult cr = new CollisionResult();

            if (dir == Vector2.Zero)
            {
                cr.collision = ColPolygonPolygon(poly1, poly2);
                return cr;
            }

            List<Vector2> edges1 = poly1.GetClosedEdges();
            List<Vector2> edges2 = poly2.GetClosedEdges();

            //List<Axis> axes = new List<Axis>();

            int polyAxisCol = 0;
            int jNearest = -1;
            int jNearestReversed = -1;

            for (int i = 0; i < edges1.Count; i++)
            {
                int j = 0;
                int jFinal = 0;
                //axes.Add(new Axis(edges1[i]));
                Axis axis = new Axis(edges1[i]);
                axis.a1 = Vector2.Dot(poly1.pos + poly1.vertices[i], axis.axis);

                axis.b2 = Vector2.Dot(poly2.pos + poly2.vertices[j], axis.axis);

                for (j = 1; j < poly2.vertices.Count; j++)
                {
                    float dot = Vector2.Dot(poly2.pos + poly2.vertices[j], axis.axis);
                    if (dot > axis.b2)
                    {
                        axis.b2 = dot;
                        jFinal = j;
                    }
                }

                float dotDir = Vector2.Dot(dir, axis.axis);
                float dist = (axis.b2 - axis.a1) / dotDir;

                if (dotDir < 0)
                {
                    if (!cr.distance.HasValue || dist > cr.distance)
                    {
                        cr.distance = dist;
                        cr.axisCol = axis.axis;
                        jNearest = jFinal;
                    }
                }
                else if (dotDir > 0)
                {
                    if (!cr.distanceReversed.HasValue || dist < cr.distanceReversed)
                    {
                        cr.distanceReversed = dist;
                        cr.axisColReversed = axis.axis;
                        jNearestReversed = jFinal;
                    }
                }
                else if (axis.b2 - axis.a1 <= 0) //TODO: CHECK: <= before was < (2017.08.14)
                    return new CollisionResult();

            }
            for (int i = 0; i < edges2.Count; i++)
            {
                int j = 0;
                int jFinal = 0;
                //axes.Add(new Axis(edges2[i]));
                Axis axis = new Axis(edges2[i]);
                axis.a1 = Vector2.Dot(poly2.pos + poly2.vertices[i], axis.axis);

                axis.b2 = Vector2.Dot(poly1.pos + poly1.vertices[j], axis.axis);

                for (j = 1; j < poly1.vertices.Count; j++)
                {
                    float dot = Vector2.Dot(poly1.pos + poly1.vertices[j], axis.axis);
                    if (dot > axis.b2)
                    {
                        axis.b2 = dot;
                        jFinal = j;
                    }
                }

                float dotDir = -Vector2.Dot(dir, axis.axis);
                float dist = (axis.b2 - axis.a1) / dotDir;

                if (dotDir < 0)
                {
                    if (!cr.distance.HasValue || dist > cr.distance)
                    {
                        cr.distance = dist;
                        cr.axisCol = -axis.axis;
                        jNearest = jFinal;
                        polyAxisCol = 1;
                    }
                }
                else if (dotDir > 0)
                {
                    if (!cr.distanceReversed.HasValue || dist < cr.distanceReversed)
                    {
                        cr.distanceReversed = dist;
                        cr.axisColReversed = -axis.axis;
                        jNearestReversed = jFinal;
                    }
                }
                else if (axis.b2 - axis.a1 <= 0) //TODO: CHECK: <= before was < (2017.08.14)
                    return new CollisionResult();
            }

            if (cr.distance.HasValue && cr.distanceReversed <= cr.distance)
                return new CollisionResult();

            cr.colCornerPoly = polyAxisCol + 1;
            cr.colCornerIndex = jNearest;
            
            cr.distance -= minDist / Math.Abs(Vector2.Dot(cr.axisCol, dir));
            cr.distanceReversed += minDist / Math.Abs(Vector2.Dot(cr.axisColReversed, dir));

            cr.collision = cr.distance < 0 && cr.distanceReversed > 0;

            return cr;
        }

        public static CollisionResult DistPolygonPolygonOpened(M_Polygon poly1, M_Polygon poly2, Vector2 dir)
        {
            bool open = false, openReversed = false;

            CollisionResult cr = new CollisionResult();

            if (dir == Vector2.Zero)
            {
                cr.collision = ColPolygonPolygon(poly1, poly2);
                return cr;
            }

            List<Vector2> edges1 = poly1.GetClosedEdges();
            List<Vector2> edges2 = poly2.GetClosedEdges();

            for (int i = 0; i < edges1.Count; i++)
            {
                int endVerticeCol = -1; //if the collision happens to be on an open vertice save the index of this vertice here
                Axis axis = new Axis(edges1[i]);
                axis.a1 = Vector2.Dot(poly1.pos + poly1.vertices[i], axis.axis);

                axis.b2 = Vector2.Dot(poly2.pos + poly2.vertices[0], axis.axis);
                endVerticeCol = !poly2.closed ? 0 : -1;

                for (int j = 1; j < poly2.vertices.Count; j++)
                {
                    float dot = Vector2.Dot(poly2.pos + poly2.vertices[j], axis.axis);
                    if (dot > axis.b2)
                    {
                        axis.b2 = dot;
                        endVerticeCol = (!poly2.closed && j == poly2.vertices.Count - 1) ? poly2.vertices.Count - 1 : -1;
                    }
                }

                float dotDir = Vector2.Dot(dir, axis.axis);
                float dist = (axis.b2 - axis.a1) / dotDir;

                if (dotDir < 0)
                {
                    if (!cr.distance.HasValue || dist > cr.distance)
                    {
                        cr.distance = dist;
                        cr.axisCol = axis.axis;

                        if (!poly1.closed && i == edges1.Count - 1)
                            open = true;
                        else
                        {
                            open = false;
                            if (endVerticeCol == 0)
                            {
                                if (!poly2.startCorner || Vector2.Dot(new Vector2(-edges2[0].Y, edges2[0].X), cr.axisCol) > 0)
                                    open = true;
                            }
                            else if (endVerticeCol != -1)
                            {
                                if (!poly2.endCorner || Vector2.Dot(new Vector2(-edges2[endVerticeCol - 1].Y, edges2[endVerticeCol - 1].X), cr.axisCol) > 0)
                                    open = true;
                            }
                        }
                    }
                }
                else if (dotDir > 0)
                {
                    if (!cr.distanceReversed.HasValue || dist < cr.distanceReversed)
                    {
                        cr.distanceReversed = dist;
                        cr.axisColReversed = axis.axis;

                        if (!poly1.closed && i == edges1.Count - 1)
                            openReversed = true;
                        else
                        {
                            openReversed = false;
                            if (endVerticeCol == 0)
                            {
                                if (!poly2.startCorner || Vector2.Dot(new Vector2(-edges2[0].Y, edges2[0].X), cr.axisColReversed) > 0)
                                    openReversed = true;
                            }
                            else if (endVerticeCol != -1)
                            {
                                if (!poly2.endCorner || Vector2.Dot(new Vector2(-edges2[endVerticeCol - 1].Y, edges2[endVerticeCol - 1].X), cr.axisColReversed) > 0)
                                    openReversed = true;
                            }
                        }
                    }
                }
                else if (axis.b2 - axis.a1 < 0)
                    return new CollisionResult();

            }
            for (int i = 0; i < edges2.Count; i++)
            {
                int endVerticeCol = -1; //if the collision happens to be on an open vertice save the index of this vertice here
                Axis axis = new Axis(edges2[i]);
                axis.a1 = Vector2.Dot(poly2.pos + poly2.vertices[i], axis.axis);

                axis.b2 = Vector2.Dot(poly1.pos + poly1.vertices[0], axis.axis);
                endVerticeCol = !poly1.closed ? 0 : -1;

                for (int j = 1; j < poly1.vertices.Count; j++)
                {
                    float dot = Vector2.Dot(poly1.pos + poly1.vertices[j], axis.axis);
                    if (dot > axis.b2)
                    {
                        axis.b2 = dot;
                        endVerticeCol = (!poly1.closed && j == poly1.vertices.Count - 1) ? poly1.vertices.Count - 1 : -1;
                    }
                }

                float dotDir = -Vector2.Dot(dir, axis.axis);
                float dist = (axis.b2 - axis.a1) / dotDir;

                if (dotDir < 0)
                {
                    if (!cr.distance.HasValue || dist > cr.distance)
                    {
                        cr.distance = dist;
                        cr.axisCol = -axis.axis;

                        if (!poly2.closed && i == edges2.Count - 1)
                            open = true;
                        else
                        {
                            open = false;
                            if (endVerticeCol == 0)
                            {
                                if (!poly1.startCorner || Vector2.Dot(new Vector2(-edges1[0].Y, edges1[0].X), cr.axisCol) < 0)
                                    open = true;
                            }
                            else if (endVerticeCol != -1)
                            {
                                if (!poly1.endCorner || Vector2.Dot(new Vector2(-edges1[endVerticeCol - 1].Y, edges1[endVerticeCol - 1].X), cr.axisCol)< 0)
                                    open = true;
                            }
                        }
                    }
                }
                else if (dotDir > 0)
                {
                    if (!cr.distanceReversed.HasValue || dist < cr.distanceReversed)
                    {
                        cr.distanceReversed = dist;
                        cr.axisColReversed = -axis.axis;

                        if (!poly2.closed && i == edges2.Count - 1)
                            openReversed = true;
                        else
                        {
                            openReversed = false;
                            if (endVerticeCol == 0)
                            {
                                if (!poly1.startCorner || Vector2.Dot(new Vector2(-edges1[0].Y, edges1[0].X), cr.axisColReversed) < 0)
                                    openReversed = true;
                            }
                            else if (endVerticeCol != -1)
                            {
                                if (!poly1.endCorner || Vector2.Dot(new Vector2(-edges1[endVerticeCol - 1].Y, edges1[endVerticeCol - 1].X), cr.axisColReversed) < 0)
                                    openReversed = true;
                            }
                        }
                    }
                }
                else if (axis.b2 - axis.a1 < 0)
                    return new CollisionResult();
            }

            if (cr.distance.HasValue && cr.distanceReversed <= cr.distance)
                return new CollisionResult();


            cr.distance -= minDist / Math.Abs(Vector2.Dot(cr.axisCol, dir));
            cr.distanceReversed += minDist / Math.Abs(Vector2.Dot(cr.axisColReversed, dir));

            if (open)
            {
                cr.distance = null;
                cr.axisCol = Vector2.Zero;
            }
            if (openReversed)
            {
                cr.distanceReversed = null;
                cr.axisColReversed = Vector2.Zero;
            }


            if (cr.distance.HasValue && cr.distanceReversed.HasValue)
                cr.collision = cr.distance < 0 && cr.distanceReversed > 0;

            return cr;
        }

        public static CollisionResult DistPolygonPolygon2(M_Polygon poly1, M_Polygon poly2, Vector2 dir)
        {
            CollisionResult cr = new CollisionResult();

            if (dir == Vector2.Zero)
            {
                cr.collision = ColPolygonPolygon(poly1, poly2);
                return cr;
            }

            List<Vector2> edges1 = poly1.GetEdges();
            List<Vector2> edges2 = poly2.GetEdges();
            List<Vector2> axes = new List<Vector2>();
            axes.AddRange(GetAxes(edges1));
            axes.AddRange(GetAxes(edges2));

            float[][] dirDists = new float[axes.Count][];

            for (int i = 0; i < axes.Count; i++)
            {
                float[] projection1 = GetProjection(axes[i], poly1.pos, poly1.vertices, edges1);
                float[] projection2 = GetProjection(axes[i], poly2.pos, poly2.vertices, edges2);

                float dotDir = dir.X * axes[i].X + dir.Y * axes[i].Y;

                if (dotDir == 0f)
                {
                    //no move direction on this axis, if no collision on this axis, then there is no distance on this direction
                    if (projection1[1] <= projection2[0] || projection1[0] >= projection2[1])
                        return new CollisionResult();
                }
                else
                {
                    float dirDist1 = (projection2[1] - projection1[0] - minDist * Math.Sign(dotDir)) / dotDir;
                    float dirDist2 = (projection2[0] - projection1[1] - minDist * Math.Sign(dotDir)) / dotDir;

                    if (dirDist1 < dirDist2)
                        dirDists[i] = (new float[] { dirDist1, (projection2[0] - projection1[1] + minDist * Math.Sign(dotDir)) / dotDir });
                    else
                        dirDists[i] = (new float[] { dirDist2, (projection2[1] - projection1[0] + minDist * Math.Sign(dotDir)) / dotDir });


                    if (!cr.distance.HasValue || dirDists[i][0] > cr.distance)
                    {
                        cr.axisCol = axes[i];
                        cr.distance = dirDists[i][0];
                    }

                    if (!cr.distanceReversed.HasValue || dirDists[i][1] < cr.distanceReversed)
                    {
                        cr.axisColReversed = axes[i];
                        cr.distanceReversed = dirDists[i][1];
                    }
                }
            }

            //if (cr.distance.HasValue)
            {
                for (int i = 0; i < dirDists.Length; i++)
                {
                    if (dirDists[i] != null && dirDists[i][1] <= cr.distance)
                        return new CollisionResult();
                }

                cr.SetCollisionFromDist();

                if (Vector2.Dot(dir, cr.axisCol) > 0)
                    cr.axisCol = -cr.axisCol;
                if (Vector2.Dot(dir, cr.axisColReversed) < 0)
                    cr.axisColReversed = -cr.axisColReversed;
            }

            return cr;
        }


        public static bool ColPolygonCircle(M_Polygon polygon, M_Circle circle)
        {
            if (polygon.vertices.Count == 0)
                return false;

            List<Vector2> edges1 = polygon.GetEdges();
            List<Vector2> axes = new List<Vector2>();
            axes.AddRange(GetAxes(edges1));

            if (axes.Count == 0)
            {
                if (polygon.vertices.Count == 1)
                    return ColVectorCircle(polygon.pos + polygon.vertices[0], circle);
                return false;
            }

            //add axis between circle and nearest vertice
            int nearestI = -1;
            float nearestDist = 0;
            for (int i = 0; i < polygon.vertices.Count; i++)
            {
                float cDist = (float)(Math.Pow(circle.pos.X - polygon.pos.X - polygon.vertices[i].X, 2) + Math.Pow(circle.pos.Y - polygon.pos.Y - polygon.vertices[i].Y, 2));
                if (i == 0 || cDist < nearestDist)
                {
                    nearestDist = cDist;
                    nearestI = i;
                }
            }

            Vector2 cornerAxis = circle.pos - polygon.pos - polygon.vertices[nearestI];
            if (cornerAxis != Vector2.Zero)
                axes.Add(Calculate.XPositive(Vector2.Normalize(cornerAxis)));


            for (int i = 0; i < axes.Count; i++)
            {
                float[] projection1 = GetProjection(axes[i], polygon.pos, polygon.vertices, edges1);
                float circle_axis = Vector2.Dot(axes[i], circle.pos);

                if (projection1[0] > circle_axis + circle.radius || projection1[1] < circle_axis - circle.radius)
                    return false;
            }
            return true;
        }

        public static CollisionResult DistPolygonCircle(M_Polygon polygon, M_Circle circle)
        {
            CollisionResult cr = new CollisionResult();

            List<Vector2> edges1 = polygon.GetEdges();
            List<Vector2> axes = new List<Vector2>();
            axes.AddRange(GetAxes(edges1));


            if (axes.Count == 0)
            {
                if (polygon.vertices.Count == 1)
                    cr.collision = ColVectorCircle(polygon.pos + polygon.vertices[0], circle);
                return cr;
            }

            //add axis between circle and nearest vertice
            int nearestI = -1;
            float nearestDist = 0;
            for (int i = 0; i < polygon.vertices.Count; i++)
            {
                float cDist = (float)(Math.Pow(circle.pos.X - polygon.pos.X - polygon.vertices[i].X, 2) + Math.Pow(circle.pos.Y - polygon.pos.Y - polygon.vertices[i].Y, 2));
                if (i == 0 || cDist < nearestDist)
                {
                    nearestDist = cDist;
                    nearestI = i;
                }
            }

            Vector2 cornerAxis = circle.pos - polygon.pos - polygon.vertices[nearestI];
            if (cornerAxis != Vector2.Zero)
                axes.Add(Calculate.XPositive(Vector2.Normalize(cornerAxis)));


            for (int i = 0; i < axes.Count; i++)
            {
                float[] projection1 = GetProjection(axes[i], polygon.pos, polygon.vertices, edges1);
                float circle_axis = Vector2.Dot(axes[i], circle.pos);
                float[] projection2 = new float[] { circle_axis - circle.radius, circle_axis + circle.radius };

                float dirDist1 = projection2[1] - projection1[0] - minDist;
                float dirDist2 = projection2[0] - projection1[1] - minDist;

                if (dirDist1 < dirDist2)
                    dirDist2 = projection2[0] - projection1[1] + minDist;
                else
                {
                    dirDist1 = dirDist2;
                    dirDist2 = projection2[1] - projection1[0] + minDist;
                }

                if (Math.Sign(dirDist1) == Math.Sign(dirDist2))
                    return new CollisionResult();

                if (!cr.distance.HasValue || Math.Abs(dirDist1) < Math.Abs(cr.distance.Value))
                {
                    cr.axisCol = -axes[i];
                    cr.distance = -dirDist1;
                }
                if (Math.Abs(dirDist2) < Math.Abs(cr.distance.Value))
                {
                    cr.axisCol = axes[i];
                    cr.distance = dirDist2;
                }
            }

            cr.collision = true;

            return cr;
        }

        public static CollisionResult DistPolygonCircle(M_Polygon polygon, M_Circle circle, Vector2 dir)
        {
            dir = -dir; //- for polygon, circle order change
            
            CollisionResult cr = new CollisionResult();

            if (dir == Vector2.Zero)
            {
                cr.collision = ColPolygonCircle(polygon, circle);
                return cr;
            }

            if (polygon.vertices.Count == 0)
                return cr;

            float? cDist = null;

            //find start on edge
            List<Vector2> edges2 = polygon.GetEdges();

            //Init for [0]
            Vector2 e = Vector2.Normalize(edges2[0]);
            Vector2 p = circle.pos - polygon.pos - polygon.vertices[0];

            Vector2 pDist;
            float pDist_e;

            int i = 0;
            bool noEdge = false;

            float radius = circle.radius + minDist;

            while (true)
            {
                //edge check
                if (!noEdge)
                {
                    Vector2 ne = new Vector2(e.Y, -e.X); //rotate counter-clockwise for the normal pointing out of the polygon
                    float p_ne = Vector2.Dot(p, ne);

                    float dir_ne = Vector2.Dot(dir, ne);

                    if (dir_ne != 0)
                    {
                        bool forward = dir_ne < 0;
                        Vector2 cDir = dir;
                        if (!forward)
                            cDir *= -1f;

                        //if (dir_ne < 0) //dir against ne? (pointing to the top side of the edge)
                        //{
                        //if (forward)
                        cDist = (radius - ne.X * p.X - ne.Y * p.Y) / (ne.X * cDir.X + ne.Y * cDir.Y);
                        //else
                        //    cDist = (-circle.radius - ne.X * p.X - ne.Y * p.Y) / (-ne.X * cDir.X - ne.Y * cDir.Y);

                        pDist = p + cDir * (float)cDist;
                        pDist_e = Vector2.Dot(pDist, e);

                        if (pDist_e >= 0 && pDist_e <= edges2[i].Length())
                        {
                            if (forward)
                            {
                                //cDist -= minDist / cDir.Length();
                            }
                            if (!forward)
                            {
                                cDist *= -1f;
                                //cDist += minDist / cDir.Length();
                            }

                            if (forward)
                            {
                                if (!cr.distance.HasValue)
                                {
                                    cr.axisCol = -ne; //- for polygon, circle order change
                                    cr.distance = cDist;
                                }
                            }
                            else if (!cr.distanceReversed.HasValue)
                            {
                                cr.distanceReversed = cDist;
                                cr.axisColReversed = -ne;
                            }

                            if (cr.distance.HasValue && cr.distanceReversed.HasValue)
                            {
                                cr.SetCollisionFromDist();
                                return cr;
                            }
                        }
                        //}
                    }
                }

                //next corner check
                noEdge = false;
                int j = i + 1;
                if (j == polygon.vertices.Count)
                    j = 0;
                else if (j == edges2.Count)
                {
                    if (!polygon.closed)//???????is this necessary?
                        noEdge = true;
                    //else
                    //    j = 0;
                }
                //cDist = GetDistCircle(circle.pos.X, circle.pos.Y, polygon.vertices[i].X + polygon.pos.X, polygon.vertices[i].Y + polygon.pos.Y, dir.X, dir.Y, circle.radius);

                p = circle.pos - polygon.pos - polygon.vertices[j];

                Vector2 oldE = e;
                if (!noEdge)
                    e = Vector2.Normalize(edges2[j]);
                else
                    e = new Vector2(-oldE.Y, oldE.X);//Vector2.Zero;

                if ((polygon.endCorner || j != polygon.vertices.Count - 1) && (polygon.startCorner || j != 0))
                {
                    float?[] cDists = ABCFormula(dir.X * dir.X + dir.Y * dir.Y, 2 * (p.X * dir.X + p.Y * dir.Y), p.X * p.X + p.Y * p.Y - radius * radius);

                    for (int k = 0; k < cDists.Length; k++)
                    {
                        //Vector2 cDir = dir;
                        //if (k == 0)
                        //    cDir *= -1f;

                        if (cDists[k].HasValue)
                        {
                            //check if the distancePoint is not in the prev voroni region
                            pDist = circle.pos + dir * (float)cDists[k] - polygon.pos - polygon.vertices[j];
                            if (j == 0 && !polygon.closed)
                                pDist_e = Vector2.Dot(pDist, new Vector2(e.Y, -e.X));
                            else
                                pDist_e = Vector2.Dot(pDist, oldE);

                            if (pDist_e > 0) //is pos right (clockwise) from the previous edge?
                            {

                                pDist = p + dir * (float)cDists[k];
                                pDist_e = Vector2.Dot(pDist, e);

                                //if (pDist_e <= 0) //TODO: check if replaced one works -v
                                if (pDist_e < 0) //is pos left (counter-clockwise) from the next edge?
                                {

                                    if (k == 0)
                                    {
                                        //cDists[k] += minDist / dir.Length();
                                        if (!cr.distanceReversed.HasValue)
                                        {
                                            cr.distanceReversed = cDists[k];
                                            cr.axisColReversed = Vector2.Normalize(polygon.pos + polygon.vertices[j] - cr.distanceReversed.Value * dir - circle.pos);
                                        }
                                    }
                                    else if (!cr.distance.HasValue)
                                    {
                                        //cDists[k] -= minDist / dir.Length();
                                        cr.distance = cDists[k];
                                        cr.axisCol = Vector2.Normalize(polygon.pos + polygon.vertices[j] - cr.distance.Value * dir - circle.pos);
                                    }

                                    if (cr.distance.HasValue && cr.distanceReversed.HasValue)
                                    {
                                        cr.SetCollisionFromDist();
                                        return cr;
                                    }
                                }
                            }
                        }
                    }
                }
                
                i = j;

                if (i == 0)
                    break;
            }
            return cr;
        }


        public static bool ColPolygonSprite(M_Polygon polygon, Sprite sprite)
        {
            throw new NotImplementedException();
        }

        #region old


        //public static CollisionResult DistPolygonPolygonOpened(M_Polygon poly1, M_Polygon poly2, Vector2 dir)
        //{
        //    bool open = false, openReversed = false;

        //    CollisionResult cr = new CollisionResult();

        //    if (dir == Vector2.Zero)
        //    {
        //        cr.collision = ColPolygonPolygon(poly1, poly2);
        //        return cr;
        //    }

        //    List<Vector2> edges1 = poly1.GetClosedEdges();
        //    List<Vector2> edges2 = poly2.GetClosedEdges();

        //    if (!Jau1(ref cr, dir, edges1, edges2, poly1, poly2, ref open, ref openReversed, 1))
        //        return new CollisionResult();
        //    if (!Jau1(ref cr, -dir, edges2, edges1, poly2, poly1, ref open, ref openReversed, -1))
        //        return new CollisionResult();

        //    if (cr.distance.HasValue && cr.distanceReversed <= cr.distance)
        //        return new CollisionResult();


        //    cr.distance -= minDist / Math.Abs(Vector2.Dot(cr.axisCol, dir));
        //    cr.distanceReversed += minDist / Math.Abs(Vector2.Dot(cr.axisColReversed, dir));

        //    if (open)
        //    {
        //        cr.distance = null;
        //        cr.axisCol = Vector2.Zero;
        //    }
        //    if (openReversed)
        //    {
        //        cr.distanceReversed = null;
        //        cr.axisColReversed = Vector2.Zero;
        //    }


        //    if (cr.distance.HasValue && cr.distanceReversed.HasValue)
        //        cr.collision = cr.distance < 0 && cr.distanceReversed > 0;

        //    return cr;
        //}

        //static bool Jau1(ref CollisionResult cr, Vector2 dir, List<Vector2> edges1, List<Vector2> edges2, M_Polygon poly1, M_Polygon poly2, ref bool open, ref bool openReversed, int reversed)
        //{
        //    for (int i = 0; i < edges1.Count; i++)
        //    {
        //        int endVerticeCol = -1; //if the collision happens to be on an open vertice save the index of this vertice here
        //        Axis axis = new Axis(edges1[i]);
        //        axis.a1 = Vector2.Dot(poly1.pos + poly1.vertices[i], axis.axis);

        //        axis.b2 = Vector2.Dot(poly2.pos + poly2.vertices[0], axis.axis);
        //        endVerticeCol = !poly2.closed ? 0 : -1;

        //        for (int j = 1; j < poly2.vertices.Count; j++)
        //        {
        //            float dot = Vector2.Dot(poly2.pos + poly2.vertices[j], axis.axis);
        //            if (dot > axis.b2)
        //            {
        //                axis.b2 = dot;
        //                endVerticeCol = (!poly2.closed && j == poly2.vertices.Count - 1) ? poly2.vertices.Count - 1 : -1;
        //            }
        //        }

        //        float dotDir = Vector2.Dot(dir, axis.axis);
        //        float dist = (axis.b2 - axis.a1) / dotDir;

        //        if (dotDir < 0)
        //        {
        //            if (!cr.distance.HasValue || dist > cr.distance)
        //                Jau2(ref cr.distance, ref cr.axisCol, axis, reversed, poly1, poly2, i == edges1.Count - 1, edges2, ref open, dist, endVerticeCol);
        //        }
        //        else if (dotDir > 0)
        //        {
        //            if (!cr.distanceReversed.HasValue || dist < cr.distanceReversed)
        //                Jau2(ref cr.distanceReversed, ref cr.axisColReversed, axis, reversed, poly1, poly2, i == edges1.Count - 1, edges2, ref openReversed, dist, endVerticeCol);
        //        }
        //        else if (axis.b2 - axis.a1 < 0)
        //            return false;

        //    }

        //    return true;
        //}

        //static void Jau2(ref float? distance, ref Vector2 axisCol, Axis axis, int reversed, M_Polygon poly1, M_Polygon poly2, bool lastIndex, List<Vector2> edges2, ref bool open, float dist, int endVerticeCol)
        //{
        //    distance = dist;
        //    axisCol = axis.axis * reversed;

        //    if (!poly1.closed && lastIndex)
        //        open = true;
        //    else
        //    {
        //        open = false;
        //        if (endVerticeCol == 0)
        //        {
        //            if (!poly2.startCorner || Vector2.Dot(new Vector2(-edges2[0].Y, edges2[0].X), axis.axis) > 0)
        //                open = true;
        //        }
        //        else if (endVerticeCol != -1)
        //        {
        //            if (!poly2.endCorner || Vector2.Dot(new Vector2(-edges2[endVerticeCol].Y, edges2[endVerticeCol].X), axis.axis) < 0)
        //                open = true;
        //        }
        //    }
        //}


        #endregion


        #endregion

        #region Circle

        public static bool ColCircleCircle(M_Circle circle1, M_Circle circle2)
        {
            float radius = circle1.radius + circle2.radius;
            Vector2 dist = circle2.pos - circle1.pos;
            return (dist.X * dist.X + dist.Y * dist.Y < radius * radius);
        }

        public static CollisionResult DistCircleCircle(M_Circle circle1, M_Circle circle2)
        {
            Vector2 dist = circle2.pos - circle1.pos;
            float distLength = (float)Math.Sqrt(dist.X * dist.X + dist.Y * dist.Y);

            if (distLength >= circle1.radius + circle2.radius)
                return new CollisionResult();
            else
            {
                CollisionResult cr = new CollisionResult();
                cr.collision = true;
                cr.distance = circle1.radius + circle2.radius - distLength;
                cr.axisCol = -Vector2.Normalize(dist);
                return cr;
            }
        }

        public static CollisionResult DistCircleCircle(M_Circle circle1, M_Circle circle2, Vector2 dir)
        {
            CollisionResult cr = new CollisionResult();

            float radius = circle1.radius + circle2.radius + minDist;

            Vector2 dist = circle2.pos - circle1.pos;

            float?[] results = ABCFormula(dir.X * dir.X + dir.Y * dir.Y
                                         , -2 * (dist.X * dir.X + dist.Y * dir.Y)
                                         , dist.X * dist.X + dist.Y * dist.Y - radius * radius);

            if (results[0] != null)
            {
                if (!float.IsNaN(results[0].Value) && !float.IsNaN(results[1].Value))
                {
                    cr.distance = results[1].Value;
                    cr.distanceReversed = results[0].Value;

                    cr.SetCollisionFromDist();

                    Vector2 colPos = circle1.pos + dir * cr.distance.Value;
                    cr.axisCol = Vector2.Normalize(colPos - circle2.pos);

                    colPos = circle1.pos + dir * cr.distanceReversed.Value;
                    cr.axisColReversed = Vector2.Normalize(colPos - circle2.pos);
                }
                else

                {
                    //TODO: check if this isn't causing any bugs
                }
            }

            return cr;
        }

        public static bool ColCircleSprite(M_Circle circle, Sprite sprite)
        {
            M_Rectangle rect = sprite.GetRectangle();

            if (ColRectangleCircle(rect, circle))
            {
                Matrix transform1 = sprite.GetMatrix();

                Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transform1);
                Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transform1);

                Vector2 yPosIn2 = Vector2.Transform(Vector2.Zero, transform1);

                float radiusSquared = (float)Math.Pow(circle.radius, 2);

                for (int y1 = 0; y1 < sprite.sizey; y1++)
                {
                    Vector2 posIn2 = yPosIn2;

                    for (int x1 = 0; x1 < sprite.sizex; x1++)
                    {
                        float xB = posIn2.X - circle.pos.X;
                        float yB = posIn2.Y - circle.pos.Y;

                        //calc dist from nearest pixel corner (get the nearest)
                        float minDist = (float)(Math.Pow(xB, 2) + Math.Pow(yB, 2));
                        float newDist = (float)(Math.Pow(xB + stepX.X, 2) + Math.Pow(yB + stepX.Y, 2));
                        if (newDist < minDist)
                        {
                            minDist = newDist;
                            newDist = (float)(Math.Pow(xB + stepX.X + stepY.X, 2) + Math.Pow(yB + stepX.Y + stepY.Y, 2));
                        }
                        else
                        {
                            newDist = (float)(Math.Pow(xB + stepY.X, 2) + Math.Pow(yB + stepY.Y, 2));
                            if (newDist < minDist)
                            {
                                minDist = newDist;
                                newDist = (float)(Math.Pow(xB + stepX.X + stepY.X, 2) + Math.Pow(yB + stepX.Y + stepY.Y, 2));
                            }
                        }

                        if (newDist < minDist)
                            minDist = newDist;

                        if (minDist < radiusSquared)
                        {
                            Color color1 = sprite.colorData[x1 + y1 * sprite.sizex];

                            if (color1.A != 0)
                            {
                                return true;
                            }
                        }

                        posIn2 += stepX;
                    }

                    yPosIn2 += stepY;
                }
            }

            return false;
        }

        #endregion

        #region Sprite
        
        public static bool ColSpriteSprite(Sprite sprite1, Sprite sprite2)
        {
            if (sprite1.IsTransformed() || sprite2.IsTransformed())
                return ColSpriteSpriteTransformed(sprite1, sprite2);
            else
                return ColSpriteSpriteIdentity(sprite1, sprite2);
        }

        public static bool ColSpriteSpriteIdentity(Sprite sprite1, Sprite sprite2)
        {
            M_Rectangle rect1 = sprite1.GetRectangle();
            M_Rectangle rect2 = sprite2.GetRectangle();
            if (ColRectangleRectangle(rect1, rect2))
            {
                int xdiff = (int)(rect2.pos.X - rect1.pos.X);
                int ydiff = (int)(rect2.pos.Y - rect1.pos.Y);

                int left = Math.Max(0, (int)rect2.pos.X - (int)rect1.pos.X);
                int right = Math.Min((int)rect1.size.X, (int)(rect2.pos.X + rect2.size.X - rect1.pos.X));
                int top = Math.Max(0, (int)rect2.pos.Y - (int)rect1.pos.Y);
                int bottom = Math.Min((int)rect1.size.Y, (int)(rect2.pos.Y + rect2.size.Y - rect1.pos.Y));
                for (int y = Math.Max(top, ydiff); y < bottom; y++)
                {
                    for (int x = Math.Max(left, xdiff); x < right; x++)
                    {
                        Color color1 = sprite1.colorData[y * sprite1.sizex + x];

                        if (color1.A != 0)
                        {
                            Color color2 = sprite2.colorData[(y - ydiff) * sprite2.sizex + (x - xdiff)];
                            if (color2.A != 0)
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool ColSpriteSpriteTransformed(Sprite sprite1, Sprite sprite2)
        {
            M_Rectangle rect1 = sprite1.GetRectangle();
            M_Rectangle rect2 = sprite2.GetRectangle();

            if (ColRectangleRectangle(rect1, rect2))
            {
                Matrix transform1 = sprite1.GetMatrix();
                Matrix transform2 = sprite2.GetMatrix();

                Matrix transform1To2 = transform1 * Matrix.Invert(transform2);

                Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transform1To2);
                Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transform1To2);

                Vector2 yPosIn2 = Vector2.Transform(Vector2.Zero, transform1To2) + (stepX + stepY) * 0.5f;

                for (int y1 = 0; y1 < sprite1.sizey; y1++)
                {
                    Vector2 posIn2 = yPosIn2;

                    for (int x1 = 0; x1 < sprite1.sizex; x1++)
                    {
                        int xB = (int)(posIn2.X);
                        int yB = (int)(posIn2.Y);

                        if (xB >= 0 && xB < sprite2.sizex &&
                            yB >= 0 && yB < sprite2.sizey)
                        {
                            Color color1 = sprite1.colorData[x1 + y1 * sprite1.sizex];
                            Color color2 = sprite2.colorData[xB + yB * sprite2.sizex];

                            if (color1.A != 0 && color2.A != 0)
                            {
                                return true;
                            }
                        }

                        posIn2 += stepX;
                    }

                    yPosIn2 += stepY;
                }
            }

            return false;
        }


        #endregion

        public static List<Vector2> GetAxes(List<Vector2> edges)
        {
            List<Vector2> axes = new List<Vector2>();

            for (int i = 0; i < edges.Count; i++)
            {
                Vector2 axis = Vector2.Normalize(new Vector2(-edges[i].Y, edges[i].X));
                if (axis.X < 0)
                    axis *= -1f;
                else if (axis.X == 0 && axis.Y < 0)
                    axis.Y *= -1f;

                if (!axes.Contains(axis))
                    axes.Add(axis);
            }

            //if only one axis is available (line) make it two dimensional (+normal axis)
            if (axes.Count <= 2 && axes.Count > 0)
            {
                Vector2 axis = new Vector2(-axes[0].Y, axes[0].X);
                if (axis.X < 0)
                    axis *= -1f;
                else if (axis.X == 0 && axis.Y < 0)
                    axis.Y *= -1f;
                axes.Add(axis);
            }

            return axes;
        }

        public static float[] GetProjection(Vector2 axis, Vector2 pos, List<Vector2> vertices, List<Vector2> edges)
        {
            float[] projection = new float[2];
            bool hasValue = false;

            if (edges.Count == 0)
            {
                float dotPos = ((vertices[0].X + pos.X) * axis.X + (vertices[0].Y + pos.Y) * axis.Y);

                projection[0] = projection[1] = dotPos;

                return new float[0];
            }
            else
            {
                for (int j = 0; j < edges.Count; j++)
                {
                    float dotLength = (edges[j].X * axis.X + edges[j].Y * axis.Y);


                    float dotPos = ((vertices[j].X + pos.X) * axis.X + (vertices[j].Y + pos.Y) * axis.Y);

                    float min, max;
                    if (dotLength >= 0)
                    {
                        min = dotPos;
                        max = dotPos + dotLength;
                    }
                    else
                    {
                        min = dotPos + dotLength;
                        max = dotPos;
                    }

                    if (!hasValue)
                    {
                        projection[0] = min;
                        projection[1] = max;
                        hasValue = true;
                    }
                    else
                    {
                        if (min < projection[0])
                            projection[0] = min;
                        if (max > projection[1])
                            projection[1] = max;
                    }
                }
            }

            return projection;
        }

        public static float?[] ABCFormula(float a, float b, float c)
        {
            float discriminant = b * b - 4f * a * c;
            if (discriminant < 0)
                return new float?[] { null, null };
            else
            {
                float sqrt = (float)Math.Sqrt(discriminant);
                return new float?[] { (-b + sqrt) / (2f * a), (-b - sqrt) / (2f * a) };
            }
        }

        class Axis
        {
            public Vector2 axis;
            public float a1, b2; //a1 <> b2   a2 <> b1

            public Axis(Vector2 edge)
            {
                axis = Vector2.Normalize(new Vector2(-edge.Y, edge.X));
            }
        }
    }
}
