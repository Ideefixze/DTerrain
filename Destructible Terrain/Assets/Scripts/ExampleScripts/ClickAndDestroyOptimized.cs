using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DTerrain
{
    /// <summary>
    /// Destroys a circle and builds a circle but only on secondary layer.
    /// Primary serves as logical layer for reducing sprite renderers and only handles collisions.
    /// Used in SampleScene2.
    /// </summary>
    public class ClickAndDestroyOptimized : ClickAndDestroy
    {

        protected override void OnLeftMouseButtonClick()
        {

            Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition) - primaryLayer.transform.position;

            primaryLayer?.Paint(new PaintingParameters() 
            { 
                Color = Color.clear, 
                Position = new Vector2Int((int)(p.x * primaryLayer.PPU) - circleSize, (int)(p.y * primaryLayer.PPU) - circleSize), 
                Shape = destroyCircle, 
                PaintingMode=PaintingMode.REPLACE_COLOR,
                DestructionMode = DestructionMode.DESTROY
            });

            secondaryLayer?.Paint(new PaintingParameters() 
            {
                Color = Color.clear,
                Position = new Vector2Int((int)(p.x * secondaryLayer.PPU) - circleSize, (int)(p.y * secondaryLayer.PPU) - circleSize), 
                Shape = destroyCircle, 
                PaintingMode=PaintingMode.REPLACE_COLOR,
                DestructionMode = DestructionMode.NONE
            });
            
        }

        protected override void OnRightMouseButtonClick()
        {
            Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition) - primaryLayer.transform.position;
            primaryLayer?.Paint(new PaintingParameters()
            {
                Color = Color.black,
                Position = new Vector2Int((int)(p.x * primaryLayer.PPU) - circleSize, (int)(p.y * primaryLayer.PPU) - circleSize),
                Shape = destroyCircle,
                PaintingMode = PaintingMode.NONE,
                DestructionMode = DestructionMode.BUILD
            });

            secondaryLayer?.Paint(new PaintingParameters()
            {
                Color = Color.black,
                Position = new Vector2Int((int)(p.x * secondaryLayer.PPU) - circleSize, (int)(p.y * secondaryLayer.PPU) - circleSize),
                Shape = destroyCircle,
                PaintingMode = PaintingMode.REPLACE_COLOR,
                DestructionMode = DestructionMode.BUILD
            });

        }
    }
}
