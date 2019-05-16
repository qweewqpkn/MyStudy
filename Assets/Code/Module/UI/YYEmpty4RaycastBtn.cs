using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;


public class YYEmpty4RaycastBtn :MaskableGraphic
{
    protected YYEmpty4RaycastBtn()
    {
        useLegacyMeshGeneration = false;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
    }
}

