# DTerrain
Destructible terrain in Unity

Simple destructible terrain in Unity based on bitmaps and Quadtree. Pretty efficent and works with Unity Colliders. If you want to use it, clone this repository and see example scene.

![Demo](gif.gif)

## FAQ
### How I can make it work faster?
*Increase number of chunks or reduce number of operations done on world per frame. I try my best to optimize it.*
### I can't access any of the components from this package.
*Make sure you add ```using DTerrain;``` at the begining of you scripts.*
### Will it work with my Unity version?
*It should as code is universal and doesn't use version specific tweaks in Unity (only BoxColliders2D).*
### Can I use it for free?
*Yes.*
