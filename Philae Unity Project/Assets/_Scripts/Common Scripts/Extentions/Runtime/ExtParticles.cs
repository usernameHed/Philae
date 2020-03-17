using UnityEngine;

namespace hedCommon.extension.runtime
{
    public static class ExtParticles
    {
        public static void ChangeColorParticleImmediatly(ParticleSystem particle, Color color)
        {
            ParticleSystem.MainModule main = particle.main;
            main.startColor = color;

            // initialize an array the size of our current particle count
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particle.particleCount];
            // *pass* this array to GetParticles...
            int num = particle.GetParticles(particles);
            for (int k = 0; k < num; k++)
            {
                particles[k].color = color;
            }
            // re-assign modified array
            particle.SetParticles(particles, num);
        }
    }
}