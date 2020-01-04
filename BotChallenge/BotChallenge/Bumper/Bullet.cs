using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuliHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BotChallenge.Bumper
{
    public class Bullet
    {
        public const float RADIUS = 0.2f;
        public const float DAMAGE = 1f;

        private Vector2 lastPos, lastLastPos;
        private Vector2 _pos;
        private Vector2 pos
        {
            get { return _pos; }
            set
            {
                _pos = value;
                mask.pos = _pos;
            }
        }
        private Vector2 velocity;
        private Color color;
        private EnvBumper env;
        private M_Circle mask;
        private int id;
        public int Id { get { return id; } }
        public bool Collectible { get { return id == -1; } }

        public Vector2 Pos => pos;
        public Vector2 LastPos => lastPos;
        public Vector2 Velocity => velocity;
        public Color Color => color;
        public M_Circle Mask => mask;


        public Bullet(Vector2 pos, Vector2 velocity, int id, Color color, EnvBumper env)
        {
            mask = new M_Circle(pos, RADIUS);

            this.pos = this.lastPos = lastLastPos = pos;
            this.velocity = velocity;
            this.id = id;
            this.color = color;
            this.env = env;
        }

        internal bool Update()
        {
            Move();
            velocity *= 0.99f;

            if (velocity != Vector2.Zero)
            {
                if (velocity.Length() < 0.1f)
                {
                    id = -1;
                    color = Color.White;
                }

                if (velocity.Length() < 0.01f)
                    velocity = Vector2.Zero;
            }

            //collision detection for collecting ammo
            if (id == -1)
            {
                int botIndex = -1;
                for (int i = 0; i < env.bots.Length; i++)
                {
                    if (mask.ColCircle(env.bots[i].mask))
                    {
                        if (botIndex == -1)
                            botIndex = i;
                        else
                        {
                            botIndex = -1; //if two bots try to collect the ammo it doesn't work
                            break;
                        }
                    }
                }

                if (botIndex != -1)
                {
                    env.bots[botIndex].ammo++;
                    return true;
                }
            }

            return false;
        }

        private void Move()
        {
            if (velocity != Vector2.Zero)
            {
                #region Move loop
                
                float t = 0;
                while (true)
                {
                    if (velocity.Length() < 0.001f)
                        break;

                    M_Rectangle rect = new M_Rectangle(pos, Vector2.Zero);
                    rect.Enlarge(RADIUS + 1);
                    rect.Expand(velocity);

                    EnvBumper.Bot botHit = null;

                    CollisionResult cr = new CollisionResult();
                    env.EachWall(rect, (x, y) =>
                    {
                        CollisionResult cr2 = mask.DistToPolygon(new M_Rectangle(x, y, 1, 1).ToPolygon(), velocity);
                        cr.AddCollisionResult(cr2, velocity);
                    });
                    if (!Collectible)
                    {
                        for (int i = 0; i < env.bots.Length; i++)
                        {
                            if (env.bots[i].Alive)
                            {
                                CollisionResult cr2 = mask.DistToCircle(env.bots[i].mask, velocity);
                                if (cr.AddCollisionResult(cr2, velocity))
                                    botHit = env.bots[i];
                            }
                        }
                    }

                    if (!cr.distance.HasValue || t + cr.distance.Value >= 1)
                    {
                        if (t > 1)
                            t = 1;
                        pos += velocity * (1 - t);
                        break;
                    }

                    #region step velocity

                    //velocity to nearest dist
                    pos += velocity * cr.distance.Value;
                    t += cr.distance.Value;

                    Vector2 lastMoveN = new Vector2(-velocity.Y, velocity.X);

                    Vector2 tangent = new Vector2(-cr.axisCol.Y, cr.axisCol.X);

                    
                    velocity = Vector2.Dot(velocity, tangent) * tangent - Vector2.Dot(velocity, cr.axisCol) * cr.axisCol;

                    if (botHit != null)
                    {
                        if (id != -1)
                        {
                            float damage = velocity.Length() * DAMAGE;
                            botHit.health -= damage;
                            if (botHit.Id != id)
                                env.bots[id].damageDealt += damage;
                            else
                                env.bots[id].damageDealt -= damage;
                            if (botHit.health < 0)
                            {
                                botHit.health = 0;
                                if (botHit.Id != id)
                                    env.bots[id].kills++;
                                else
                                    env.bots[id].kills--;
                            }
                        }
                    }

                    #endregion
                }

                #endregion
            }
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            DrawM.Vertex.DrawCircle(pos, RADIUS, color, 8f);

            if (lastPos != pos)
            {
                Vector2 n = Vector2.Normalize(pos - lastLastPos);
                n = new Vector2(-n.Y, n.X);
                DrawM.Vertex.DrawPolygon(Vector2.Zero, new List<Vector2>() { lastLastPos, pos - n * RADIUS, pos + n * RADIUS }, color);

                lastLastPos = lastPos;
                lastPos = pos;
            }
        }
    }
}
