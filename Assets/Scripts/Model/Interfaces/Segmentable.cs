using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISegmentable
{

    bool UpdateNeighbors(ISegmentable segment);
    bool GetNeighbors();
    bool SetSprites();

}
