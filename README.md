# DTerrain
Destructible terrain in Unity

Simple **destructible terrain** in Unity based on bitmaps and Quadtree. Pretty efficent and works with Unity Colliders. Very reusable and customizable for your own needs. This beginner friendly tool will take your game to the next level. If you want to use it, clone this repository and see example scene.

Also provides additional functionalities as **painting on customizable layers**.

![Demo](dterrainexample_new.gif)

## FAQ
### How destructible terrain works?
I use **Ranges**: [min;max]
Then I make a list of ranges called **Column**.
I fill those ranges using image (if color.alpha>threshold I fill it and use range to remember it effectively).
Now I have a list of **Columns** that make a single **Chunk**. **Layer** is made of chunks and when any changes are made to the **Layer** - it changes the certain chunk (or chunks). 

There are many ways you can handle destruction: if I have shape (list of **Columns**) I can manually remove each pixel, or use **ranges** of this shape to delete effectively bigger areas of terrain.

Each **Chunk** has their sub-texture made from original Texture. I found out that changing a few smaller Textures is much, much faster than changing one big texture. 

Whenever a change is made (destruction) each **Chunk** recalculates sub-textures based on their **Columns** and recreates BoxColliders2D using Quadtree to fit new terrain.

### Would it work in realtime game?
*Yes. Example scene rarely goes below ~80 FPS druing per frame destruction on my machine.*
### How I can make it work even faster?
*Slightly increase number of chunks or reduce number of operations done on world per frame.*
*Reduce unnecessary per frame changes to the terrain as it makes chunk to recalculate it's colliders.*
*Recalculating colliders takes about 90% of all computation for DTerrain, keep that in mind.*
*Also, chunks are based on SpriteRenderers meaning the more chunks camera sees the slower it will work. To reduce lag separate typical collision layer into two layers: visible (only sprite renderer - one big chunk) and logic (only collision - many chunks). See ``SampleScene2Optimized`` to see the idea behind it.*
### I can't access any of the components from this package.
*Make sure you add ```using DTerrain;``` at the begining of you scripts.*
### Will it work with my Unity version?
*It should as code is universal and doesn't use version specific tweaks in Unity (only BoxColliders2D, SpriteRenderers and SpriteMasks).*
### Can I use it for free?
*Yes. Now and forever. You don't have to credit me, but I'd really like to know if you built something meaningful with it.* 

## Special thanks:
- **/u/idbrii** for pointing out a fix that nearly doubled the FPS.

## Features in the future:
- Rewriting the whole code to be more expandable (DONE ✔️)
- Adding mesh optimization (DONE ✔️ - separate collision logic and visible layers: see ``SampleScene2``)
- Adding wiki on how to use it (IN PROGRESS 🔜) 

## Crust port: plan

