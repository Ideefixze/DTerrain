using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DTerrain
{
    public class BasicSimulatedLayer : SimulatedLayer
    {
        public virtual void Start()
        {
            StartCoroutine(WaitForParentInit());
        }

        public virtual void Update()
        {
            if(Input.GetKeyDown(KeyCode.E))
            Simulate();
        }

        IEnumerator WaitForParentInit()
        {
            while (ParentLayer.Chunks == null)
                yield return new WaitForEndOfFrame();

            SpawnChunks();
            InitChunks();

            yield return null;
        }
    }
}
