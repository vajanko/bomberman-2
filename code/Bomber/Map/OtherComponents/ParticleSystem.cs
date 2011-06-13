using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Bomber
{
    class ParticleSystem : MapObject
    {
        // list of all living particles in this particle system
        protected List<ParticleData> particles;
        protected Random generator;

        public override void Initialize()
        {
            base.Initialize();
            particles.Clear();  // empty list
        }
        public override void Draw(GameTime gameTime)
        {
            ParticleData data;
            for (int i = 0; i < particles.Count; i++)
            {
                data = particles[i];
                SpriteBatch.Draw(texture, data.Position, null, data.ModColor, i, new Vector2(256, 256),
                    data.Scaling, SpriteEffects.None, 1);
            }
        }
        public override void Update(GameTime gameTime)
        {   // get the current time
            float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
            ParticleData data;  // points to the current particle

            // update all particles, remove old ones
            // start at the end, as we need to move all particles to left, when some in the middle is removed
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                data = particles[i];
                float timeAlive = now - data.BirthTime;
                if (timeAlive > data.MaxAge)    // too old particle
                    particles.RemoveAt(i);
                else
                {   // relAge is 0 at it's beginning and 1 at the end of it's life
                    float relAge = timeAlive / (float)data.MaxAge;

                    // update position
                    data.Position = 0.5f * data.Accelaration * relAge * relAge + data.Direction * relAge + data.OrginalPosition;

                    // update color
                    data.ModColor = new Color(new Vector4(1.0f - relAge));

                    // update size
                    Vector2 fromCenter = data.Position - data.OrginalPosition;
                    float distance = fromCenter.Length();
                    data.Scaling = (50 + distance) / 200;

                    particles[i] = data;
                }
            }
        }

        public void AddParticles(Vector2 position, int count, double age, float size, GameTime gameTime)
        {
            for (int i = 0; i < count; i++)
                addParticle(position, size, age, gameTime);
        }

        protected void addParticle(Vector2 pos, float size, double maxAge, GameTime gameTime)
        {
            ParticleData data = new ParticleData();
            data.OrginalPosition = pos;
            data.Position = pos;
            data.BirthTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            data.MaxAge = maxAge;
            data.Scaling = size; // 0.25f;
            data.ModColor = Color.White;

            float particleDistance = (float)generator.NextDouble() * size;
            Vector2 displacement = new Vector2(particleDistance, 0);
            float angle = MathHelper.ToRadians(generator.Next(360));
            displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(angle));

            data.Direction = displacement;
            data.Accelaration = 3.0f * data.Direction;

            particles.Add(data);
        }

        #region Constructors

        /// <summary>
        /// Create an instance of particle system
        /// </summary>
        /// <param name="map">Map where this component will be placed</param>
        /// <param name="textureFile">Path to the file where texture for this particle system is stored</param>
        public ParticleSystem(Map map, string textureFile) : base(map, textureFile) 
        {
            particles = new List<ParticleData>();
            generator = new Random();
        }

        #endregion
    }

    /// <summary>
    /// Holds data for an individual particle
    /// </summary>
    public struct ParticleData
    {
        public float BirthTime;
        public double MaxAge;
        public Vector2 OrginalPosition;
        public Vector2 Accelaration;
        public Vector2 Direction;
        public Vector2 Position;
        public float Scaling;
        public Color ModColor;
    }
}