This fork is being ported to C with [crust](https://github.com/brentharts/crust),
aimed at small, bounded games: few objects, every count known when the game is
packed. The pieces go where they fit:

* **Model** (`Range`, `Column`, `Shape`): plain data and loops, through crust's
  C# subset (CSRUST.md).
* **Chunks, layers, texture sources** (MonoBehaviours): through crust's
  unity_pack.
* **Colliders**: straight into [Box2D-Packed](https://github.com/crustos/box2d)
  shapes, instead of `BoxCollider2D` components.
* **Inspector code** (`HelpAttribute`) is not part of it: it stays behind
  `#if !CRUST` (crust defines `CRUST`), as it is already behind `UNITY_EDITOR`.

Every change keeps the source plain C# that Unity compiles as before.

### Status

The model -- `Range`, `Column`, `Shape`, `PixelRect`, `ColumnQuadTree`, `RectMerge`, `ChainTrace` -- translates
through crust's C# subset **and runs the same**: `Tests~/crust_equivalence.sh` runs 300 random chunk sets
through the C# (mono) and through the translated C (gcc, AddressSanitizer / UBSan) and compares every
rectangle and chain, which come out identical. Plain C# tests of the model are `Tests~/run.sh`. `UnityExtensions`
and the interfaces (`IChunk`, `IChunkCollider`, `ILayer`, `ITextureSource`, `ITextureGenerator`) translate too.
The MonoBehaviours belong to unity_pack, and the rest stop at a named line with a reason: the gaps are listed
under step 6.

What the model does for crust: algorithms take their lists and result objects as `ref` (lent, not copied; the
same in Unity); `Range`'s `Equals(object)` and `IEquatable` are for Unity only (`#if CRUST` leaves them out; its
`+` and `-` translate as they are) and `Column` removes a range with a loop; `ChainTrace` takes a neighbor chunk's border as a strip of pixels
(`SetLeft` ...) rather than holding its list.

### Where the time goes, and what changes

The README above says it: recalculating colliders is about 90% of the work.
Three things make it so, and each has a direct fix.

1. **The quadtree tests pixel by pixel.** `QuadTreeToRect` asks
   `chunk[i].isWithin(j)` -- a scan of the column's ranges -- for every pixel of
   every node, at every level. The data is already run-length: whether a
   rectangle is all ground, all air or mixed is a question per *column*
   (does one range cover rows y..y+h, does any range touch them), so a node
   costs columns x ranges, not pixels x ranges. Same rectangles, same result,
   in Unity too.
2. **Colliders are components, created and destroyed.** Every rebuild
   `AddComponent<BoxCollider2D>`s and `Destroy`s, and matches old to new with
   `colls.Find(lambda)` -- quadratic in the number of boxes. Packed, a chunk is
   one static Box2D body and its rectangles are box shapes
   (`b2MakeOffsetBox`): a rebuild destroys the chunk's shapes and creates the
   new ones, no component bookkeeping. Further, in order of payoff:
   * **merge rectangles** greedily along rows before making shapes -- far fewer
     shapes than quadtree leaves;
   * **outline instead of fill**: the terrain's boundary as `b2ChainShape`s,
     traced from the column ranges. A solid region becomes one chain however
     large it is, and chains are smooth for things rolling over them: no
     catching on the seams between boxes.
3. **Every span is an object.** `Range` is a class, so each `[min;max]` is a
   heap allocation, and destruction allocates them by the hundred. As a
   struct it is two ints in a list -- in C a plain array. One care point:
   `Range` overrides `Equals` with value equality, and `Column` removes ranges
   with `List.Remove`, which uses it; the port keeps that meaning (it must not
   become reference equality).

### Plan, in order


2. **Model as values** (DONE ✔️). `Range` a struct; `Column`'s ranges a list of them. `Column` removes
   a range with a loop (`RemoveEqual`, value equality), so crust needs no `List.Remove` through `Equals`.
3. **Column-wise quadtree** (DONE ✔️; fix 1 above) -- a change in plain C#, faster in
   Unity as well.
4. **Box2D shapes per chunk** (DONE ✔️, fix 2): one static body a chunk. `RectMerge` merges the
   column ranges into rectangles along rows (37% as many shapes as the quadtree on random data) and
   `Box2DChunkCollider` hands them to Box2D-Packed as box shapes; `ChainTrace` traces the boundary
   instead, and `Box2DChainChunkCollider` hands it over as chains (ground on the left of the way, a region
   is one chain however large it is; the ends of a chain at a chunk border meet the neighbor chunk's,
   which the layer links up and which rebuilds when the other side changes). In Unity the same two
   colliders make `BoxCollider2D` / `EdgeCollider2D` components, without the `Find`. A chunk's rotation
   when packed applies. unity_pack and Box2D-Packed carry the rest (kind 5, `Box2DTerrain`); see their
   UNITY_PACK.md. Not done: a rotation that changes at run time, and PPU of 200 or more for chains.
5. **Bounded counts.** The chunk grid follows from the texture size and the
   chunk count, both known when the game is packed: `[MaxInstances(N)]` on the
   chunk and layer classes, tables sized once. Per-chunk textures update in
   place: only the rectangle that changed is uploaded (`glTexSubImage2D` in the
   GLES hosts), not the whole chunk through `SetPixels` / `Apply`.
6. **C# subset gaps** the scripts hit, each small: `?.` (CollidableChunk,
   the example scripts), `base.Method()` / `base.Property` (ComplexChunk,
   BlankSingleTextureSource), string interpolation in log text (PaintableLayer:
   behind `#if !CRUST`, or a crust lowering), `new Color[n]`, `List.Find` with a
   lambda (the loop it stands for, after step 4 it is gone anyway), a generic
   base class (`PaintableLayer<T>`), and a field named like its enum
   (`PaintingMode PaintingMode`).
