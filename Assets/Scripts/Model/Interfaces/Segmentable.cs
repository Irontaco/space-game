using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISegmentable
{

    void UpdateNeighbors();
    void UpdateSelf();
    void CreateSegment();
    void DeleteSegment();

}
