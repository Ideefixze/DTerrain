# DTerrain
Destructible terrain in Unity

Simple destructible terrain in Unity based on bitmaps and Quadtree. Pretty efficent and works with Unity Colliders. If you want to use it, clone this repository and see example scene.

![Demo](gif.gif)

## FAQ
### How it works?
I use **Ranges**: [min;max]
Then I make a list of ranges called **Column**.
I fill those ranges using image (if color.alpha>threshold I fill it and use range to remember it effectively).
Now I have a list of **Columns** that make a single chunk. World is made of chunks and when any changes is made to the world - it changes the certain chunk. 

There are many ways you can handle destruction now: if I have shape (list of **Columns**) I can manually remove each pixel (slower, but easier and less buggy), or use **ranges** of this shape.

Each chunk has their sub-texture made from original Texture. I found out that changing a few smaller Textures is much, much faster than changing one big texture. 

Whenever change is made (destruction) recalculates sub-textures on changed chunks and recreates BoxColliders2D using Quadtree.

### How I can make it work faster?
*Increase number of chunks or reduce number of operations done on world per frame. I try my best to optimize it.*
### I can't access any of the components from this package.
*Make sure you add ```using DTerrain;``` at the begining of you scripts.*
### Will it work with my Unity version?
*It should as code is universal and doesn't use version specific tweaks in Unity (only BoxColliders2D).*
### Can I use it for free?
*Yes.*
