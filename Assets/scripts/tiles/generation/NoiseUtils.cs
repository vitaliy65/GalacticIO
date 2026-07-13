using UnityEngine;

namespace tiles
{
    /// <summary>
    /// A single call to Mathf.PerlinNoise gives smooth but fairly uniform, blobby
    /// output - fine for a quick test, but it doesn't look like real terrain.
    /// Real-looking maps sum several "octaves" of noise together: a low-frequency
    /// layer for large landmasses/continents, plus progressively higher-frequency,
    /// lower-amplitude layers for hills and local detail. This is usually called
    /// fractal noise or fBm (fractal Brownian motion).
    ///
    /// Reference: https://www.redblobgames.com/maps/terrain-from-noise/
    /// </summary>
    public static class NoiseUtils
    {
        /// <param name="x">World-space X of the sample point.</param>
        /// <param name="y">World-space Y of the sample point (this project's hex
        /// grid lies on the XY plane - see HexBiomeGenerator).</param>
        /// <param name="octaves">How many noise layers to stack. 3-5 is typical;
        /// more adds finer detail but costs more per-tile.</param>
        /// <param name="persistence">How much each successive octave's amplitude
        /// shrinks. 0.5 is a common default (each octave contributes half as much
        /// as the last).</param>
        /// <param name="lacunarity">How much each successive octave's frequency
        /// grows. 2 is a common default (each octave is twice as "zoomed in").</param>
        /// <param name="scale">Overall zoom level of the noise. Larger values =
        /// bigger, smoother features; smaller values = noisier, busier output.
        /// Tune this relative to your hex tile spacing, not to pixels.</param>
        /// <param name="offset">Shifts the sampled region. Change this (e.g. to a
        /// random value per playthrough) to get a different map from the same
        /// settings; keep it fixed to regenerate the same map deterministically.</param>
        /// <returns>A value normalized to roughly [0, 1].</returns>
        public static float FractalNoise(
            float x,
            float y,
            int octaves,
            float persistence,
            float lacunarity,
            float scale,
            Vector2 offset)
        {
            scale = Mathf.Max(scale, 0.0001f);

            float amplitude = 1f;
            float frequency = 1f;
            float noiseSum = 0f;
            float amplitudeSum = 0f;

            for (int i = 0; i < Mathf.Max(octaves, 1); i++)
            {
                float sampleX = (x + offset.x) / scale * frequency;
                float sampleY = (y + offset.y) / scale * frequency;

                // Mathf.PerlinNoise returns roughly [0, 1]; remap to [-1, 1] so
                // octaves can partially cancel each other like real fBm noise
                // instead of only ever stacking upward.
                float sample = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;

                noiseSum += sample * amplitude;
                amplitudeSum += amplitude;

                amplitude *= persistence;
                frequency *= lacunarity;
            }

            if (amplitudeSum <= 0f)
            {
                return 0.5f;
            }

            // noiseSum is in [-amplitudeSum, amplitudeSum]; bring it back to [0, 1].
            return Mathf.InverseLerp(-amplitudeSum, amplitudeSum, noiseSum);
        }
    }
}
