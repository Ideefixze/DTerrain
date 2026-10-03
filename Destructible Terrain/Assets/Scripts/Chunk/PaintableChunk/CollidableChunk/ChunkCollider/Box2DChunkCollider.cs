using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    /// <summary>
    /// Chunk collider that builds merged rectangles straight from the chunk's columns (RectMerge)
    /// instead of walking a quadtree.
    ///
    /// Packed with crust the rectangles go to Box2D-Packed: the chunk is one static body and every
    /// rectangle a box shape (b2MakeOffsetBox); a rebuild replaces the chunk's shapes, there are no
    /// components. In Unity the same rectangles become BoxCollider2D components, reused by index
    /// (no lookups), so the scene behaves as before with fewer boxes.
    ///
    /// Boxes are in chunk-local units: (centerX, centerY, halfWidth, halfHeight) per rectangle.
    /// A chunk needs a kind-5 collider row: crust's unity_pack makes one for any script of this name.
    /// </summary>
    public class Box2DChunkCollider : MonoBehaviour, IChunkCollider
    {
        /// <summary>
        /// Most shapes a chunk may hold when packed (the glue's table is sized once). Rectangles past
        /// it are not sent and Overflow is set. Unity has no such limit.
        /// </summary>
        public int MaxShapes = 256;

        /// <summary>True if the last rebuild had more rectangles than MaxShapes.</summary>
        public bool Overflow { get; private set; }

        private List<PixelRect> rects = new List<PixelRect>();

#if !CRUST
        private readonly List<BoxCollider2D> colliders = new List<BoxCollider2D>();
#endif

        /// <summary>
        /// Prepares all colliders of the chunk from its columns.
        /// </summary>
        /// <param name="pixelData">List of columns (chunk data) to find potential colliding pixels</param>
        public void UpdateColliders(List<Column> pixelData, ITextureSource textureSource)
        {
            float ppu = textureSource.PPU;

            rects.Clear();
            RectMerge.FromColumns(ref pixelData, ref rects);

#if CRUST
            int n = rects.Count;
            Overflow = n > MaxShapes;
            if (Overflow) n = MaxShapes;

            Box2DTerrain.Begin(gameObject);
            for (int i = 0; i < n; i++)
            {
                PixelRect p = rects[i];
                Box2DTerrain.AddBox(gameObject, (p.X + p.W / 2f) / ppu, (p.Y + p.H / 2f) / ppu,
                                    p.W / 2f / ppu, p.H / 2f / ppu);
            }
            Box2DTerrain.End(gameObject);
#else
            Overflow = false;
            GetComponents<BoxCollider2D>(colliders);

            for (int i = 0; i < rects.Count; i++)
            {
                PixelRect p = rects[i];
                Vector2 offset = new Vector2((p.X + p.W / 2f) / ppu, (p.Y + p.H / 2f) / ppu);
                Vector2 size = new Vector2(p.W / ppu, p.H / ppu);

                BoxCollider2D b;
                if (i < colliders.Count)
                    b = colliders[i];
                else
                    b = gameObject.AddComponent<BoxCollider2D>();

                if (b.offset != offset) b.offset = offset;
                if (b.size != size) b.size = size;
            }

            for (int i = colliders.Count - 1; i >= rects.Count; i--)
                Destroy(colliders[i]);
#endif
        }
    }

#if CRUST
    /// <summary>
    /// The hook between a terrain chunk and Box2D-Packed (b2u_terrain_set in physics_box2d.c).
    /// unity_pack lowers these calls; there is no body here. No arrays: Begin, one AddBox per
    /// rectangle, End -- which replaces the shapes of go's terrain collider.
    /// </summary>
    public static class Box2DTerrain
    {
        public static void Begin(GameObject go) { }

        /// <summary>A box relative to go's position: center x, center y, half width, half height.</summary>
        public static void AddBox(GameObject go, float cx, float cy, float hw, float hh) { }

        public static void End(GameObject go) { }

        /// <summary>
        /// A chunk's boundary as chains instead of boxes: BeginChains, then per chain ChainBegin and
        /// a ChainPoint for each point, then EndChains. Points are relative to go's position, the
        /// ground is on the left of the way and the chain collides on its right. loop is 1 for a
        /// closed chain.
        /// </summary>
        public static void BeginChains(GameObject go) { }
        public static void ChainBegin(GameObject go, int loop) { }
        public static void ChainPoint(GameObject go, float x, float y) { }
        public static void EndChains(GameObject go) { }
    }
#endif
}
