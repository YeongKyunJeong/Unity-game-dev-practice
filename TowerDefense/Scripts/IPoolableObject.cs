using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolableObject
{
    public void EachPoolingInitialize();

    public void FirstPoolingInitialize();

}
