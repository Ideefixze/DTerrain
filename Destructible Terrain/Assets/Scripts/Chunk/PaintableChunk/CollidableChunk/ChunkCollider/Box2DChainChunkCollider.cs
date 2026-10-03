using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    /// <summary>
    /// Chunk collider that traces the boundary of the chunk's ground as chains (ChainTrace) instead
    /// of filling it with boxes. A solid region is one chain however large it is, and a body rolling
    /// over it does not catch on the seams between boxes.
    ///
    /// Packed with crust the chains go to Box2D-Packed: the chunk is one static body and every chain a
    /// b2Chain, the ground on the left of the way and the collision on the right (the air); a chain that
    /// did not change in a rebuild keeps its shapes. In Unity the same chains become EdgeCollider2D
    /// components (two-sided there).
    ///
    /// The chain along a chunk border needs the chunks next to this one: they are the CollidableChunk's
    /// Left / Right / Down / UpNeighbor, which the layer sets, and a change next to a border makes the
    /// neighbor rebuild. Points are in chunk-local units. Pixels per unit must stay under 200
    /// (Box2D refuses edges shorter than 0.005).
    /// </summary>
    public class Box2DChainChunkCollider : MonoBehaviour, IChunkCollider
    {
        /// <summary>Most points of all chains of a chunk when packed (the glue's table is sized once).</summary>
        public int MaxPoints = 1024;

        /// <summary>Most chains of a chunk when packed.</summary>
        public int MaxChains = 64;

        /// <summary>True if the last rebuild had more chains or points than the limits; the rest were cut.</summary>
        public bool Overflow { get; private set; }

        private ChainSet chains = new ChainSet();

#if !CRUST
        private readonly List<EdgeCollider2D> colliders = new List<EdgeCollider2D>();
#endif

        /// <summary>
        /// Traces the chunk's columns, and the pixels of its neighbors along the borders, into chains.
        /// </summary>
        public void UpdateColliders(List<Column> pixelData, ITextureSource textureSource)
        {
            float ppu = textureSource.PPU;
            int width = textureSource.Texture.width;
            int height = textureSource.Texture.height;

            ChainTrace trace = new ChainTrace(width, height);
            CollidableChunk chunk = GetComponent<CollidableChunk>();
            if (chunk != null)
            {
                if (chunk.LeftNeighbor != null)
                {
                    List<Column> cols = chunk.LeftNeighbor.Columns;
                    trace.SetLeft(ref cols);
                }
                if (chunk.RightNeighbor != null)
                {
                    List<Column> cols = chunk.RightNeighbor.Columns;
                    trace.SetRight(ref cols);
                }
                if (chunk.DownNeighbor != null)
                {
                    List<Column> cols = chunk.DownNeighbor.Columns;
                    trace.SetDown(ref cols);
                }
                if (chunk.UpNeighbor != null)
                {
                    List<Column> cols = chunk.UpNeighbor.Columns;
                    trace.SetUp(ref cols);
                }
            }

            chains.Clear();
            trace.Trace(ref pixelData, ref chains);

            //The chains that fit the limits, in order
            int fit = 0;
            int points = 0;
            Overflow = false;
            for (int k = 0; k < chains.ChainCount; k++)
            {
                if (fit >= MaxChains || points + chains.Counts[k] > MaxPoints)
                {
                    Overflow = true;
                    break;
                }
                points += chains.Counts[k];
                fit++;
            }

#if CRUST
            Box2DTerrain.BeginChains(gameObject);
            for (int k = 0; k < fit; k++)
            {
                Box2DTerrain.ChainBegin(gameObject, chains.Loop[k]);
                int first = chains.Starts[k];
                for (int i = 0; i < chains.Counts[k]; i++)
                {
                    Box2DTerrain.ChainPoint(gameObject, chains.Points[2 * (first + i)] / ppu,
                                            chains.Points[2 * (first + i) + 1] / ppu);
                }
            }
            Box2DTerrain.EndChains(gameObject);
#else
            GetComponents<EdgeCollider2D>(colliders);

            for (int k = 0; k < fit; k++)
            {
                int first = chains.Starts[k];
                int n = chains.Counts[k];
                bool loop = chains.Loop[k] == 1;

                Vector2[] pts = new Vector2[loop ? n + 1 : n];
                for (int i = 0; i < n; i++)
                    pts[i] = new Vector2(chains.Points[2 * (first + i)] / ppu, chains.Points[2 * (first + i) + 1] / ppu);
                if (loop) pts[n] = pts[0];

                EdgeCollider2D e;
                if (k < colliders.Count)
                    e = colliders[k];
                else
                    e = gameObject.AddComponent<EdgeCollider2D>();
                e.points = pts;
            }

            for (int k = colliders.Count - 1; k >= fit; k--)
                Destroy(colliders[k]);
#endif
        }
    }
}
